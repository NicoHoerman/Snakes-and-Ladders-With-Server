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
        public string TurnInfoOutput { get; set; } = string.Empty;
        public string AfterTurnOutput { get; set; } = string.Empty;
        public string HelpOutput { get; set; } = string.Empty;
        public int CurrentPlayer { get; set; }

        public string MainMenuOuput { get; set; } = string.Empty;
        public string FinishInfo { get; set; } = string.Empty;
        public string Finishskull1 { get; set; } = string.Empty;
        public string Finishskull2 { get; set; } = string.Empty;
        public string Input { get; set; } = string.Empty;
        #endregion

        public static ManualResetEvent GameFinished = new ManualResetEvent(false);

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
            var parser = new Parse();
            parser.AddCommand("/help", OnHelpCommand);
            parser.AddCommand("/closegame", OnCloseGameCommand);
            parser.AddCommand("/rolldice", OnRollDiceCommand);

            _gameInfoOutput = _dataProvider.GetText("gameinfo");
            _afterBoardOutput = string.Format(
                _dataProvider.GetText("afterboardinfo"), 
                _dataProvider.GetNumberLiteral(_logic.CurrentPlayerID));
            _boardOutput = _game.Board.CreateOutput();

            while (_isRunning)
            {
                SaveProperties(_lastInput,_error,_gameInfoOutput,_boardOutput,
                    _helpOutput,_afterTurnOutput,_afterBoardOutput,_logic.CurrentPlayerID);
                
                while (Input.Length == 0) 
                {
                }
                parser.Execute(Input);

                _boardOutput = _game.Board.CreateOutput();
                SaveProperties(_lastInput, _error, _gameInfoOutput, _boardOutput, _helpOutput, _afterTurnOutput, _afterBoardOutput, _logic.CurrentPlayerID);

                _afterBoardOutput = string.Format(
                _dataProvider.GetText("afterboardinfo"),
                _dataProvider.GetNumberLiteral(_logic.CurrentPlayerID));


                _lastInput = Input;
                Input = string.Empty;
                if (_lastInput == "/rolldice")
                    ServerActions.TurnFinished.Set();

            }
        }

        private void OnRollDiceCommand()
        {
            var turnstate = _logic.MakeTurn();
            ActOnTurnState(turnstate);   
        }

        private void OnCloseGameCommand()
        {
            _isRunning = false;
                
            _game.SwitchState(new GameEndingState(_game));
        }

        private void OnHelpCommand()
        {
            _helpOutput = "Commands are" + "\n" + "/closegame" + "\n" + "/rolldice";
        }

        //Undertakes diffrent Actions, depending on the TurnState returned by MakeTurn()
        public void ActOnTurnState(TurnState turnstate)
        {
            var dataprovider = new DataProvider();

            int lastPlayer = _logic.CurrentPlayerID -1;
            if (_logic.CurrentPlayerID == 1)
                lastPlayer =_logic._numberOfPlayers;

            if (turnstate == TurnState.GameFinished)
            {
                _isRunning = false;
                _boardOutput = _game.Board.CreateOutput();

                _afterBoardOutput = string.Empty;
                SaveProperties(_lastInput, _error, _gameInfoOutput, _boardOutput, _helpOutput, _afterTurnOutput, _afterBoardOutput, _logic.CurrentPlayerID);

                ServerActions.TurnFinished.Set();

                GameFinished.WaitOne();
                GameFinished.Reset();
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
            TurnInfoOutput = string.Empty;
            AfterTurnOutput = string.Empty;
            HelpOutput = string.Empty;
        }

        private void SaveProperties(string _lastInput, string _error, string _gameInfoOutput, string _boardOutput, string _helpOutput, string _afterTurnOutput, string _afterBoardOutput,int _currentPlayer)
        {
            Lastinput = _lastInput;
            Error = _error;
            GameInfoOuptput = _gameInfoOutput;
            BoardOutput = _boardOutput;
            TurnInfoOutput = _afterBoardOutput;
            AfterTurnOutput = _afterTurnOutput;
            HelpOutput = _helpOutput;
            CurrentPlayer = _currentPlayer;

        }

        public void SetInput(string input)
        {
            Input = input;
        }
    }
}
