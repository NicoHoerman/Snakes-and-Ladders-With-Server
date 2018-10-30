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


            /* IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 8080);

             Console.WriteLine("Listening...");
             Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

             socket.Bind(endPoint);
             socket.Listen(0);

             socket.Accept();

             Console.WriteLine("Jemand will sich verbinden.");

             Console.ReadLine();

             */
            const int PORT_NO = 8080;
            IPAddress SERVER_IP = IPAddress.Parse("127.0.0.1");

            TcpListener listener = new TcpListener(SERVER_IP, PORT_NO);
            Console.WriteLine("Listening...");
            listener.Start();

            TcpClient client = listener.AcceptTcpClient();
            //1
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            // startgame
            //programm.exe()
            {




            }



            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            //2
            NetworkStream nwStream2 = client.GetStream();
            byte[] buffer2 = new byte[client.ReceiveBufferSize];

            int bytesRead2 = nwStream2.Read(buffer2, 0, client.ReceiveBufferSize);

            //1
            string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received : " + dataReceived);

            Console.WriteLine("Sending back : $$ " + dataReceived);
            nwStream.Write(buffer, 0, bytesRead);

            //2
            string dataReceived2 = Encoding.ASCII.GetString(buffer2, 0, bytesRead2);
            Console.WriteLine("Received : " + dataReceived2);

            Console.WriteLine("Sending back : $$ " + dataReceived2);
            nwStream.Write(buffer2, 0, bytesRead2);


            
            client.Close();
            listener.Stop();
            Console.ReadLine();
        }
    }
}
