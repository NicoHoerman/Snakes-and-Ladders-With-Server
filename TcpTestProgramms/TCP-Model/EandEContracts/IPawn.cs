using System;
using System.Collections.Generic;
using System.Text;

namespace TCP_Model.EandEContracts
{

    public interface IPawn
    {
        int location { get; set; }
        int color { get; set; }
        int playerID { get; set; }
        long Id { get; set; }
        EntityType type { get; }
        void MovePawn(int fieldsToMove);
    }
}
