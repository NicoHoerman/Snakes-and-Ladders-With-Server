using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace TCP_Client.DTO
{
    public class BroadcastDTO : IProtocol
    {
        public string _Server_name;
        public string _Server_ip;
        public int _CurrentPlayerCount;
        public int _MaxPlayerCount;
    }
}
