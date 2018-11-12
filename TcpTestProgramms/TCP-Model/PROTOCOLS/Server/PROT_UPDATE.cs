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
        public string updated_board;
        public string updated_dice_information;
        public string updated_turn_information;
        
    }
}
