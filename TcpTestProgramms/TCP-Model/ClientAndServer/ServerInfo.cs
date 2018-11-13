using System;
using System.Collections.Generic;
using System.Text;
using TCP_Model.Contracts;
using System.Net;

namespace TCP_Model.ClientAndServer
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
            Console.WriteLine(((IPEndPoint)_communications[0]._client.Client.RemoteEndPoint).Address.ToString());
        }
    }
}
