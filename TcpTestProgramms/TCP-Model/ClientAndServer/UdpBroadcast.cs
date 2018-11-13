using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TCP_Model.PROTOCOLS.Server;

namespace TCP_Model.ClientAndServer
{
    public class UdpBroadcast
    {
        private bool isBroadcasting;
        private byte[] _ServerInfo;

        UdpClient udpServer;

        public UdpBroadcast()
        {
            udpServer = new UdpClient();
            SetBroadcastMsg();
        }

        public void Broadcast()
        {
            isBroadcasting = true;
            while (isBroadcasting)
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 7070);
                udpServer.Send(_ServerInfo, _ServerInfo.Length, ip);
                //udpServer.Close();
                Thread.Sleep(10000);
            }
        }

        public void SetBroadcastMsg()
        {

            DataPackage dataPackage = new DataPackage
            {
                Header = ProtocolAction.Broadcast,
                Payload = JsonConvert.SerializeObject(new PROT_BROADCAST
                {
                    _Server_ip = "172.22.22.184",
                    _Server_name = "Eels and Escalators Server_1"
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;
            _ServerInfo = dataPackage.ToByteArray();
        }
    }
}



    

