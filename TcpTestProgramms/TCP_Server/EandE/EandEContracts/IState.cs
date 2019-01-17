﻿
namespace EandE_ServerModel.EandE.EandEContracts
{
    public interface IState
    {
        void Execute();
		void ExecuteStateAction(string input);
		int LastPlayer { get; set; }
		int CurrentPlayer { get; set; }
		string TurnStateProp { get; set; }
	}
}
