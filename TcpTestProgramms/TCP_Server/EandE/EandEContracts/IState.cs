
namespace EandE_ServerModel.EandE.EandEContracts
{
    public interface IState
    {
        void Execute();
		void ExecuteStateAction(string input);
		int LastPlayer { get;set; }
	}
}
