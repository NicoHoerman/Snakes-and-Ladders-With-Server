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

            /*IPAddress leonsadress = IPAddress.Parse("172.22.22.184");
            IPAddress myadress = IPAddress.Parse("172.22.22.153");

            IPEndPoint endPoint = new IPEndPoint(myadress, 8080);


            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(endPoint);

            byte[] content = Encoding.ASCII.GetBytes("Hello World");

            socket.Send(content);
            */

            Console.WriteLine("Ich bin ein Client");



            TcpClient client = new TcpClient("127.0.0.1", 8080);
           // client.Connect("127.0.0.1", 8080);



            string textToSend = "XD sent at " + DateTime.Now.ToString();
            string moreTextToSend = "Mehr Text ahhhhhhhhhhhhhhahhahahahhahah sent at " + DateTime.Now.ToString();

            NetworkStream nwStream = client.GetStream();
            
            //1
            byte[] bytesToSend = Encoding.ASCII.GetBytes(textToSend);
            Console.WriteLine("Sending : " + textToSend);
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);

            //2
            byte[] bytesToSend2 = Encoding.ASCII.GetBytes(moreTextToSend);
            Console.WriteLine("Sending : " + moreTextToSend);
            nwStream.Write(bytesToSend2, 0, bytesToSend2.Length);

            //1
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));

            //2
            byte[] bytesToRead2 = new byte[client.ReceiveBufferSize];
            int bytesRead2 = nwStream.Read(bytesToRead2, 0, client.ReceiveBufferSize);
            Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesToRead2, 0, bytesRead2));




            Console.ReadLine();
            client.Close();

        }
    }
}
