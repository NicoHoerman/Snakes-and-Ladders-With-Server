using Shared.Contract;
using System;
using System.Collections.Generic;
using System.Net;

namespace TCP_Server
{
    public class ServerInfo
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
