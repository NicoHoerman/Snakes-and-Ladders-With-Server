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
        private DataPackageProvider _dataPackageProvider;

        public ClientConnection(ServerInfo serverinfo, DataPackageProvider dataPackageProvider)
        {
            _serverInfo = serverinfo;
            _dataPackageProvider = dataPackageProvider;
        }

        public void Execute(ICommunication communication)
        {
            _serverInfo.lobbylist[0]._CurrentPlayerCount++;
            //Send To Current
            DataPackage acceptedInfoPackage = _dataPackageProvider.GetPackage("AcceptedInfo");


            //Send To All
            DataPackage lobbyDisplayPackage = _dataPackageProvider.GetPackage("LobbyDisplay");
        }
	}
}
