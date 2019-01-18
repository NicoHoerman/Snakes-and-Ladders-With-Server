using EandE_ServerModel.EandE.EandEContracts;

namespace TCP_Client.Game
{
	public class Game : IGame
	{
		public IBoard Board { get; set; }
		public IRules Rules { get; private set; }
	}
}   
