using System;
using System.Collections.Generic;
using System.Text;
using TCP_Model.Contracts;
using TCP_Model.EandEContracts;
using TCP_Model.StuffFromEandE;

namespace TCP_Model.States
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

    }
}
