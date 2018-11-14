using EandE_ServerModel.ServerModel.Contracts;

namespace EandE_ServerModel.ServerModel.PROTOCOLS.Server
{
    public class PROT_BROADCAST : IProtocol
    {
        public string _Server_name;
        public string _Server_ip;
        public int _CurrentPlayerCount;
        public int _MaxPlayerCount;
    }
}
