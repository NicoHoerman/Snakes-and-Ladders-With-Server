    using System;
using System.Collections.Generic;
using System.Text;
using EelsAndEscalators.Contracts;

namespace EelsAndEscalators.ClassicEandE
{

    public class ClassicEscalator : IEntity
    {
        public int top_location { get; set; }
        public int bottom_location { get; set; }

        public EntityType type => EntityType.Escalator;

        public long Id { get; set; }

        public ClassicEscalator()
        {

        }

        public void SetPawn(IPawn pawn)
        {
          pawn.location = top_location;
        }

        public bool OnSamePositionAs(IPawn pawn)
        {
          return bottom_location == pawn.location ? true : false;
        }
    }
}
