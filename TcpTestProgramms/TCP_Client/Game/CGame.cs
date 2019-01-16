using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.GameAndLogic
{
	public class Game : IGame
	{
		public IBoard Board { get; set; }
		public IRules Rules { get; private set; }
	}
}   
