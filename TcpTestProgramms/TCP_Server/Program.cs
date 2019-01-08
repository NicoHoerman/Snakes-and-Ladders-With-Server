using System;
using System.ComponentModel;
using TCP_Server.UDP;

namespace TCP_Server
{
    class Program
    {
    
        static void Main(string[] args)
        {
            Console.WriteLine("Listening for Clients...");
            var udpserver = new UdpBroadcast();
            

            var backgroundworker = new BackgroundWorker();

            backgroundworker.DoWork += (obj, ea) => udpserver.RunUdpServer();
            backgroundworker.RunWorkerAsync();



            Console.WriteLine("Waiting for players ");
            var server = new Server(udpserver);
            server.Run();
        }
    }
}
