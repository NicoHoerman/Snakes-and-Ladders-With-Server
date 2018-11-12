using System;
using System.ComponentModel;
using TCP_Model;
using TCP_Model.ClientAndServer;

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
            var server = new Server();
            server.Run();
        }
    }
}
