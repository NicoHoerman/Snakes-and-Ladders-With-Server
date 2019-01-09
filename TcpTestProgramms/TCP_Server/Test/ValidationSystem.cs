using System;
using System.Collections.Generic;
using TCP_Server.Enum;

namespace TCP_Server.Test
{
    public class ValidationSystem
    {
        private ServerInfo _serverInfo;
        private bool isRunning;
        private Validator validator;
        private ClientDisconnection _disconnectionHandler;
        private ClientConnection _connectionHandler;

        public ValidationSystem(ServerInfo serverInfo,ClientDisconnection disconnectionHandler
            , ClientConnection connectionHandler)
        {
            _serverInfo = serverInfo;
            validator = new Validator();
            _disconnectionHandler = disconnectionHandler;
            _connectionHandler = connectionHandler;
        }

        public void Start()
        {
            isRunning = true;
            Core.status = ValidationEnum.WaitingForPlayer;

            while (isRunning)
            {
                switch (Core.status)
                {
                    case ValidationEnum.WaitingForPlayer:
                        //Something
                        break;
                    case ValidationEnum.ValidationState:
                        validator.Validate();
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

        private void LobbyCheck()
        {
            if (_serverInfo.lobbylist[0].IsLobbyComplete())
            {
                Core._ConnectionStatus = ClientConnectionStatus.Declined;
                _disconnectionHandler.Execute();
            }
            else
            {
                Core._ConnectionStatus = ClientConnectionStatus.Accepted;
                _connectionHandler.Execute();
            }
            Core.status = ValidationEnum.WaitingForPlayer;
        }
    }
}
