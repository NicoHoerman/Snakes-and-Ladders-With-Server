using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.States
{
    public class GameStartingState : IState
    {
        private readonly IGame _game;
        #region Properties
        public string MainMenuOuput { get; set; } 
        public string Lastinput { get; set; }
        public string Error { get; set; }
        public string GameInfoOuptput { get; set; }
        public string BoardOutput { get; set; }
        public string AfterBoardOutput { get; set; }
        public string AfterTurnOutput { get; set; }
        public string HelpOutput { get; set; }
        public string Finishinfo { get; set; }
        public string Finishskull1 { get; set; }
        public string Finishskull2 { get; set; }
        public string Input { get; set; }
        #endregion
        public GameStartingState(IGame game) 
        {
            _game = game;
        }

        public void Execute()
        {
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
