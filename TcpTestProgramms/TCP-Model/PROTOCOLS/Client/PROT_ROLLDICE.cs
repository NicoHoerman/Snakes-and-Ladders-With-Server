using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Model
{
    public class PROT_ROLLDICE: IProtocol
    {
        public string Client_IP;
        public bool Its_the_clients_turn;
    }
}
