using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;


namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {

            IPAddress adress = IPAddress.Parse("127.0.0.1");

            IPEndPoint endPoint = new IPEndPoint(adress, 8080);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(endPoint);

            byte[] content = System.Text.Encoding.ASCII.GetBytes("Hello World");

            //socket.Send(content, SocketFlags.Broadcast);

            Console.ReadLine();

        }
    }
}
