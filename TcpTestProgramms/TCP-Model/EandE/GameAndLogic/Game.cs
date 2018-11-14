using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.States;

namespace EandE_ServerModel.EandE.GameAndLogic
{
    public class Game : IGame
    {
        public bool isRunning { get; set; }
        public IBoard Board { get; set; }
        public IRules Rules { get; private set; }
        public IState State { get; private set; }

        public void Init()
        {
            State = new MainMenuState(this);
            Run();
        }

        public void Run()
        {
            isRunning = true;
            while (isRunning)
            {
                State.Execute();
            }
        }
        
        public void InitializeGame()
        {
           Rules.SetupEntitites();
        }
        public void SwitchRules(IRules createdRule)
        {
             Rules = createdRule;     
        }       

        public void SwitchState(IState newState)
        {
            State = newState;
        }

        public void ClosingGame()
        {
            isRunning = false;
        }

    }

}   
