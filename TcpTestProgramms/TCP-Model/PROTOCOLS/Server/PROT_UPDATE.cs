using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCP_Model.Contracts;

namespace TCP_Model.PROTOCOLS.Server
{
    public class PROT_UPDATE : IProtocol
    {
        public string _Updated_board;
        public string _Updated_dice_information;
        public string _Updated_turn_information;
        
    }
}
