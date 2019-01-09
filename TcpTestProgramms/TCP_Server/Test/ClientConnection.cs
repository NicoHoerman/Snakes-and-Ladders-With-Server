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
        public TcpClient _client;
        private ServerInfo _serverInfo;

        public ClientConnection(ClientConnectionStatus clientStatus, ServerInfo serverinfo)
        {
            _serverInfo = serverinfo;
            Execute(clientStatus);
        }

        public void Execute(ClientConnectionStatus clientStatus)
        {
            if (clientStatus == ClientConnectionStatus.Accepted)
                AcceptClient();
            else if (clientStatus == ClientConnectionStatus.Declined)
                DeclineClient();
            else
                throw new Exception();
            _client = null;
        }

        private void DeclineClient()
        {
            throw new NotImplementedException();
        }

        private void AcceptClient()
        {
            throw new NotImplementedException();
        }

        private void DisconnectClient()
        {
            var currentCommunication = _serverInfo._communications.Last();
            currentCommunication.Stop();

            communicationsToRemove.Add(currentCommunication);

            RemoveFromList();
        }
	}
}