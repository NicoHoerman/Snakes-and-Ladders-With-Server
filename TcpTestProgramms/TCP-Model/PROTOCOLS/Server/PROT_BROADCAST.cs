using System;
using System.Collections.Generic;
using System.Text;
using TCP_Model.Contracts;

namespace TCP_Model.PROTOCOLS.Server
{
    class PROT_BROADCAST : IProtocol
    {
        public string server_name;
        public string server_ip;
        public string player_slot_info; //maybe needs to be split up into more variables// something like [1/4] Players would be sweet though.
    }
}
