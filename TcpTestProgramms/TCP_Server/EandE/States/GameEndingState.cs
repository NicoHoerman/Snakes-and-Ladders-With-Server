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
        #region Properties
        public string MainMenuOuput { get; set; } = string.Empty;
        public string Lastinput { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string GameInfoOuptput { get; set; } = string.Empty;
        public string BoardOutput  { get; set; } = string.Empty;
        public string AfterBoardOutput { get; set; } = string.Empty;
        public string AfterTurnOutput { get; set; } = string.Empty;
        public string Finishinfo { get; set; } = string.Empty;
        public string Finishskull1 { get; set; } = string.Empty;
        public string Finishskull2 { get; set; } = string.Empty;
        public string Input { get; set; } = string.Empty;
        public string HelpOutput { get; set; } = string.Empty;
        public int CurrentPlayer { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        #endregion
        public void ClearProperties()
        {
            throw new System.NotImplementedException();
        }

        public void Execute()
        {
            _game.ClosingGame();

        }

        public void SetInput(string input)
        {
        }
    }

}

