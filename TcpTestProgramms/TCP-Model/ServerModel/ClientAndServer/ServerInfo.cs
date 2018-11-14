using System;
using System.Collections.Generic;
using System.Net;
using EandE_ServerModel.ServerModel.Contracts;

namespace EandE_ServerModel.ServerModel.ClientAndServer
{
    class ServerInfo
    {
        public int _MaxPlayerCount { get; set; }
        public int _CurrentPlayerCount { get; set; } = 0;
        public string _LobbyName { get;  set; }
        public List<ICommunication> _communications { get; set; }

        public ServerInfo(string lobbyname,int maxplayercount)
        {
            _MaxPlayerCount = maxplayercount;
            _LobbyName = lobbyname;
        }

        public void PrintPlayerIP()
        {
            for (int i = 0; i < _communications.Count; i++)
            {
                Console.WriteLine(((IPEndPoint)_communications[i]._client.Client.RemoteEndPoint).Address.ToString());
            }
        }
    }
}
