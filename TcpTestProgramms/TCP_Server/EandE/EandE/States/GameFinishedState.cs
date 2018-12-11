using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.StuffFromEandE;
using System;

namespace EandE_ServerModel.EandE.States
{
    public class GameFinishedState : IState
    {
        private readonly IGame _game;
        private readonly ISourceWrapper _sourceWrapper;
        private readonly DataProvider _dataProvider;
        

        public bool isFinished;
        public string _finishinfo =string.Empty;
        private string _finishskull1 = string.Empty;
        private string _finishskull2 = string.Empty;
        public int _winner;

        #region Properties
        public string Finishinfo { get; set; } = string.Empty;
        public string Finishskull1 { get; set; } = string.Empty;
        public string Finishskull2 { get; set; } = string.Empty;
       
        public string MainMenuOuput { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string AdditionalInformation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Lastinput { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Error { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string GameInfoOuptput { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string BoardOutput { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string AfterBoardOutput { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string AfterTurnOutput { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string HelpOutput { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Input { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion

        public GameFinishedState(IGame game, ISourceWrapper sourceWrapper, DataProvider dataProvider, int winner)
        {
            _game = game;
            _sourceWrapper = sourceWrapper;
            _dataProvider = dataProvider;        
            isFinished = true;
            _winner = winner;
        }

        public GameFinishedState(IGame game,int winner)
            : this(game, new SourceWrapper(), new DataProvider(),winner)
        { }

        public void Execute()
        {
            _finishinfo = string.Format(
                _dataProvider.GetText("playerwins"),
                _dataProvider.GetNumberLiteral(_winner));
            _finishskull1 = string.Format(
                _dataProvider.GetText("finishskull1"));
            _finishskull2 = string.Format(
                _dataProvider.GetText("finishskull2"));

            while (isFinished)
            {
                SaveProperties(_finishinfo,Finishskull1,Finishskull2);
                _sourceWrapper.Clear();
                _sourceWrapper.WriteOutput(35, 0, _finishinfo, ConsoleColor.Green);
                _sourceWrapper.WriteOutput(0, 0, _finishskull1);
                _sourceWrapper.WriteOutput(35, 5, _finishskull2);
                _sourceWrapper.WriteOutput(73, 0, _finishskull1);

                _sourceWrapper.WriteOutput(35, 3, "Press any Key to leave", ConsoleColor.DarkGreen);
                Console.SetCursorPosition(57, 3);
                var input = _sourceWrapper.ReadKey();
                 if(input != null)
                {
                    isFinished = false;
                    _game.SwitchState(new MainMenuState(_game));
                }

                

            }
        }

        public void ClearProperties()
        {
            Finishinfo = string.Empty;
            Finishskull1 = string.Empty;
            Finishskull2 = string.Empty;
        }

        public void SaveProperties(string _finshinfo,string _finishskull1,string _finishskull2)
        {
            Finishinfo = _finishinfo;
            Finishskull1 = _finishskull1;
            Finishskull2 = _finishskull2;
        }

        public void SetInput(string input)
        {
            throw new NotImplementedException();
        }
    }
}
