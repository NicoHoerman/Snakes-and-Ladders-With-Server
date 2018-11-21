﻿using Newtonsoft.Json;
using Shared.Communications;
using Shared.Contract;
using Shared.Contracts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using TCP_Server.Enum;
using TCP_Server.PROTOCOLS;


namespace TCP_Server.Actions
{
    public class ServerActions
    {
        private string servername = string.Empty;
        private int currentplayer;
        private int maxplayer;

        private Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>> _protocolActions;
        public static ManualResetEvent verificationVariableSet = new ManualResetEvent(false);
        ServerInfo _ServerInfo;
        public ClientConnectionAttempt _ConnectionStatus;

        public ServerActions(ServerInfo serverInfo)
        {
            _protocolActions = new Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>>
            {
                { ProtocolActionEnum.RollDice,  OnRollDiceAction },
                { ProtocolActionEnum.GetHelp,   OnGetHelpAction },
                { ProtocolActionEnum.StartGame, OnStartGameAction },
                { ProtocolActionEnum.CloseGame, OnCloseGameAction },
                { ProtocolActionEnum.OnConnection,OnConnectionAction }
            };

            _ServerInfo = serverInfo;
        }

        public void ExecuteDataActionFor(ICommunication communication, DataPackage data)
        {
            if (_protocolActions.TryGetValue(data.Header, out var protocolAction) == false)
                throw new InvalidOperationException("Invalid communication");

            protocolAction(communication, data);
        }

        #region Protocol actions
        private void OnConnectionAction(ICommunication communication, DataPackage data)
        {
            servername = _ServerInfo._LobbyName;
            currentplayer = _ServerInfo._CurrentPlayerCount;
            maxplayer = _ServerInfo._MaxPlayerCount;

            verificationVariableSet.WaitOne();
            verificationVariableSet.Reset();

            string returnMsg = string.Empty;
            if(_ConnectionStatus == ClientConnectionAttempt.Accepted)
            {
                returnMsg = "You are Conected to the Server and in the Lobby\n " +
                            $"Lobby {servername} Player [{currentplayer}/{maxplayer}]";
            }
            else if(_ConnectionStatus == ClientConnectionAttempt.Declined)
            {
                throw new InvalidOperationException();
            }
            else if(_ConnectionStatus == ClientConnectionAttempt.NotSet)
                throw new InvalidOperationException();

            _ConnectionStatus = ClientConnectionAttempt.NotSet;
            var dataPackage = new DataPackage
            {

                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _Updated_View = returnMsg
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;
            communication.Send(dataPackage);

        }

        private void OnRollDiceAction(ICommunication communication, DataPackage data)
        {

            var dataPackage = new DataPackage
            {

                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _Updated_View = "Placeholder",
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        private void OnGetHelpAction(ICommunication communication, DataPackage data)
        {

            var clientId = CreateProtocol<PROT_HELPTEXT>(data);

            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.HelpText,
                Payload = JsonConvert.SerializeObject(new PROT_HELPTEXT
                {
                    _Text = $"Here is your help Player {clientId._Text}.\n " +
                    $"Commands:\n/rolldice\n/closegame\n"
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        private void OnCloseGameAction(ICommunication arg1, DataPackage arg2)
        {
            throw new NotImplementedException();
        }

        private void OnStartGameAction(ICommunication arg1, DataPackage arg2)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Static helper functions

        private static T CreateProtocol<T>(DataPackage data) where T : IProtocol
        {
            return JsonConvert.DeserializeObject<T>(data.Payload);
        }

        #endregion
    }
}
