using Shared.Communications;
using Shared.Contract;
using System;
using System.Collections.Generic;
using TCP_Server.Enum;

namespace TCP_Server.Test
{
    public class ValidationSystem
    {
        private ServerInfo _serverInfo;
        private bool isRunning;
        private ClientDisconnection _disconnectionHandler;
        private ClientConnection _connectionHandler;
        private DataPackageProvider _dataPackageProvider;
        public ICommunication currentcommunication;

        public ValidationSystem(ServerInfo serverInfo,ClientDisconnection disconnectionHandler
            , ClientConnection connectionHandler, DataPackageProvider dataPackageProvider)
        {
            _serverInfo = serverInfo;
            _disconnectionHandler = disconnectionHandler;
            _connectionHandler = connectionHandler;
            _dataPackageProvider = dataPackageProvider;
        }

        public void Start()
        {
            isRunning = true;
            Core.ValidationStatus = ValidationEnum.WaitingForPlayer;

            while (isRunning)
            {
                switch (Core.ValidationStatus)
                {
                    case ValidationEnum.WaitingForPlayer:
                        
                        break;
                    case ValidationEnum.ValidationState:
                        
                        break;
                    case ValidationEnum.LobbyCheck:
                        LobbyCheck();
                        break;
                    case ValidationEnum.DeclineState:
                        _disconnectionHandler.DisconnectClient();
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
                Core.ConnectionStatus = ClientConnectionStatus.Declined;
                _disconnectionHandler.Execute(currentcommunication);
            }
            else
            {
                Core.ConnectionStatus = ClientConnectionStatus.Accepted;
                _connectionHandler.Execute(currentcommunication);
            }
            Core.ValidationStatus = ValidationEnum.WaitingForPlayer;
        }
    }
}
