using TCP_Client.GameStuff.ClassicEandE;
using TCP_Client.GameStuff.EandEContracts;
using TCP_Client.GameStuff.XML_Config;

namespace TCP_Client.GameStuff
{
	public class Game : IGame
	{
		public int Yourpawn { get; set; }
		public int DiceResult { get; set; }
		public int LastPlayer { get; set; }
		public string Turnstate { get; set; }

		public IBoard Board { get; set; }
		public IRules Rules { get; private set; }
		private IConfigurationProvider _configurationProvider;

		public Game()
		{
			_configurationProvider = new ConfigurationProvider();
		}

		public void CreateRules()
		{
			Rules = new ClassicRules(this, _configurationProvider);
		}

		public void MakeTurn()
		{
			switch (Turnstate)
			{
				case "TurnFinished":

					break;
				case "PlayerExceedsBoard":

					break;
				case "GameFinished":

					break;
			}
		}


	}
}   
