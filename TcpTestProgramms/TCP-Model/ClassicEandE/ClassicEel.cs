using System;
using System.Collections.Generic;
using System.Text;
using TCP_Model.EandEContracts;

namespace TCP_Model.ClassicEandE
{
    public class ClassicEel : IEntity
    {
        public int top_location { get ;set; }
        public int bottom_location { get; set; }
        public EntityType type => EntityType.Eel;

        public long Id { get; set; }

        public void SetPawn(IPawn pawn)
        {
            pawn.location = bottom_location;
        }

        public bool OnSamePositionAs(IPawn pawn)
        {
            return top_location == pawn.location ? true : false;
        }
    }
}
