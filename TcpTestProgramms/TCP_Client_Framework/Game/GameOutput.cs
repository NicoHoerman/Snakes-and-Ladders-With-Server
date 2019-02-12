using System;
using System.Collections.Generic;
using System.Linq;
using TCP_Client.GameStuff.ClassicEandE;
using TCP_Client.GameStuff.EandEContracts;
using TCP_Client.GameStuff.XML_Config;
using Wrapper;
using Wrapper.Contracts;

namespace TCP_Client.GameStuff
{
	public class GameOutput : IGame
	{
		public int LastPlayer { get; set; }
		public string Turnstate { get; set; }
		public int CurrentPlayerID { get; set; } 

		public int _numberOfPlayers;

		public IBoard Board { get; set; }
		public IRules Rules { get; private set; }
		public int Pawn2 { get; private set; }
		public int Pawn1 { get; private set; }

		private IConfigurationProvider _configurationProvider;

		private ClientDataProvider _clientDataProvider;
		
		#region ahhhhhhhhhhhhhhhhhhh3
		private IUpdateOutputView _gameInfoOutputView;
		private IUpdateOutputView _turnInfoOutputView;
		private IUpdateOutputView _boardOutputView;
		private IUpdateOutputView _afterTurnOutputView;
		private IUpdateOutputView _finishOutputView;
		private IUpdateOutputView _finishskull1;
		private IUpdateOutputView _finishskull2;
		private IUpdateOutputView _finishskull3;
		#endregion

		public GameOutput()
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
		public void SetCurrentPlayer(int input)
		{
			CurrentPlayerID = 1;
			if (!(input == 0))
				CurrentPlayerID = input;
		}
		public void SetPawnLocation(int pawn1,int pawn2)
		{
			if (!(pawn1 == 0))
				Pawn1 = pawn1;
			if (!(pawn2 == 0))
				Pawn2 = pawn2;
		}

		public void SetViews(Dictionary<ClientView, IView> _views)
		{
			_afterTurnOutputView = _afterTurnOutputView = _views[ClientView.AfterTurnOutput] as IUpdateOutputView;
			_boardOutputView = _views[ClientView.Board] as IUpdateOutputView;
			_gameInfoOutputView = _views[ClientView.GameInfo] as IUpdateOutputView;
			_turnInfoOutputView = _turnInfoOutputView = _views[ClientView.TurnInfo] as IUpdateOutputView;
			_finishOutputView = _views[ClientView.FinishInfo] as IUpdateOutputView;
			_finishskull1 = _views[ClientView.FinishSkull1] as IUpdateOutputView;
			_finishskull2 = _views[ClientView.FinishSkull3] as IUpdateOutputView;
			_finishskull3 = _views[ClientView.FinishSkull2] as IUpdateOutputView;
		}
		#endregion

		public void CreateRules() => Rules = new ClassicRules(this, _configurationProvider);

		public void MakeBoardView()
		{
			_boardOutputView.SetUpdateContent(Board.CreateOutput());
			_turnInfoOutputView.SetUpdateContent(string.Format(_clientDataProvider.GetText("RollTheDice"),
				_clientDataProvider.GetNumberLiteral(CurrentPlayerID)));
			_gameInfoOutputView.SetUpdateContent(_clientDataProvider.GetText("gameinfo"));
		}

		public void UpdateGameOutput()
		{
			switch (Turnstate)
			{
				case "TurnFinished":
					UpdateLocations();

					_boardOutputView.SetUpdateContent(Board.CreateOutput());
					_turnInfoOutputView.SetUpdateContent(string.Format(_clientDataProvider.GetText("RollTheDice"),
						_clientDataProvider.GetNumberLiteral(CurrentPlayerID)));

					_afterTurnOutputView.SetUpdateContent(string.Format(
							_clientDataProvider.GetText("diceresultinfo"),
								Rules.DiceResult,_clientDataProvider.GetNumberLiteral(LastPlayer)));
					_gameInfoOutputView.SetUpdateContent(_clientDataProvider.GetText("gameinfo"));
					break;
				case "PlayerExceedsBoard":
					_turnInfoOutputView.SetUpdateContent(string.Format(_clientDataProvider.GetText("RollTheDice"),
						_clientDataProvider.GetNumberLiteral(CurrentPlayerID)));

					_afterTurnOutputView.SetUpdateContent(string.Format(
						_clientDataProvider.GetText("diceresultinfo"), 
							Rules.DiceResult,_clientDataProvider.GetNumberLiteral(LastPlayer)) 
							+ "\n"
								 + string.Format(
									_clientDataProvider.GetText("playerexceedsboardinfo"),
										_clientDataProvider.GetNumberLiteral(LastPlayer)));
					break;
				case "GameFinished":
					UpdateLocations();

					_boardOutputView.SetUpdateContent(Board.CreateOutput());
					_turnInfoOutputView.SetUpdateContent(string.Format(_clientDataProvider.GetText("RollTheDice"),
						_clientDataProvider.GetNumberLiteral(CurrentPlayerID)));

					_afterTurnOutputView.SetUpdateContent(string.Format(
						_clientDataProvider.GetText("diceresultinfo"),
							Rules.DiceResult, _clientDataProvider.GetNumberLiteral(LastPlayer)));

					_gameInfoOutputView.SetUpdateContent(_clientDataProvider.GetText("gameinfo"));

					_finishOutputView.SetUpdateContent(string.Format(_clientDataProvider.GetText("playerwins"),
						_clientDataProvider.GetNumberLiteral(LastPlayer)));
					_finishskull1.SetUpdateContent(_clientDataProvider.GetText("finishskull1"));
					_finishskull2.SetUpdateContent(_clientDataProvider.GetText("finishskull1"));
					_finishskull3.SetUpdateContent(_clientDataProvider.GetText("finishskull2"));
					break;
			}
		}

		private void UpdateLocations()
		{
			var _Pawn1 = Board.Pawns.Find(x => x.PlayerID.Equals(1));
			var _Pawn2 = Board.Pawns.Find(x => x.PlayerID.Equals(2));
			_Pawn1.Location = Pawn1;
			_Pawn2.Location = Pawn2;
		}
	}
}   
