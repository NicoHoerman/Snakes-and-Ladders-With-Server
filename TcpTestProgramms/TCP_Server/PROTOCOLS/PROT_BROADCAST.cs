using Shared.Contracts;

namespace TCP_Server.PROTOCOLS
{
    public class PROT_BROADCAST : IProtocol
    {
        public string _Server_name;
        public string _Server_ip;
        public int _CurrentPlayerCount;
        public int _MaxPlayerCount;
    }
}
