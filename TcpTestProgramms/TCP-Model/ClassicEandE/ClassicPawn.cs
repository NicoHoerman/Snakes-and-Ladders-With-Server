using System;
using System.Collections.Generic;
using System.Text;
using TCP_Model.EandEContracts;

namespace TCP_Model.ClassicEandE
{
    
    public class ClassicPawn : IPawn
    {
        public int location { get; set; }
        public int color { get; set; }
        public int playerID { get; set; }
        public long Id { get; set; }
        public EntityType type => EntityType.Pawn;

        public ClassicPawn()
        {
        }

        public void MovePawn(int fieldsToMove)
        {
            location += fieldsToMove;
        }
    }
}
