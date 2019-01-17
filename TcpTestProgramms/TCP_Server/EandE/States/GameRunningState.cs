using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.GameAndLogic;
using EandE_ServerModel.EandE.StuffFromEandE;

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
		public int CurrentPlayer { get; set; }
		public string TurnStateProp { get; set; }
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

		private void OnRollDiceCommand()
		{
			var turnstate = _logic.MakeTurn();
			CurrentPlayer = _logic.CurrentPlayerID;
			LastPlayer = _logic.LastPlayer();
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
