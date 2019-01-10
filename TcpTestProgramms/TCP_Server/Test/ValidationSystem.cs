﻿using Shared.Contract;
using System;
using System.Linq;
using System.Timers;
using TCP_Server.Actions;
using TCP_Server.Enum;

namespace TCP_Server.Test
{
    public class ValidationSystem
    {
        private ServerInfo _serverInfo;
        private bool isRunning;
        private bool timerElapsed = false;
        private ClientDisconnection _disconnectionHandler;
        private ClientConnection _connectionHandler;
        private ServerDataPackageProvider _dataPackageProvider;
        public ICommunication currentcommunication;
        public static bool validationStatus = false;
        private ServerActions _serverActions;
        

        private Timer timer;


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
            isRunning = true;
            Core.ValidationStatus = ValidationEnum.WaitingForPlayer;

            while (isRunning)
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

        public void ValidateClient()
        {
            _serverInfo._communications.Last().SetNWStream();
                   
            _serverInfo._communications.Last().Send(_dataPackageProvider.GetPackage("ValidationRequest"));

            timer = new Timer(3000);
            timer.Enabled = true;
            timer.AutoReset = false;
            timer.Elapsed += timerSetter;

            while (!validationStatus && !timerElapsed)
            {

            }

            if (validationStatus)
                Core.ValidationStatus = ValidationEnum.LobbyCheck;
            else
                Core.ValidationStatus = ValidationEnum.DeclineState;
            
        }
       

        private void timerSetter(Object source, ElapsedEventArgs e)
        {
            timerElapsed = true;
        }
    }
}
