using System;
using System.ComponentModel;
using TCP_Server.UDP;

namespace TCP_Server
{
    class Program
    {
    
        static void Main(string[] args)
        {
            var serverInfo = new ServerInfo("TestLobby",2,8080);

            Console.WriteLine("Broadcasting...");
            var udpserver = new UdpBroadcast(serverInfo);
            

            var backgroundworker = new BackgroundWorker();

            backgroundworker.DoWork += (obj, ea) => udpserver.StartListening();
            backgroundworker.RunWorkerAsync();



            Console.WriteLine("Waiting for players ");
            var server = new Server(serverInfo,udpserver);
            server.Run();
        }
    }
}
