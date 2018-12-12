using Shared.Contract;
using System;
using System.Collections.Generic;
using System.Net;

namespace TCP_Server
{
    public class ServerInfo
    {
        public int _MaxPlayerCount { get; set; }
        public int _CurrentPlayerCount { get; set; } = 1;
        public string _LobbyName { get;  set; }
        public int _ServerPort { get; set; }
        public List<ICommunication> _communications { get; set; }

        private List<IPAddress> _Blacklist = new List<IPAddress>();

        public ServerInfo(string lobbyname,int maxplayercount,int port)
        {
            _MaxPlayerCount = maxplayercount;
            _LobbyName = lobbyname;
            _ServerPort = port;
        }

        public void PrintPlayerIP()
        {
            for (int i = 0; i < _communications.Count; i++)
            {
                Console.WriteLine(((IPEndPoint)_communications[i]._client.Client.RemoteEndPoint).Address.ToString());
            }
        }

        public IPAddress GetPlayerIP(int communicationNr)
        {
            return ((IPEndPoint)_communications[communicationNr]._client.Client.RemoteEndPoint).Address;
        }
    }
}
