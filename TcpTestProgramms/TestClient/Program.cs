using System;
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

            /* IPAddress leonsadress = IPAddress.Parse("172.22.22.184");
             IPAddress myadress = IPAddress.Parse("172.22.22.153");
            */

            Udp_Client client = new Udp_Client();


            Console.WriteLine("Hallo ");
            Console.WriteLine("Suche nach Servern");


            client.PROT_JOIN();

            
            Console.ReadLine();
          

        }
    }
}
