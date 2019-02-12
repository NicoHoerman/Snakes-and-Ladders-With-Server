﻿using System.Diagnostics;
using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.States
{
    class GameEndingState :IState
    {
        private readonly IGame _game;

        public GameEndingState(IGame game)
        {
            _game = game;
        }
        #region Properties
        public int CurrentPlayer { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public int LastPlayer { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public string TurnStateProp { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public int Pawn1Location { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public int Pawn2Location { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		#endregion

		public void Execute()
        {
			Debug.WriteLine("State gewechselt zu Game Ending");
            _game.ClosingGame();
        }

		public void ExecuteStateAction(string input)
		{
			throw new System.NotImplementedException();
		}
    }

}

