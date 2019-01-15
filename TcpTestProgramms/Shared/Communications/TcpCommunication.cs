 using Shared.Contract;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// Wir haben eine TcpCommunication eine klasse die die Verbindung zwischen Server und client regelt 
/// </summary>
namespace Shared.Communications
{
    public class TcpCommunication : ICommunication
    {
        public TcpClient _client { get; set; }
        private NetworkStream _nwStream;
        private bool _nwStreamNotSet = true;
        public bool IsMaster { get; set; } = false;
       
        private MemoryStream _localBuffer;
        private List<DataPackage> _packageQueue;

        private readonly object _lock;
        private BackgroundWorker _backgroundWorker;
        private BackgroundWorker _backgroundWorker2;

        private bool _isCommunicationRunning = true;

        public TcpCommunication()
            : this(new TcpClient())
        { }

        public TcpCommunication(TcpClient client)
        {
            _client = client;
            _packageQueue = new List<DataPackage>();

            _lock = new object();

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += (_, __) => CheckNWStreamUpdates();
            _backgroundWorker.RunWorkerAsync();

            _backgroundWorker2 = new BackgroundWorker();
            _backgroundWorker2.DoWork += (_, __) => GetStreamWithStyle();
            _backgroundWorker2.RunWorkerAsync();
        }

        private void GetStreamWithStyle()
        {
            while (_nwStreamNotSet)
                try
                {
                    _nwStream = _client.GetStream();
                    _nwStreamNotSet = false;
                }
                catch
                {
                    return;
                }
        }

        public void SetNWStream()
        {
            _nwStream = _client.GetStream();
        }

        public void AddPackage(DataPackage dataPackage)
        {
            lock (_lock)
                _packageQueue.Add(dataPackage);
        }

        public bool IsDataAvailable()
        {
            lock (_lock)
                return _packageQueue.Count != 0;
        }

        public DataPackage Receive()
        {
            lock (_lock)
            {
				DataPackage element = _packageQueue.First();
                _packageQueue.RemoveAt(0);
                return element;
            }
        }

        public void ReceiveCallback(Action<DataPackage> receiveCallback)
        {
            throw new NotImplementedException();
        }

        public void Send(DataPackage data)
        {
			byte[] bytesToSend = data.ToByteArrayUTF();
            _nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        private void CheckNWStreamUpdates()
        {
            //if (_NWStreamNotSet)
            //    return;

            while (_isCommunicationRunning)
            {
                try
                {
                    if (_nwStream != null && _nwStream.DataAvailable)
                    {
                        ReadDataToBuffer();
                        CheckForNewPackages();
                    }
                    else
                        //wichtig sonst ript dein PC
                        Thread.Sleep(1);
                }
                catch
                {
                    return;
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                try
                {
					Socket client = _client.Client;
                    if (client.Poll(0, SelectMode.SelectRead))
                    {
						byte[] buff = new byte[1];
                        if (client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public void Stop()
        {
            _isCommunicationRunning = false;
            _client.Close();
            //_nwStream.Close();
        }

        private void CheckForNewPackages()
        {
            if (_nwStream == null)
                return;

            if (_localBuffer.Length < 2 * sizeof(Int32))
                return;

            _localBuffer.Seek(0, SeekOrigin.Begin);

            using (var reader = new BinaryReader(_localBuffer))
            {
                var package = new DataPackage
                {
                    Size = reader.ReadInt32(),
                    Header = (ProtocolActionEnum)reader.ReadInt32()
                };
                if (package.Size <= _localBuffer.Length)
                {
                    _localBuffer.Position = 2 * sizeof(Int32);

					int sizeOfPayload = package.Size - 2 * sizeof(Int32);

					byte[] bytesToRead = new byte[sizeOfPayload];
                    _localBuffer.Read(bytesToRead, 0, sizeOfPayload);
                    package.Payload = Encoding.UTF8.GetString(bytesToRead, 0, bytesToRead.Length);

                    lock (_lock)
                        _packageQueue.Add(package);
                }
            }
        }

        private void ReadDataToBuffer()
        {

            if (_nwStream == null)
                return;

			byte[] bytesToRead = new byte[_client.ReceiveBufferSize];
            _nwStream.Read(bytesToRead, 0, _client.ReceiveBufferSize);

            _localBuffer = new MemoryStream();
            _localBuffer.Seek(0, SeekOrigin.End);
            _localBuffer.Write(bytesToRead, 0, bytesToRead.Length);
        }       
    }
}
