using System;
using TCP_Model;

namespace TCP_Server
{
    class Program
    {
    
        static void Main(string[] args)
        {
            Console.WriteLine("Listening... ");
            var server = new Server();
            server.Run();
        }
    }
}
