using Shared.Communications;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCP_Client.UDP
{
    public class UdpClientUnit
    {
        private UdpClient _udpClient;
        public List<DataPackage> _dataList;
        public bool _closed = false;

        public UdpClientUnit()
        {
            _udpClient = new UdpClient(7075);
            _dataList = new List<DataPackage>();
        }

        public void StartListening()
        {
            if (_udpClient.Client != null)
                _udpClient.BeginReceive(Receive, new object());
        }

        public void Receive(IAsyncResult ar)
        {
            if (!_closed)
            {
                var _receiveEndPoint = new IPEndPoint(IPAddress.Any, 7070);
                var bytes = _udpClient.EndReceive(ar, ref _receiveEndPoint);

                //string message = Encoding.ASCII.GetString(bytes);
                //Console.WriteLine(message);

                var localBuffer = new MemoryStream();
                localBuffer.Write(bytes, 0, bytes.Length);

                if (localBuffer.Length < 2 * sizeof(Int32))
                    return;

                localBuffer.Seek(0, SeekOrigin.Begin);
                using (var reader = new BinaryReader(localBuffer))
                {
                    var package = new DataPackage
                    {
                        Size = reader.ReadInt32(),
                        Header = (ProtocolActionEnum)reader.ReadInt32()
                    };
                    if (package.Size <= localBuffer.Length)
                    {
                        localBuffer.Position = 2 * sizeof(Int32);
                        var sizeOfPayload = package.Size - 2 * sizeof(Int32);
                        var bytesToRead = new byte[sizeOfPayload];
                        localBuffer.Read(bytesToRead, 0, sizeOfPayload);
                        package.Payload = Encoding.ASCII.GetString(bytesToRead, 0, bytesToRead.Length);

                        _dataList.Add(package);
                        Thread.Sleep(1);
                        SendRequest();
                    }
                }
            }
        }

        public void StopListening()
        {
            _closed = true;
            //_UdpClient.Dispose();
            //_UdpClient.Close();
        }


        public void SendRequest()
        {
            if (!_closed)
            {
                var  broadcastMessage = "is there a Server";
                var bytes = Encoding.ASCII.GetBytes(broadcastMessage);

                var _ipEndPoint = new IPEndPoint(IPAddress.Broadcast, 7070);

                _udpClient.Send(bytes,bytes.Length, _ipEndPoint);
                StartListening();
            }
        }
    }
}
