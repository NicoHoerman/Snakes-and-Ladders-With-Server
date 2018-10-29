using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO.Ports;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 8080);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(endPoint);
            socket.Listen(0);

            socket.Accept();

            Console.WriteLine("Jemand will sich verbinden.");

            Console.ReadLine();
        }
    }
}
