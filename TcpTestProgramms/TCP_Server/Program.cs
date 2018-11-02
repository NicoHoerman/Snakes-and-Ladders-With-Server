using TCP_Model;

namespace TCP_Server
{
    class Program
    {
    
        static void Main(string[] args)
        {
            var server = new Server();
            server.Run();
        }
    }
}
