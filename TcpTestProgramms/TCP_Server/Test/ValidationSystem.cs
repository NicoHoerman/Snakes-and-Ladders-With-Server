using System;
using System.Collections.Generic;
using TCP_Server.Enum;

namespace TCP_Server.Test
{
    public class ValidationSystem
    {
        private List<Lobby> _lobbylist;
        private bool isRunning;

        public ValidationSystem(List<Lobby> lobbylist)
        {
            _lobbylist = lobbylist;
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
            if (_lobbylist[0].IsLobbyComplete())
            {
                _ConnectionStatus = ClientConnectionStatus.Declined;
            }
            else
            {
                _ConnectionStatus = ClientConnectionStatus.Accepted;
            }
            new ClientConnection(_ConnectionStatus);
            Server.status = ValidationEnum.WaitingForPlayer;

        }

    }
}
