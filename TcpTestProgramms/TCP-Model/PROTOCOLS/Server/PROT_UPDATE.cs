using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Model
{
    public class PROT_UPDATE : IProtocol
    {
        public string Updated_Board;
        public string Updated_DiceInformation;
        public string Updated_TurnInformation;
        public bool Game_Finished;
    }
}
