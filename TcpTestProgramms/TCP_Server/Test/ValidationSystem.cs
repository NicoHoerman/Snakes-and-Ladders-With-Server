using System;
using System.Collections.Generic;
using TCP_Server.Enum;
using System.Threading.Tasks;
using System.Threading;
using Shared.Enums;
using Newtonsoft.Json;
using Shared.Communications;
using TCP_Server.PROTOCOLS;
using System.Timers;

namespace TCP_Server.Test
{
    public class ValidationSystem
    {
        private static System.Timers.Timer timer;
        private ServerInfo _serverInfo;
        private bool isRunning;
        private Server _server;
        bool validationStatus;

        public ValidationSystem(ServerInfo serverInfo, Server server)
        {
            _serverInfo = serverInfo;
            _server = server;
        }

        public ClientConnectionStatus _ConnectionStatus = ClientConnectionStatus.Pending;

        public void Start()
        {
            isRunning = true;

            while (isRunning)
            {
                switch (Server.ValidationStatus)
                {
                    case ValidationEnum.WaitingForPlayer:
                        //Something
                        break;
                    case ValidationEnum.ValidationState:
                         ValidateClientAsync();                      
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
                _ConnectionStatus = ClientConnectionStatus.Declined;
            }
            else
            {
                _ConnectionStatus = ClientConnectionStatus.Accepted;
            }
            new ClientConnection(_ConnectionStatus, _serverInfo, _server);
            Server.ValidationStatus = ValidationEnum.WaitingForPlayer;

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

            _serverInfo._communications.ForEach(communication =>
            {
                communication.Send(ValidationRequestPackage);
            });          

            validationStatus = await _server._ActionsHandler.OnValidationAction();

            timer = new System.Timers.Timer(5000);
            timer.Enabled = true;
            timer.Elapsed += ValidationHelper;

            return validationStatus;
        }

        private void ValidationHelper(Object source, ElapsedEventArgs e)
        {
            if (validationStatus)
                _server.SwitchState(ValidationEnum.LobbyCheck);
            else
                _server.SwitchState(ValidationEnum.DeclineState);
        }

       

    }
}
