﻿using TCP_Client.Game.EandEContracts;

namespace TCP_Client.Game.ClassicEandE
{
	public class ClassicEel : IEntity
    {
        public int Top_location { get ;set; }
        public int Bottom_location { get; set; }
        public EntityType Type => EntityType.Eel;

        public long Id { get; set; }

        public void SetPawn(IPawn pawn)
        {
            pawn.Location = Bottom_location;
        }

        public bool OnSamePositionAs(IPawn pawn)
        {
            return Top_location == pawn.Location ? true : false;
        }
    }
}
