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
        private bool ruleNotSet = true;
        private bool gameNotStarted = true;

        private string _error = string.Empty;
        private string _lastInput = string.Empty;
        private string _additionalInformation = string.Empty;
        private string _mainMenuOutput = string.Empty;

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
                parser.AddCommand("/startgame", OnStartGameCommand);
                parser.AddCommand("/closegame", OnCloseGameCommand);
                parser.AddCommand("/classic", OnClassicCommand);
                parser.SetErrorAction(OnErrorCommand);

                _mainMenuOutput = _dataProvider.GetText("mainmenuinfo");

               
                while (ruleNotSet)
                {
                    UpdateOutput();
                    _error = string.Empty;

                    _sourceWrapper.WriteOutput(0, 15, "Type an Command: ", ConsoleColor.DarkGray);
                    Console.SetCursorPosition(17, 15);
                    var input = _sourceWrapper.ReadInput();

                    _lastInput = input;
                    rulesname = input;
                    parser.Execute(input);
             
                }
                while (gameNotStarted)
                {
                    UpdateOutput();
                    _error = string.Empty;

                    _sourceWrapper.WriteOutput(0, 17, "Type an Command: ", ConsoleColor.DarkGray);
                    Console.SetCursorPosition(17, 17);
                    var input = _sourceWrapper.ReadInput();

                    _lastInput = input;
                    parser.Execute(input);
                }
            }
        }

        private void OnErrorCommand(string token)
        {
            _error = "Invalid input";
            return;
        }

        private void OnStartGameCommand()
        {
            if(ruleNotSet)
                _error = "Please choose a rule first";
            else
            {
                inMenu = false;
                gameNotStarted = false;
                _game.SwitchState(new GameStartingState(_game));
            }
        }

        private void OnCloseGameCommand()
        {
            ruleNotSet = false;
            gameNotStarted = false;
            inMenu = false;
            _game.SwitchState(new GameEndingState(_game));
        }

        private void OnClassicCommand()
        {
            CreateNewRulesInGame(rulesname);
        }

        private void UpdateOutput()
        {
            _sourceWrapper.Clear();
            _sourceWrapper.WriteOutput(0,0,_mainMenuOutput, ConsoleColor.DarkCyan);
            _sourceWrapper.WriteOutput(0, 12, string.Empty);

            if (_additionalInformation.Length != 0)
                _sourceWrapper.WriteOutput(0, 15, _additionalInformation, ConsoleColor.DarkCyan);

            if (_error.Length != 0)
            {
                _sourceWrapper.WriteOutput(0, 12, "Last Input: " + _lastInput, ConsoleColor.DarkRed);
                _sourceWrapper.WriteOutput(0, 13, "Last Error: " + _error, ConsoleColor.Red);
            }
        }

        private void CreateNewRulesInGame(string rulesname)
        {
            if (_rulesFactory.TryGetValue(rulesname.Substring(1, rulesname.Length - 1), out var createdRule))
            {
                _game.SwitchRules(createdRule(_game, _configurationProvider));
                ruleNotSet = false;
                _additionalInformation = "Ruleset chosen.\nYou can now start the game.";
            }
            else
                _error = "Interal error.";
        }

    }
}
