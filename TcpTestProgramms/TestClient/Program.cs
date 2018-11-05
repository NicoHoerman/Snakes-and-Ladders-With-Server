using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using TcpTestProgramm;

namespace TestClient
{


    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
            game.Run();
        }
    }
}

/* IPAddress leonsadress = IPAddress.Parse("172.22.22.184");
 IPAddress myadress = IPAddress.Parse("172.22.22.153");
*/

//TCP_Client client = new TCP_Client();

//Console.WriteLine("Suche nach Servern");

