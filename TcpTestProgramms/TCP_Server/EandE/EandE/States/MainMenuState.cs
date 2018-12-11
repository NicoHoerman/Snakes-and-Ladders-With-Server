using EandE_ServerModel.EandE.ClassicEandE;
using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.StuffFromEandE;
using EandE_ServerModel.EandE.XML_Config;
using System;
using System.Collections.Generic;

namespace EandE_ServerModel.EandE.States
{

    public class MainMenuState : IState
    {
        private readonly IGame _game;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISourceWrapper _sourceWrapper;
        private readonly DataProvider _dataProvider;

        private bool inMenu;

        private string _mainMenuOutput = string.Empty;
        public string Input { get; set; } = string.Empty;
        

        #region Properties
        public string MainMenuOuput { get; set; } = string.Empty;
        public string Lastinput { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        public string GameInfoOuptput { get; set; } = string.Empty;
        public string BoardOutput { get; set; } = string.Empty;
        public string AfterBoardOutput { get; set; } = string.Empty;
        public string AfterTurnOutput { get; set; } = string.Empty;
        public string HelpOutput { get; set; } = string.Empty;
        public string Finishinfo { get; set; } = string.Empty;
        public string Finishskull1 { get; set; } = string.Empty;
        public string Finishskull2 { get; set; } = string.Empty;
        #endregion

        private Dictionary<string, Func<IGame,IConfigurationProvider, IRules>> _rulesFactory = new Dictionary<string, Func<IGame, IConfigurationProvider, IRules>>
        {
            { "classic", (game,configP) => new ClassicRules(game,configP) },
        //    { "fancy", (g) => new FancyRules(g) },
        };
        private string rulesname;


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
            inMenu = true;
        }
     

        public MainMenuState(IGame game)
            : this(game, new ConfigurationProvider(), new SourceWrapper(), new DataProvider())

        { }

        public void Execute()
        {
            
            while (inMenu)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                var parser = new Parse();
                parser.AddCommand("/closegame", OnCloseGameCommand);
                parser.AddCommand("/classic", OnClassicCommand);

                _mainMenuOutput = _dataProvider.GetText("mainmenuinfo");

                while (Input == string.Empty)
                {

                }

                rulesname = Input;
                parser.Execute(Input);
                Input = string.Empty;
                inMenu = false;
                _game.SwitchState(new GameStartingState(_game));
            }
        }

        private void SaveProperties(string lastInput, string error, string mainmenuInfo)
        {
            Lastinput = lastInput;
            Error = error;
            MainMenuOuput = mainmenuInfo;
        }

        public void ClearProperties()
        {
            Lastinput = string.Empty;
            Error = string.Empty;
            MainMenuOuput = string.Empty;
        }

        private void OnCloseGameCommand()
        {
            inMenu = false;
            _game.SwitchState(new GameEndingState(_game));
        }

        private void OnClassicCommand()
        {
            CreateNewRulesInGame(rulesname);
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
