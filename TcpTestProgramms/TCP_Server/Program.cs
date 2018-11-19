using System;
using System.ComponentModel;
using TCP_Server.UDP;

namespace TCP_Server
{
    class Program
    {
    
        static void Main(string[] args)
        {
            Console.WriteLine("Broadcasting...");
            var udpserver = new UdpBroadcast();
            

            var backgroundworker = new BackgroundWorker();

            backgroundworker.DoWork += (obj, ea) => udpserver.Broadcast();
            backgroundworker.RunWorkerAsync();



            Console.WriteLine("Waiting for players ");
            var server = new Server("Test Lobby", 2);
            server.Run();
        }
    }
}
