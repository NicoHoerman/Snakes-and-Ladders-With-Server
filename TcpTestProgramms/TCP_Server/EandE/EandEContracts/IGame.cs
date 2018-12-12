
namespace EandE_ServerModel.EandE.EandEContracts
{
    public interface IGame
    {

        bool isRunning { get; set; }

        IRules Rules { get; }
        IBoard Board { get; set; }
        IState State { get; }

        void Init();
        void InitializeGame();
        void SwitchRules(IRules creator);
        void SwitchState(IState newState);
        void ClosingGame();
    }
}
