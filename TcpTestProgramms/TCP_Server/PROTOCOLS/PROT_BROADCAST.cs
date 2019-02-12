using Shared.Contracts;

namespace TCP_Server.PROTOCOLS
{
    public class PROT_BROADCAST : IProtocol
    {
        public string _server_name;
        public string _server_ip;
        public int _server_Port; 
        public int _currentPlayerCount;
        public int _maxPlayerCount;
    }
}
