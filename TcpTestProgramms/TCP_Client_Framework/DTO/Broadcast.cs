using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace TCP_Client.DTO
{
    public class BroadcastDTO : IProtocol
    {
        public string _server_name;
        public string _server_ip;
        public int _server_Port;
        public int _currentPlayerCount;
        public int _maxPlayerCount;
    }
}
