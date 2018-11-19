using Newtonsoft.Json;
using Shared.Communications;
using Shared.Enums;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TCP_Server.PROTOCOLS;

namespace TCP_Server.UDP
{
    public class UdpBroadcast
    {
        private bool isBroadcasting;
        private byte[] _ServerInfo;
        private const string SERVER_IP_WLAN = "172.22.21.132";
        private const string SERVER_IP_LAN = "172.22.22.153";

        UdpClient udpServer;

        public UdpBroadcast()
        {
            udpServer = new UdpClient();
            //SetBroadcastMsg();
        }

        public void Broadcast()
        {
            isBroadcasting = true;
            while (isBroadcasting)
            {
                SetBroadcastMsg();
                IPEndPoint ip = new IPEndPoint(IPAddress.Parse(SERVER_IP_LAN), 7070);
                udpServer.Send(_ServerInfo, _ServerInfo.Length, ip);
                //udpServer.Close();
                Thread.Sleep(5000);
            }
        }

        public void SetBroadcastMsg()
        {
            Random random = new Random();
            DataPackage dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Broadcast,
                Payload = JsonConvert.SerializeObject(new PROT_BROADCAST
                {
                    _Server_ip = SERVER_IP_LAN,
                    _Server_name = "Eels and Escalators Server_1",
                    _CurrentPlayerCount = random.Next(0,4),
                    _MaxPlayerCount = 4
                    
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;
            _ServerInfo = dataPackage.ToByteArray();
        }
    }
}



    

