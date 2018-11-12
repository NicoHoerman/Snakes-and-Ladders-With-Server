using TCP_Model;
using TCP_Model.ClientAndServer;

namespace TCP_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Client();
            game.Run();
        }
    }
}
