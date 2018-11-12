using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP_Model.ClientAndServer
{
    public class Receiver
    {
        private UdpClient udp;

        public Receiver()
        {
            udp = new UdpClient(8080);
        }

        public void StartListening()
        {
            udp.BeginReceive(Receive, new object());
        }

        private void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 8080);
            byte[] bytes = udp.EndReceive(ar, ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            //get Package 
            StartListening();
        }
    }
}
