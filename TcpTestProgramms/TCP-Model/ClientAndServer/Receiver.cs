using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP_Model.ClientAndServer
{
    public class Receiver
    {
        private UdpClient udp;
        public DataPackage data;
        public Receiver()
        {
            udp = new UdpClient(7070);
        }

        public void StartListening()
        {         
            udp.BeginReceive(Receive, new object());
        }

        public void Receive(IAsyncResult ar)
        {
            if (udp != null)
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, 7070);
                byte[] bytes = udp.EndReceive(ar, ref ip);

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
                    package.Header = (ProtocolAction)reader.ReadInt32();
                    if (package.Size <= localBuffer.Length)
                    {
                        localBuffer.Position = 2 * sizeof(Int32);
                        var sizeOfPayload = package.Size - 2 * sizeof(Int32);
                        var bytesToRead = new byte[sizeOfPayload];
                        localBuffer.Read(bytesToRead, 0, sizeOfPayload);
                        package.Payload = Encoding.ASCII.GetString(bytesToRead, 0, bytesToRead.Length);

                        data = package;
                        StartListening();
                    }
                }
            }
            
        }

        public void StopListening()
        {
            udp.Dispose();
            udp.Close();
        }
    }
}
