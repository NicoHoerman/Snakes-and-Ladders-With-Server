using System;
using System.Linq;
using System.Net.Sockets;
using Shared.Communications;
using Shared.Contract;
using TCP_Server.DataProvider;
using TCP_Server.Enum;
using TCP_Server.Support;

namespace TCP_Server.Test
{
    public class ClientConnection
    {
        //<New>		
        private ServerInfo _serverInfo;
        private ServerDataPackageProvider _dataPackageProvider;

        public ClientConnection(ServerInfo serverinfo, ServerDataPackageProvider dataPackageProvider)
        {
            _serverInfo = serverinfo;
            _dataPackageProvider = dataPackageProvider;
        }

        public void Execute(ICommunication communication)
        {
            _serverInfo._lobbylist[0]._CurrentPlayerCount++;
            communication.Send(_dataPackageProvider.GetPackage("AcceptedInfo"));
            
            for (int i = 0; i <= _serverInfo._communications.Count - 1; i++)
                _serverInfo._communications[i].Send(_dataPackageProvider.LobbyDisplay());
        }
	}
}
