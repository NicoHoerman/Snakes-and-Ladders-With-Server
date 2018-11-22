using Newtonsoft.Json;
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
        public static ManualResetEvent MessageSent = new ManualResetEvent(false);

        ServerInfo _ServerInfo;
        public ClientConnectionAttempt _ConnectionStatus = ClientConnectionAttempt.NotSet;

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

            var dataPackage = new DataPackage();
            if (_ConnectionStatus == ClientConnectionAttempt.Accepted)
            {
                dataPackage = new DataPackage
                {
                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_ACCEPT
                    {
                        _SmallUpdate = "You are Conected to the Server and in the Lobby\n " +
                            $"Lobby {servername} Player [{currentplayer}/{maxplayer}]"
                    })
                };
            }
            else if (_ConnectionStatus == ClientConnectionAttempt.Declined)
            {
                dataPackage = new DataPackage
                {
                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_DECLINE
                    {
                        _SmallUpdate = "You got declined Lobby is probably full"
                    })
                };

                var updatePackage = new DataPackage
                {
                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                    {
                        _SmallUpdate = "A player got decliend" 
                    })
                };
                updatePackage.Size = updatePackage.ToByteArray().Length;
                //A send that sends to everyone except the current comunication
                for(int i=0;i<= _ServerInfo._communications.Count-1; i++)
                {
                    if(!(_ServerInfo._communications[i] == communication))
                    _ServerInfo._communications[i].Send(updatePackage);
                }
            }
            else if (_ConnectionStatus == ClientConnectionAttempt.NotSet)
                throw new InvalidOperationException();

            _ConnectionStatus = ClientConnectionAttempt.NotSet;
            
            dataPackage.Size = dataPackage.ToByteArray().Length;
            communication.Send(dataPackage);
            MessageSent.Set();
        }

        private void OnRollDiceAction(ICommunication communication, DataPackage data)
        {
            throw new NotImplementedException();
            var dataPackage = new DataPackage
            {

                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _GameViewUpdate = "Placeholder",
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        private void OnGetHelpAction(ICommunication communication, DataPackage data)
        {
            throw new NotImplementedException();
            var clientId = CreateProtocol<PROT_HELPTEXT>(data);

            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.HelpText,
                Payload = JsonConvert.SerializeObject(new PROT_HELPTEXT
                {
                    
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
