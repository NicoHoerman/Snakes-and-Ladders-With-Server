using Shared.Contracts;

namespace TCP_Server.PROTOCOLS
{
    public class PROT_UPDATE : IProtocol
    {       
        public string _infoOutput;
		public string _lobbyDisplay;
		public string _commandList;
		public int _diceResult;
		public int _lastPlayer;
		public string _turnstate;
    }
}
