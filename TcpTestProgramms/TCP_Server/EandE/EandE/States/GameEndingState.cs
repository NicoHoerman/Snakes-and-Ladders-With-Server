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
        public string MainMenuOuput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string AdditionalInformation { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Lastinput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Error { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string GameInfoOuptput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string BoardOutput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string AfterBoardOutput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string AfterTurnOutput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string HelpOutput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Finishinfo { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Finishskull1 { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Finishskull2 { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        #endregion
        public void ClearProperties()
        {
            throw new System.NotImplementedException();
        }

        public void Execute()
        {
            _game.ClosingGame();

        }
    }

}

