﻿using EandE_ServerModel.EandE.EandEContracts;
using System;
using System.Diagnostics;

namespace EandE_ServerModel.EandE.States
{
	public class GameFinishedState : IState
    {
        private readonly IGame _game;
        public bool _isFinished;

        #region Properties
        public int CurrentPlayer { get; set; }
		public int LastPlayer { get; set; }
		public string TurnStateProp { get; set; }
		public int Pawn1Location { get; set; }
		public int Pawn2Location { get; set; }
		#endregion

		public GameFinishedState(IGame game)
        {
            _game = game;
            _isFinished = true;
        }

		public void Execute()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (_isFinished)
            {
                if (stopwatch.ElapsedMilliseconds > 1000 * 15)
                {
                    _isFinished = false;
                    _game.SwitchState(new GameEndingState(_game));
                }
            }
        }
        public void ExecuteStateAction(string input)
		{
			Debug.WriteLine("finish State Action gecalled");
		}
	}
}
