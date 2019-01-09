using System;
using System.Linq;
using System.Net.Sockets;
using Shared.Communications;
using TCP_Server.Enum;

namespace TCP_Server.Test
{
    public class ClientConnection
    {
        //<New>		
        private ServerInfo _serverInfo;

        public ClientConnection(ServerInfo serverinfo)
        {
            _serverInfo = serverinfo;
        }

        public void Execute()
        {
           
        }
	}
}
