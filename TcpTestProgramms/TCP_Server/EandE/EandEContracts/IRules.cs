﻿using System.Xml.Linq;


namespace EandE_ServerModel.EandE.EandEContracts
{

    public interface IRules
    {
        int NumberOfPawns { get;}
        int DiceSides { get; }  
        int DiceResult { get; }

        IPawn CreatePawn(XElement configuration);
        IEntity CreateEel(XElement configuration);
        IEntity CreateEscalator(XElement configuration);
        IBoard CreateBoard();

        void RollDice();
        void SetupEntitites();
    }
}
