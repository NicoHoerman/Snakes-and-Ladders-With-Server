using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.States
{
    public class GameStartingState : IState
    {
        private readonly IGame _game;
        #region Properties
        public int CurrentPlayer { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public int LastPlayer { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public string TurnStateProp { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		#endregion
		public GameStartingState(IGame game) 
        {
            _game = game;
        }

        public void Execute()
        {
            MainMenuState.StateFinished.WaitOne();
            MainMenuState.StateFinished.Reset();
            _game.InitializeGame();
            _game.SwitchState(new GameRunningState(_game));
        }

		public void ExecuteStateAction(string input)
		{
			throw new System.NotImplementedException();
		}
	}
}
