using EandE_ServerModel.ServerModel.ProtocolActionStuff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EandE_ServerModel.ServerModel.ClientAndServer
{
    public class Receiver
    {
        private UdpClient _Udp;
        public List<DataPackage> _DataList;
        bool _closed = false;

        public Receiver()
        {
            _Udp = new UdpClient(7070);
            _DataList = new List<DataPackage>();
        }

        public void StartListening()
        {
            if (_Udp.Client != null)
                _Udp.BeginReceive(Receive, new object());
        }

        public void Receive(IAsyncResult ar)
        {
            if (!_closed)
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, 7070);
                byte[] bytes = _Udp.EndReceive(ar, ref ip);

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
                        StartListening();
                    }
                }
            }
        }

        public void StopListening()
        {
            _closed = true;
            _Udp.Dispose();
            _Udp.Close();
        }
    }
}
