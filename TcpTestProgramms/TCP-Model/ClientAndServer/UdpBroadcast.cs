﻿using Newtonsoft.Json;
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
        private bool isBroadcasting;
        private byte[] _ServerInfo;

        UdpClient udpServer;

        public UdpBroadcast()
        {
            udpServer = new UdpClient(8080);
            SetBroadcastMsg();
        }

        public void Broadcast()
        {
            isBroadcasting = true;
            while (isBroadcasting)
            {
                UdpClient client = new UdpClient();
                IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 8080);
                client.Send(_ServerInfo, _ServerInfo.Length, ip);
                client.Close();
            }
        }

        public void SetBroadcastMsg()
        {

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
            dataPackage.Size = dataPackage.ToByteArray().Length;
            _ServerInfo = dataPackage.ToByteArray();
        }
    }
}



    

