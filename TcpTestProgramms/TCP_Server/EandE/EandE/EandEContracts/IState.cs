
namespace EandE_ServerModel.EandE.EandEContracts
{
    public interface IState
    {
        void Execute();
        void ClearProperties();
        void SetInput(string input);

        string Input { get; set; }
        string MainMenuOuput { get; set; }
        string Lastinput { get; set; }
        string Error { get; set; }
        string GameInfoOuptput { get; set; }
        string BoardOutput { get; set; }
        string AfterBoardOutput { get; set; }
        string AfterTurnOutput { get; set; }
        string HelpOutput { get; set; }
        string Finishinfo { get; set; }
        string Finishskull1 { get; set; }
        string Finishskull2 { get; set; }
    }
}
