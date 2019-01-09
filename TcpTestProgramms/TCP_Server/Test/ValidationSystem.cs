using System;
using System.Collections.Generic;
using TCP_Server.Enum;

namespace TCP_Server.Test
{
    public class ValidationSystem
    {
        private ServerInfo _serverInfo;
        private bool isRunning;

        public ValidationSystem(ServerInfo serverInfo)
        {
            _serverInfo = serverInfo;
        }

        public void Start()
        {
            isRunning = true;

            while (isRunning)
            {
                switch (Server.status)
                {
                    case ValidationEnum.WaitingForPlayer:
                        //Something
                        break;
                    case ValidationEnum.ValidationState:

                        break;
                    case ValidationEnum.LobbyCheck:
                        LobbyCheck();
                        break;
                    case ValidationEnum.DeclineState:
                        //Something
                        break;
                    default:
                        break;
                }
            }
        }

        public ClientConnectionStatus _ConnectionStatus = ClientConnectionStatus.Pending;

        private void LobbyCheck()
        {
            if (_serverInfo.lobbylist[0].IsLobbyComplete())
            {
                _ConnectionStatus = ClientConnectionStatus.Declined;
            }
            else
            {
                _ConnectionStatus = ClientConnectionStatus.Accepted;
            }
            new ClientConnection(_ConnectionStatus,_serverInfo);
            Server.status = ValidationEnum.WaitingForPlayer;

        }

    }
}
