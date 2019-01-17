using Shared.Contract;
using System;
using System.Collections.Generic;
using System.Net;
using TCP_Server.Test;

namespace TCP_Server.Support
{
    public class ServerInfo
    {
        public List<ICommunication> _communications { get; set; }
        public List<ICommunication> _communicationsToRemove { get; set; }
        public List<Lobby> _lobbylist { get; set; }
        public ServerInfo()
        {
            _communications = new List<ICommunication>();
            _communicationsToRemove = new List<ICommunication>();
            _lobbylist = new List<Lobby>();
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
