using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.States
{
    public class GameStartingState : IState
    {
        private readonly IGame _game;
        #region Properties
        public string MainMenuOuput { get; set; } = string.Empty;
        public string Lastinput { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string GameInfoOuptput { get; set; } = string.Empty;
        public string BoardOutput { get; set; } = string.Empty;
        public string AfterBoardOutput { get; set; } = string.Empty;
        public string AfterTurnOutput { get; set; } = string.Empty;
        public string HelpOutput { get; set; } = string.Empty;
        public string Finishinfo { get; set; } = string.Empty;
        public string Finishskull1 { get; set; } = string.Empty;
        public string Finishskull2 { get; set; } = string.Empty;
        public string Input { get; set; } = string.Empty;
        public int CurrentPlayer { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
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

        public void ClearProperties()
        {
        }

        public void SetInput(string input)
        {
        }
    }
}
