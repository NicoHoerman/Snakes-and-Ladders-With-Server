using TCP_Model.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using TCP_Model.EandEContracts;

namespace TCP_Model.States
{
    class GameEndingState :IState
    {
        private readonly IGame _game;

        public GameEndingState(IGame game)
        {
            _game = game;
        }

        public void Execute()
        {
            _game.ClosingGame();

        }
    }

}

