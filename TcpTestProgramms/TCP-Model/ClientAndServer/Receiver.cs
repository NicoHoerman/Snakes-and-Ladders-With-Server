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
            udp = new UdpClient(7070);
        }

        public void StartListening()
        {
            udp.BeginReceive(Receive, new object());
        }

        public void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 7070);
            byte[] bytes = udp.EndReceive(ar, ref ip);


            string message = Encoding.ASCII.GetString(bytes);
            Console.WriteLine(message);
            //get Package 
            StartListening();
        }
    }
}
