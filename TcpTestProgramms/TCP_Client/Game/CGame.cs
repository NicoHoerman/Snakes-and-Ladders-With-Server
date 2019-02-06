using System;
using System.Linq;
using TCP_Client.GameStuff.ClassicEandE;
using TCP_Client.GameStuff.EandEContracts;
using TCP_Client.GameStuff.XML_Config;
using Wrapper.Contracts;

namespace TCP_Client.GameStuff
{
	public class Game : IGame
	{
		public int Yourpawn { get; set; }
		public int LastPlayer { get; set; }
		public string Turnstate { get; set; }
		public int CurrentPlayerID { get; set; } = 1;

		public int _numberOfPlayers;

		public IBoard Board { get; set; }
		public IRules Rules { get; private set; }

		private IConfigurationProvider _configurationProvider;

		private ClientDataProvider _clientDataProvider;

		private IUpdateOutputView _gameInfoOutputView;
		private IUpdateOutputView _turnInfoOutputView;
		private IUpdateOutputView _boardOutputView;
		private IUpdateOutputView _afterTurnOutputView;

		public Game()
		{
			_configurationProvider = new ConfigurationProvider();
			_clientDataProvider = new ClientDataProvider();
		}

		#region Setters
		public void SetTurnState(string input)
		{
			if (!(input == null || input.Length == 0))
				Turnstate = input;
		}

		public void SetLastPlayer(int input)
		{
			if (!(input == 0))
				LastPlayer = input;
		}

		public void SetYourpawn(int input)
		{
			if (!(input == 0))
				Yourpawn = input;
		}

		public void SetViews(IUpdateOutputView gameInfoOutputView, IUpdateOutputView turnInfoOutputView,
			IUpdateOutputView boardOutputView, IUpdateOutputView afterTurnOutputView)

		{
			_afterTurnOutputView = afterTurnOutputView;
			_boardOutputView = boardOutputView;
			_gameInfoOutputView = gameInfoOutputView;
			_turnInfoOutputView = turnInfoOutputView;
		}
		#endregion

		public void CreateRules() => Rules = new ClassicRules(this, _configurationProvider);

		private void NextPlayer()
		{
			var orderedPlayers = Board.Pawns.OrderBy(x => x.PlayerID).ToList();

			if (_numberOfPlayers == 0)
				_numberOfPlayers = orderedPlayers[orderedPlayers.Count - 1].PlayerID;

			var nextPlayer = orderedPlayers.Where(x => x.PlayerID == CurrentPlayerID + 1).FirstOrDefault();
			if (nextPlayer == null)
				CurrentPlayerID = orderedPlayers.First().PlayerID;
			else
				CurrentPlayerID = nextPlayer.PlayerID;
		}

		public void MakeTurn()
		{
			switch (Turnstate)
			{
				case "TurnFinished":
					UpdateLocations();

					_boardOutputView.SetUpdateContent(Board.CreateOutput());
					_turnInfoOutputView.SetUpdateContent(string.Format(_clientDataProvider.GetText("afterboardinfo"),
						_clientDataProvider.GetNumberLiteral(CurrentPlayerID)));

					_afterTurnOutputView.SetUpdateContent(string.Format(
						_clientDataProvider.GetText("diceresultinfo"),
							_clientDataProvider.GetNumberLiteral(LastPlayer)));

					_gameInfoOutputView.SetUpdateContent("gameinfo");
					//Views mit DataProvider und Properties füllen
					break;
				case "PlayerExceedsBoard":

					_afterTurnOutputView.SetUpdateContent(string.Format(
						_clientDataProvider.GetText("diceresultinfo"),
							_clientDataProvider.GetNumberLiteral(LastPlayer)) 
							+ "\n"
								 + string.Format(
									_clientDataProvider.GetText("playerexceedsboardinfo"),
										_clientDataProvider.GetNumberLiteral(LastPlayer)));

					//Views mit DataProvider und Properties füllen
					break;
				case "GameFinished":
					//Views mit DataProvider und Properties füllen
					break;
			}
			NextPlayer();
		}

		private void UpdateLocations()
		{
			var _currentPawn = Board.Pawns.Find(x => x.PlayerID.Equals(CurrentPlayerID));
			if (Yourpawn == CurrentPlayerID)
			{
				_currentPawn.MovePawn(Rules.DiceResult);
				Board.Entities.ForEach(entity =>
				{
					if (entity.OnSamePositionAs(_currentPawn))
					{
						entity.SetPawn(_currentPawn);
					}
				});
			}
			else
				_currentPawn.Location = Rules.DiceResult;
		}

		public void DisableViews()
		{
			//_errorView.ViewEnabled = false;
			_gameInfoOutputView.ViewEnabled = false;
			_boardOutputView.ViewEnabled = false;
			_afterTurnOutputView.ViewEnabled = false;
			_turnInfoOutputView.ViewEnabled = false;
		}
	}
}   
