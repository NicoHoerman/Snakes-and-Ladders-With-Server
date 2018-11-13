using System;
using System.Collections.Generic;
using System.Text;
using TCP_Model.Contracts;

namespace TCP_Model.PROTOCOLS.Server
{
    class PROT_BROADCAST : IProtocol
    {
        public string _Server_name;
        public string _Server_ip;
        public string _CurrentPlayerCount;
        public string _MaxPlayerCount;
    }
}
