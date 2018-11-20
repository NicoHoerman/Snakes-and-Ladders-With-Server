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
        private UdpClient _UdpClient;
        public List<DataPackage> _DataList;
        bool _closed = false;

        public UdpClientUnit()
        {
            _UdpClient = new UdpClient(7075);
            _DataList = new List<DataPackage>();
        }

        public void StartListening()
        {
            if (_UdpClient.Client != null)
                _UdpClient.BeginReceive(Receive, new object());
        }

        public void Receive(IAsyncResult ar)
        {
            if (!_closed)
            {
                IPEndPoint _receiveEndPoint = new IPEndPoint(IPAddress.Any, 7070);
                byte[] bytes = _UdpClient.EndReceive(ar, ref _receiveEndPoint);

                //string message = Encoding.ASCII.GetString(bytes);
                //Console.WriteLine(message);

                MemoryStream localBuffer = new MemoryStream();
                localBuffer.Write(bytes, 0, bytes.Length);

                if (localBuffer.Length < 2 * sizeof(Int32))
                    return;

                localBuffer.Seek(0, SeekOrigin.Begin);
                using (var reader = new BinaryReader(localBuffer))
                {
                    var package = new DataPackage();
                    package.Size = reader.ReadInt32();
                    package.Header = (ProtocolActionEnum)reader.ReadInt32();
                    if (package.Size <= localBuffer.Length)
                    {
                        localBuffer.Position = 2 * sizeof(Int32);
                        var sizeOfPayload = package.Size - 2 * sizeof(Int32);
                        var bytesToRead = new byte[sizeOfPayload];
                        localBuffer.Read(bytesToRead, 0, sizeOfPayload);
                        package.Payload = Encoding.ASCII.GetString(bytesToRead, 0, bytesToRead.Length);

                        _DataList.Add(package);
                        Thread.Sleep(1);
                        SendRequest();
                    }
                }
            }
        }

        public void StopListening()
        {
            _closed = true;
            _UdpClient.Dispose();
            _UdpClient.Close();
        }


        public void SendRequest()
        {
            if (!_closed)
            {
                string  broadcastMessage = "is there a Server";
                var bytes = Encoding.ASCII.GetBytes(broadcastMessage);

                IPEndPoint _ipEndPoint = new IPEndPoint(IPAddress.Broadcast, 7070);

                _UdpClient.Send(bytes,bytes.Length, _ipEndPoint);
                StartListening();
            }
        }
    }
}
