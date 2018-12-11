using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.States
{
    public class GameStartingState : IState
    {
        private readonly IGame _game;
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
            throw new System.NotImplementedException();
        }
    }
}
