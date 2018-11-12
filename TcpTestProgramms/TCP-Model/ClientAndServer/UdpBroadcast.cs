using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TCP_Model.PROTOCOLS.Server;

namespace TCP_Model.ClientAndServer
{
    public class UdpBroadcast
    {
        UdpClient udpServer = new UdpClient(8080);

        public void Broadcast()
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 8080);
            byte[] bytes = Encoding.ASCII.GetBytes("Foo");
            client.Send(bytes, bytes.Length, ip);
            client.Close();
        }

        DataPackage dataPackage = new DataPackage
        {
            Header = ProtocolAction.Broadcast,
            Payload = JsonConvert.SerializeObject(new PROT_BROADCAST
            {
                server_ip = "172.22.22.184",
                server_name = "Eels and Escalators Server_1",
                player_slot_info = "[0/4] Players"
            })

        };
    }
}
