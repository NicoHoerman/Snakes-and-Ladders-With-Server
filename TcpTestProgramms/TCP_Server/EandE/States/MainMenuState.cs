using EandE_ServerModel.EandE.ClassicEandE;
using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.XML_Config;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EandE_ServerModel.EandE.States
{

	public class MainMenuState : IState
    {
        private readonly IGame _game;
        private readonly IConfigurationProvider _configurationProvider;
        private bool _inMenu;
        private string _mainMenuOutput = string.Empty;
        public string Input { get; set; } = string.Empty;
        
        #region Properties
        public int CurrentPlayer { get; set; }
		public int LastPlayer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string TurnStateProp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int Pawn1Location { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int Pawn2Location { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		#endregion

		private Dictionary<string, Func<IGame,IConfigurationProvider, IRules>> _rulesFactory = new Dictionary<string, Func<IGame, IConfigurationProvider, IRules>>
        {
            { "classic", (game,configP) => new ClassicRules(game,configP) },
        //    { "fancy", (g) => new FancyRules(g) },
        };
        private string _rulesname;
        public static ManualResetEvent StateFinished = new ManualResetEvent(false);


        public MainMenuState(IGame game, IConfigurationProvider configurationProvider)
		{
            _game = game;
            _configurationProvider = configurationProvider;
            Input = string.Empty;
            _inMenu = true;
        }

		public MainMenuState(IGame game)
			: this(game, new ConfigurationProvider())
        { }

        public void Execute()
        {
			ExecuteStateAction("classic");
			while (_inMenu)
			{ }
        }

        public void OnCloseGameCommand()
        {
            _inMenu = false;
            _game.SwitchState(new GameEndingState(_game));
        }

        public void OnClassicCommand()
        {
			_rulesname = "/classic";
            CreateNewRulesInGame(_rulesname);
            _inMenu = false;
            StateFinished.Set();
            _game.SwitchState(new GameStartingState(_game));
        }
		public void ExecuteStateAction(string input)
		{
			switch (input)
			{
				case "classic":
					OnClassicCommand();
					break;
				case "close":
					OnCloseGameCommand();
					break;
				default:
					break;
			}
		}
        
        private void CreateNewRulesInGame(string rulesname)
        {
            if (_rulesFactory.TryGetValue(rulesname.Substring(1, rulesname.Length - 1), out var createdRule))
            {
                _game.SwitchRules(createdRule(_game, _configurationProvider));
            }
        }
	}
}
