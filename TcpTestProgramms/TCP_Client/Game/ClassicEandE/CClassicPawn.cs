﻿using TCP_Client.Game.EandEContracts;

namespace TCP_Client.Game.ClassicEandE
{

	public class ClassicPawn : IPawn
    {
        public int Location { get; set; }
        public int Color { get; set; }
        public int PlayerID { get; set; }
        public long Id { get; set; }
        public EntityType Type => EntityType.Pawn;

        public ClassicPawn()
        {
        }

        public void MovePawn(int fieldsToMove)
        {
            Location += fieldsToMove;
        }
    }
}
