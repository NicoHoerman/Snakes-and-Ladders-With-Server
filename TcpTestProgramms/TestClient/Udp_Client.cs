using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpTestProgramm
{
    class Udp_Client
    {
        IPAddress ipAddress;
        UdpClient client;
        byte[] RequestData = Encoding.ASCII.GetBytes("Hey was geht? ");
        IPEndPoint ServerEp = new IPEndPoint(IPAddress.Any, 0);

        public Udp_Client()
        {
            ipAddress = IPAddress.Parse("127.0.0.1");
            client = new UdpClient(); 
        }

        
        


        public void PROT_JOIN()
        {
            client.EnableBroadcast = true;
            client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast,8080));


            var ServerResponseData = client.Receive(ref ServerEp);
            var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
            Console.WriteLine("Recived:  {0} from : {1}", ServerResponse, ServerEp.Address.ToString());

            client.Connect(ServerEp);
            
            client.Close();
        }

    }
}
