using System;
using System.Collections.Generic;
using System.Text;
using TCP_Model.Contracts;

namespace TCP_Model.PROTOCOLS.Server
{
    public class PROT_BROADCAST : IProtocol
    {
        public string _Server_name;
        public string _Server_ip;
        public int _CurrentPlayerCount;
        public int _MaxPlayerCount;
    }
}
