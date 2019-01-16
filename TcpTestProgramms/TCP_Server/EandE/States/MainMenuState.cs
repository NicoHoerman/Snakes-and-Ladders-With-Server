using EandE_ServerModel.EandE.ClassicEandE;
using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.StuffFromEandE;
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
        private readonly ISourceWrapper _sourceWrapper;
        private readonly DataProvider _dataProvider;

        private bool _inMenu;

        private string _mainMenuOutput = string.Empty;
        public string Input { get; set; } = string.Empty;
        

        #region Properties
        public string MainMenuOuput { get; set; } = string.Empty;
        public string Lastinput { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        public string GameInfoOuptput { get; set; } = string.Empty;
        public string BoardOutput { get; set; } = string.Empty;
        public string TurnInfoOutput { get; set; } = string.Empty;
        public string AfterTurnOutput { get; set; } = string.Empty;
        public string HelpOutput { get; set; } = string.Empty;
        public string FinishInfo { get; set; } = string.Empty;
        public string Finishskull1 { get; set; } = string.Empty;
        public string Finishskull2 { get; set; } = string.Empty;
        public int CurrentPlayer { get; set; }
        #endregion

        private Dictionary<string, Func<IGame,IConfigurationProvider, IRules>> _rulesFactory = new Dictionary<string, Func<IGame, IConfigurationProvider, IRules>>
        {
            { "classic", (game,configP) => new ClassicRules(game,configP) },
        //    { "fancy", (g) => new FancyRules(g) },
        };
        private string _rulesname;
        public static ManualResetEvent StateFinished = new ManualResetEvent(false);


        public MainMenuState(
            IGame game, 
            IConfigurationProvider configurationProvider, 
            ISourceWrapper sourceWrapper,
            DataProvider dataProvider)
        {
            _game = game;
            _configurationProvider = configurationProvider;
            _sourceWrapper = sourceWrapper;
            _dataProvider = dataProvider;
            Input = string.Empty;
            _inMenu = true;
        }

        public MainMenuState(IGame game)
            : this(game, new ConfigurationProvider(), new SourceWrapper(), new DataProvider())

        { }

        public void Execute()
        {
            while (_inMenu)
            {

            }
        }

        public void OnCloseGameCommand()
        {
            _inMenu = false;
            _game.SwitchState(new GameEndingState(_game));
        }

        public void OnClassicCommand()
        {
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

        public void SetInput(string input)
        {
            Input = input;
        }

	}
}
