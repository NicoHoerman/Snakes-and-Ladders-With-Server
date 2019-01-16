using Shared.Contract;
using System;
using System.Linq;
using System.Threading;
using System.Timers;
using TCP_Server.Actions;
using TCP_Server.Enum;

namespace TCP_Server.Test
{
    public class ValidationSystem
    {
        private bool _isRunning;
        private bool _timerElapsed = false;
        public static bool _isValidated = false;

        private ServerInfo _serverInfo;
        private ServerActions _serverActions;
        private ServerDataPackageProvider _dataPackageProvider;
        private ClientConnection _connectionHandler;
        private ClientDisconnection _disconnectionHandler;
        private System.Timers.Timer _timer;

        public ICommunication _currentcommunication;

        public ValidationSystem(ServerInfo serverInfo,ClientDisconnection disconnectionHandler
            , ClientConnection connectionHandler, ServerDataPackageProvider dataPackageProvider, ServerActions serverActions)
        {
            _serverInfo = serverInfo;
            _disconnectionHandler = disconnectionHandler;
            _connectionHandler = connectionHandler;
            _dataPackageProvider = dataPackageProvider;
            _serverActions = serverActions;
        }

        public void Start()
        {
            _isRunning = true;
            Core.ValidationStatus = ValidationEnum.WaitingForPlayer;

            while (_isRunning)
            {
                switch (Core.ValidationStatus)
                {
                    case ValidationEnum.WaitingForPlayer:
                        
                        break;
                    case ValidationEnum.ValidationState:
                        ValidateClient();
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
            if (_serverInfo._lobbylist[0].IsLobbyComplete())
            {
                Core.ConnectionStatus = ClientConnectionStatus.Declined;
                _currentcommunication.Send(_dataPackageProvider.GetPackage("LobbyCheckFailed"));
                _disconnectionHandler.Execute(_currentcommunication);
                
            }
            else
            {
                Core.ConnectionStatus = ClientConnectionStatus.Accepted;
                _currentcommunication.Send(_dataPackageProvider.GetPackage("LobbyCheckSuccessful"));
                _connectionHandler.Execute(_currentcommunication);
            }
            Core.ValidationStatus = ValidationEnum.WaitingForPlayer;
        }

        public void ValidateClient()
        {
            _serverInfo._communications.Last().SetNWStream();

            _timer = new System.Timers.Timer(10000);
            _timer.Enabled = true;
            _timer.AutoReset = false;
            _timer.Elapsed += TimerSetter;

            while (!_isValidated && !_timerElapsed)
            {
                _serverInfo._communications.Last().Send(_dataPackageProvider.GetPackage("ValidationRequest"));
                Thread.Sleep(1000);
            }
            if (_isValidated)
            {
                _serverInfo._communications.Last().Send(_dataPackageProvider.GetPackage("ValidationAccepted"));
                Thread.Sleep(3);
                Core.ValidationStatus = ValidationEnum.LobbyCheck;
            }
            else
                Core.ValidationStatus = ValidationEnum.DeclineState;
        }

        private void TimerSetter(Object source, ElapsedEventArgs e)
        {
            _timerElapsed = true;
        }
    }
}
