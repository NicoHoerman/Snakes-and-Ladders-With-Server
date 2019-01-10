using Shared.Communications;
using Shared.Contract;
using System;
using System.Collections.Generic;
using TCP_Server.Enum;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using Shared.Enums;
using Newtonsoft.Json;
using TCP_Server.PROTOCOLS;
using TCP_Server.Actions;

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
        private bool validationStatus = false;
        private ServerActions _serverActions;

        private System.Timers.Timer timer;


        public ValidationSystem(ServerInfo serverInfo,ClientDisconnection disconnectionHandler
            , ClientConnection connectionHandler, DataPackageProvider dataPackageProvider, ServerActions serverActions)
        {
            _serverInfo = serverInfo;
            _disconnectionHandler = disconnectionHandler;
            _connectionHandler = connectionHandler;
            _dataPackageProvider = dataPackageProvider;
            _serverActions = serverActions;
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
                        ValidateClientAsync();
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

        async Task<bool> ValidateClientAsync()
        {
            var ValidationRequestPackage = new DataPackage
            {

                Header = ProtocolActionEnum.ValidationRequest,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {

                })
            };

            ValidationRequestPackage.Size = ValidationRequestPackage.ToByteArray().Length;

            _serverInfo._communications[_serverInfo._communications.Count].Send(ValidationRequestPackage);

            validationStatus = await _serverActions.OnValidationAction();

            timer = new System.Timers.Timer(5000);
            timer.Enabled = true;
            timer.Elapsed += ValidationHelper;

            return validationStatus;
        }

        private void ValidationHelper(Object source, ElapsedEventArgs e)
        {
            if (validationStatus)
                Core.ValidationStatus = ValidationEnum.LobbyCheck;
            else
                Core.ValidationStatus = ValidationEnum.DeclineState;
        }
    }
}
