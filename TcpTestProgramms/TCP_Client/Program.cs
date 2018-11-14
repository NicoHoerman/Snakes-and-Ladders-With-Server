using EandE_ServerModel.ServerModel.ClientAndServer;

namespace TCP_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            client.Run();
        }
    }
}
