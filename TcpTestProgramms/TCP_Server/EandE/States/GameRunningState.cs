using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.GameAndLogic;
using EandE_ServerModel.EandE.StuffFromEandE;
using System;
using System.Threading;
using TCP_Server.Actions;

namespace EandE_ServerModel.EandE.States
{
	public class GameRunningState : IState
	{
		private readonly IGame _game;
		private readonly ISourceWrapper _sourceWrapper;
		private readonly DataProvider _dataProvider;
		private readonly Logic _logic;
		static public bool _isRunning;

		#region Properties
		public int LastPlayer { get; set; }
		#endregion

		public GameRunningState(IGame game, ISourceWrapper sourceWrapper, DataProvider dataProvider, Logic logic)
		{
			_game = game;
			_sourceWrapper = sourceWrapper;
			_dataProvider = dataProvider;
			_logic = logic;
			_isRunning = true;
		}

		public GameRunningState(IGame game)
			: this(game, new SourceWrapper(), new DataProvider(), new Logic(game))
		{ }

		public void Execute()
		{
			while (_isRunning) { }
		}

		public void OnRollDiceCommand()
		{
			var turnstate = _logic.MakeTurn();
			if (turnstate == TurnState.GameFinished)
			{
				_isRunning = false;
				_game.SwitchState(new GameFinishedState(_game));
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

				default:
					break;
			}
		}

		public void OnCloseGameCommand()
		{
			_isRunning = false;
			_game.SwitchState(new GameEndingState(_game));
		}
	}
}
