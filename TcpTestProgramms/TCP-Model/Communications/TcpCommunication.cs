using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCP_Model
{
    public class TcpCommunication : ICommunication
    {
        private TcpClient _client;
        private NetworkStream _nwStream;

        private MemoryStream _localBuffer;
        private List<DataPackage> _packageQueue;

        private readonly object _lock;
        private BackgroundWorker _backgroundWorker;


        public TcpCommunication()
            : this(new TcpClient("127.0.0.1", 8080))
        { }

        public TcpCommunication(TcpClient client)
        {
            _client = client;
            _nwStream = _client.GetStream();

            _localBuffer = new MemoryStream();
            _packageQueue = new List<DataPackage>();

            _lock = new object();
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += (_, __) => CheckForUpdates();
            _backgroundWorker.RunWorkerAsync();
        }


        public bool IsDataAvailable()
        {
            lock(_lock)
                return _packageQueue.Count != 0;
        }

        public DataPackage Receive()
        {
            lock (_lock)
            {
                var element = _packageQueue.First();
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
            _nwStream = _client.GetStream();
            var bytesToSend = data.ToByteArray();
            Console.WriteLine($"Sending : Header: {data.Header}, Size: {data.Size}, Payload: {data.Payload}");
            _nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        private void CheckForUpdates()
        {
            while (true)
            {
                if (_nwStream.DataAvailable)
                {
                    ReadDataToBuffer();
                    CheckForNewPackages();
                }
                else
                    Thread.Sleep(1);
            }

        }

        private void CheckForNewPackages()
        {
            if (_localBuffer.Length < 2 * sizeof(Int32))
                return;

            _localBuffer.Seek(0, SeekOrigin.Begin);
            using (var reader = new BinaryReader(_localBuffer))
            {
                var package = new DataPackage();
                package.Size = reader.ReadInt32();
                package.Header = (ProtocolAction)reader.ReadInt32();
                if (package.Size <= _localBuffer.Length )
                {
                    _localBuffer.Position = 2 * sizeof(Int32);

                    var sizeOfPayload = package.Size - 2 * sizeof(Int32);
                    var bytesToRead = new byte[sizeOfPayload];
                    var bytesRead = _localBuffer.Read(bytesToRead, 0, sizeOfPayload);
                    package.Payload = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

                    lock(_lock)
                        _packageQueue.Add(package);
                }
            }
        }

        private void ReadDataToBuffer()
        {
            if (_nwStream == null)
                return;

            var bytesToRead = new byte[_client.ReceiveBufferSize];
            var bytesRead = _nwStream.Read(bytesToRead, 0, _client.ReceiveBufferSize);
            _localBuffer.Seek(0, SeekOrigin.End);
            _localBuffer.Write(bytesToRead, 0, bytesRead);
        }
    }
}
