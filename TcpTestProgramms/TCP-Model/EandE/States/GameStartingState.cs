using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.States
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
