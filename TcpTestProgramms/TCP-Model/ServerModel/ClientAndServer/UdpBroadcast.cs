using EandE_ServerModel.ServerModel.ProtocolActionStuff;
using EandE_ServerModel.ServerModel.PROTOCOLS.Server;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EandE_ServerModel.ServerModel.ClientAndServer
{
    public class UdpBroadcast
    {
        private bool isBroadcasting;
        private byte[] _ServerInfo;

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
                IPEndPoint ip = new IPEndPoint(IPAddress.Parse("172.22.22.153"), 7070);
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
                    _Server_ip = "172.22.22.153",
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



    

