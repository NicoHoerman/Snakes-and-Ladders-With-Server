using System;
using System.Net.Sockets;
using Shared.Communications;
using TCP_Server.Enum;

namespace TCP_Server.Test
{
    public class ClientConnection
    {
        //<New>		
        public TcpClient _client;

        public ClientConnection(ClientConnectionStatus clientStatus)
        {
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

        private DataPackage AcceptClient()
        {
            _serverInfo._CurrentPlayerCount++;
            _udpServer.SetBroadcastMsg(_serverInfo);

            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Accept,
                Payload = JsonConvert.SerializeObject(new PROT_ACCEPT
                {
                    _SmallUpdate = "You are connected to the Server and in the Lobby "
                })
            };
            return dataPackage;
        }

        private DataPackage DeclineClient()
        {
            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Decline,
                Payload = JsonConvert.SerializeObject(new PROT_DECLINE
                {
                    _SmallUpdate = "You got declined. Lobby is probably full"
                })
            };

            return dataPackage;
        }

        private void DisconnectClient()
        {
            var currentCommunication = _serverInfo._communications.Last();
            currentCommunication.Stop();

            communicationsToRemove.Add(currentCommunication);

            RemoveFromList();
        }
        private DataPackage SendUpdateToAll()
        {
            var lobbyUpdatePackage = new DataPackage
            {
                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _lobbyDisplay = $"Current Lobby: {servername}. Players [{currentplayer}/{maxplayer}]",
                    _commandList = "Commands:\n/search (only available when not connected to a server)\n/startgame\n/closegame\n/rolldice\n/someCommand"

                })
            };
            lobbyUpdatePackage.Size = lobbyUpdatePackage.ToByteArray().Length;

            var updatePackage = new DataPackage
			{
				Header = ProtocolActionEnum.UpdateView,
				Payload = JsonConvert.SerializeObject(new PROT_UPDATE
				{
					_infoOutput = "A player got declined"
				})
			};
			updatePackage.Size = updatePackage.ToByteArray().Length;
		}	
	}
}