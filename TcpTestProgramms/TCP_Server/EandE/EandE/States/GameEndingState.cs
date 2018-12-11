using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.States
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

