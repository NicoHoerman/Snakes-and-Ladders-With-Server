using Shared.Contracts;

namespace TCP_Client.DTO
{
    public class UpdateDTO: IProtocol
    {
		public string _infoOutput;
		public string _lobbyDisplay;
		public string _commandList;
		public int _diceResult;
		public int _lastPlayer;
		public string _turnstate;
	}
}
