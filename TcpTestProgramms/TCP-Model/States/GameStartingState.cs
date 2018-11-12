using System;
using System.Collections.Generic;
using System.Text;
using TCP_Model.Contracts;
using TCP_Model.EandEContracts;

namespace TCP_Model.States
{
    public class GameStartingState : IState
    {
        private readonly IGame _game;

        public GameStartingState(IGame game) 
        {
            _game = game;
        }

        public void Execute()
        {
            _game.InitializeGame();
            _game.SwitchState(new GameRunningState(_game));
        }
    }
}
