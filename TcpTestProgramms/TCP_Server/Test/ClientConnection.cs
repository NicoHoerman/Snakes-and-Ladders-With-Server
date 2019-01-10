using System;
using System.Linq;
using System.Net.Sockets;
using Shared.Communications;
using Shared.Contract;
using TCP_Server.Enum;

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
            _serverInfo.lobbylist[0]._CurrentPlayerCount++;
            DataPackage acceptedInfoPackage = _dataPackageProvider.GetPackage("AcceptedInfo");
            communication.Send(acceptedInfoPackage);

            DataPackage lobbyDisplayPackage = _dataPackageProvider.LobbyDisplay();
            for (int i = 0; i <= _serverInfo._communications.Count - 1; i++)
            {
                _serverInfo._communications[i].Send(lobbyDisplayPackage);
            }
        }
	}
}
