using TCP_Model;

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
