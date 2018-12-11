using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.GameAndLogic;
using EandE_ServerModel.EandE.StuffFromEandE;
using System;

namespace EandE_ServerModel.EandE.States
{
    public class GameRunningState : IState
    {
        private readonly IGame _game;
        private readonly ISourceWrapper _sourceWrapper;
        private readonly DataProvider _dataProvider;
        private readonly Logic _logic;
        public bool isRunning;
        private string _error = string.Empty;
        private string _gameInfoOutput = string.Empty;
        private string _boardOutput = string.Empty;
        private string _helpOutput = string.Empty;
        private string _lastInput = string.Empty;
        public string _afterTurnOutput = string.Empty;
        private string _afterBoardOutput = string.Empty;

        #region Properties
        public string Lastinput { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string GameInfoOuptput { get; set; } = string.Empty;
        public string BoardOutput { get; set; } = string.Empty;
        public string AfterBoardOutput { get; set; } = string.Empty;
        public string AfterTurnOutput { get; set; } = string.Empty;
        public string HelpOutput { get; set; } = string.Empty;

        public string MainMenuOuput { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string AdditionalInformation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Finishinfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Finishskull1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Finishskull2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion

        public GameRunningState(IGame game, ISourceWrapper sourceWrapper, DataProvider dataProvider, Logic logic)
        {
            _game = game;
            _sourceWrapper = sourceWrapper;
            _dataProvider = dataProvider;
            _logic = logic;
            isRunning = true;
        }

        public GameRunningState(IGame game)
            : this(game, new SourceWrapper(), new DataProvider(), new Logic(game))
        { }


        public void Execute()
        {
            var parser = new Parse();
            parser.AddCommand("/help", OnHelpCommand);
            parser.AddCommand("/closegame", OnCloseGameCommand);
            parser.AddCommand("/rolldice", OnRollDiceCommand);
            parser.SetErrorAction(OnErrorCommand);

            _gameInfoOutput = _dataProvider.GetText("gameinfo");
            _afterBoardOutput = string.Format(
                _dataProvider.GetText("afterboardinfo"), 
                _dataProvider.GetNumberLiteral(_logic.CurrentPlayerID));

            while (isRunning)
            {
                _boardOutput = _game.Board.CreateOutput();
                UpdateOutput();
                SaveProperties(_lastInput,_error,_gameInfoOutput,_boardOutput,_helpOutput,_afterTurnOutput,_afterBoardOutput);
                _error = string.Empty;
                _helpOutput = string.Empty;
                _afterTurnOutput = string.Empty;

                _sourceWrapper.WriteOutput(0, 21, "Type an Command: ", ConsoleColor.DarkGray);
                Console.SetCursorPosition(17, 21);
                var input = _sourceWrapper.ReadInput();
                parser.Execute(input);

                _afterBoardOutput = string.Format(
                    _dataProvider.GetText("afterboardinfo"),
                    _dataProvider.GetNumberLiteral(_logic.CurrentPlayerID));

                _lastInput = input;
            }
        }

        private void OnErrorCommand(string token)
        {
            _error = "Unknown command.";
        }

        private void OnRollDiceCommand()
        {
            var turnstate = _logic.MakeTurn();
            ActOnTurnState(turnstate);   
        }

        private void OnCloseGameCommand()
        {
            isRunning = false;
            _game.SwitchState(new GameEndingState(_game));
        }

        private void OnHelpCommand()
        {
            _helpOutput = "Commands are" + "\n" + "/closegame" + "\n" + "/rolldice";
        }

        private void UpdateOutput()
        {
            _sourceWrapper.Clear();
            //Game Info
            _sourceWrapper.WriteOutput(0, 0, _gameInfoOutput, ConsoleColor.DarkCyan);
            
            //Board Display
            _sourceWrapper.WriteOutput(40, 10, _boardOutput, ConsoleColor.Gray);
            
            //After Board Info 
            _sourceWrapper.WriteOutput(0, 16, _afterBoardOutput, ConsoleColor.DarkCyan);


            //After Turn Info
            if(_afterTurnOutput.Length != 0)
            {
                _sourceWrapper.WriteOutput(0, 23, _afterTurnOutput, ConsoleColor.DarkCyan);

            }


            //Help Info 
            if (_helpOutput.Length != 0)
                _sourceWrapper.WriteOutput(30, 2, _helpOutput, ConsoleColor.Yellow);


            //Last Input and Error
            if (_error.Length != 0)
            {
                _sourceWrapper.WriteOutput(0, 18, _lastInput, ConsoleColor.DarkRed);
                _sourceWrapper.WriteOutput(0, 19, _error, ConsoleColor.Red);
            }
        }

        //Undertakes diffrent Actions, depending on the TurnState returned by MakeTurn()
        public void ActOnTurnState(TurnState turnstate)
        {
            var dataprovider = new DataProvider();

            int lastPlayer = _logic.CurrentPlayerID -1;
            if (_logic.CurrentPlayerID == 1)
                lastPlayer =_logic.numberOfPlayers;

            if (turnstate == TurnState.GameFinished)
            {
                isRunning = false;
                _game.SwitchState(new GameFinishedState(_game,lastPlayer));
            }
            else if (turnstate == TurnState.PlayerExceedsBoard)
            {
                _afterTurnOutput = string.Format(
                    dataprovider.GetText("diceresultinfo"),
                    _game.Rules.DiceResult,
                        dataprovider.GetNumberLiteral(lastPlayer))
                        +"\n"
                                 + string.Format(
                    dataprovider.GetText("playerexceedsboardinfo"), 
                    _dataProvider.GetNumberLiteral(lastPlayer));
                
                turnstate = TurnState.TurnFinished;
            }
            else if(turnstate == TurnState.TurnFinished)
            {
                _afterTurnOutput = string.Format(
                    dataprovider.GetText("diceresultinfo"),
                    _game.Rules.DiceResult,
                dataprovider.GetNumberLiteral(lastPlayer));
            }
        }

        public void ClearProperties()
        {
            Lastinput = string.Empty;
            Error = string.Empty;
            GameInfoOuptput = string.Empty;
            BoardOutput = string.Empty;
            AfterBoardOutput = string.Empty;
            AfterTurnOutput = string.Empty;
            HelpOutput = string.Empty;
        }

        private void SaveProperties(string _lastInput, string _error, string _gameInfoOutput, string _boardOutput, string _helpOutput, string _afterTurnOutput, string _afterBoardOutput)
        {
            Lastinput = _lastInput;
            Error = _error;
            GameInfoOuptput = _gameInfoOutput;
            BoardOutput = _boardOutput;
            AfterBoardOutput = _afterBoardOutput;
            AfterTurnOutput = _afterTurnOutput;
            HelpOutput = _helpOutput;

        }
    }
}
