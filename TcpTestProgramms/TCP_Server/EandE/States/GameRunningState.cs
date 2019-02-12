﻿using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.GameAndLogic;

namespace EandE_ServerModel.EandE.States
{
	public class GameRunningState : IState
	{
		private readonly IGame _game;
		private readonly Logic _logic;
		static public bool _isRunning;

		#region Properties
		public int LastPlayer { get; set; }
		public int CurrentPlayer { get; set; } = 1;
		public string TurnStateProp { get; set; }
		public int Pawn1Location { get; set; }
		public int Pawn2Location { get; set; }
		#endregion

		public GameRunningState(IGame game, Logic logic)
		{
			_game = game;
			_logic = logic;
			_isRunning = true;
		}

		public GameRunningState(IGame game)
			: this(game, new Logic(game))
		{ }

		public void Execute()
		{
			while (_isRunning) { }
		}

		private void OnRollDiceCommand()
		{
			var turnstate = _logic.MakeTurn();
			if (_logic.CurrentPlayerID == 0)
				CurrentPlayer = 1;
			CurrentPlayer = _logic.CurrentPlayerID;
			LastPlayer = _logic.LastPlayer();
			Pawn1Location = _game.Board.Pawns.Find(x => x.PlayerID == 1).Location;
			Pawn2Location = _game.Board.Pawns.Find(x => x.PlayerID == 2).Location;
			switch (turnstate)
			{
				case TurnState.TurnFinished:
					TurnStateProp = turnstate.ToString();
					break;
				case TurnState.PlayerExceedsBoard:
					TurnStateProp = turnstate.ToString();
					break;
				case TurnState.GameFinished:
					TurnStateProp = turnstate.ToString();
					break;
				default:
					break;
			}
		}

		public void ExecuteStateAction(string input)
		{
			switch (input)
			{
				case "rolldice":
					OnRollDiceCommand();
					break;

				case "close":
					OnCloseGameCommand();
					break;
				case "finish":
					FinishGame();
					break;
				default:
					break;
			}
		}

		private void OnCloseGameCommand()
		{
			_isRunning = false;
			_game.SwitchState(new GameEndingState(_game));
		}

		private void FinishGame()
		{
			_isRunning = false;
			_game.SwitchState(new GameFinishedState(_game));
		}
	}
}
