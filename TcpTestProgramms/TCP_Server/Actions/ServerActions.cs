using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.GameAndLogic;
using Newtonsoft.Json;
using Shared.Communications;
using Shared.Contract;
using Shared.Contracts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private Server _server;
        private ServerInfo _ServerInfo;
        private Game _game; 

        public static ManualResetEvent verificationVariableSet = new ManualResetEvent(false);
        public static ManualResetEvent MessageSent = new ManualResetEvent(false);
        public static ManualResetEvent StateSwitched = new ManualResetEvent(false);

        public ClientConnectionAttempt _ConnectionStatus = ClientConnectionAttempt.NotSet;

        public ServerActions(ServerInfo serverInfo,Server server,Game game)
        {
            _protocolActions = new Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>>
            {
                { ProtocolActionEnum.RollDice,  OnRollDiceAction },
                { ProtocolActionEnum.GetHelp,   OnGetHelpAction },
                { ProtocolActionEnum.StartGame, OnStartGameAction },
                { ProtocolActionEnum.CloseGame, OnCloseGameAction },
                { ProtocolActionEnum.OnConnection, OnConnectionAction },          
                { ProtocolActionEnum.Classic, OnClassicAction }
            };

            _server = server;
            _ServerInfo = serverInfo;
            _game = game;
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
            var lobbyUpdatePackage = new DataPackage();
            if (_ConnectionStatus == ClientConnectionAttempt.Accepted)
            {
                dataPackage = new DataPackage
                {
                    Header = ProtocolActionEnum.Accept,
                    Payload = JsonConvert.SerializeObject(new PROT_ACCEPT
                    {
                        _SmallUpdate = "You are connected to the Server and in the Lobby "
                    })
                };

                lobbyUpdatePackage = new DataPackage
                {
                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                    {
                        _lobbyDisplay = $"Lobby {servername} Player [{currentplayer}/{maxplayer}]"
                    })
                };
                lobbyUpdatePackage.Size = lobbyUpdatePackage.ToByteArray().Length;
                _ServerInfo._communications.ForEach(x => x.Send(lobbyUpdatePackage));
            }
            else if (_ConnectionStatus == ClientConnectionAttempt.Declined)
            {
                dataPackage = new DataPackage
                {
                    Header = ProtocolActionEnum.Decline,
                    Payload = JsonConvert.SerializeObject(new PROT_DECLINE
                    {
                        _SmallUpdate = "You got declined. Lobby is probably full"
                    })
                };

                var updatePackage = new DataPackage
                {
                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                    {
                        _SmallUpdate = "A player got declined"                                               
                    })
                };
                updatePackage.Size = updatePackage.ToByteArray().Length;
                //a send that sends to everyone except the current comunication
                for(int i=0;i<= _ServerInfo._communications.Count-1; i++)
                {
                    if(!(_ServerInfo._communications[i] == communication))
                    _ServerInfo._communications[i].Send(updatePackage);
                }
                //communication.Stop();
                //_server.communicationsToRemove.Add(communication);
                //_server.RemoveFromList();
            }
            else if (_ConnectionStatus == ClientConnectionAttempt.NotSet)
                throw new InvalidOperationException();

            _ConnectionStatus = ClientConnectionAttempt.NotSet;
            
            dataPackage.Size = dataPackage.ToByteArray().Length;
            Thread.Sleep(10);
            communication.Send(dataPackage);
            MessageSent.Set();
        }

        private void OnStartGameAction(ICommunication communication, DataPackage data)
        {
            
            if (_server.isLobbyComplete())
            {
                var dataPackage = new DataPackage
                {

                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                    {
                        _mainMenuOutput = "Choose a rule.\n Ruleslist:\n /classic",
                        _error = _game.State.Error,
                        _lastinput = _game.State.Lastinput
                    })
                };
                dataPackage.Size = dataPackage.ToByteArray().Length;

                communication.Send(dataPackage);

                _game.State.ClearProperties();
            }
            else
            {
                var dataPackage = new DataPackage
                {

                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                    {
                        _SmallUpdate = "Not enough Players to start the game "
                    })
                };
                dataPackage.Size = dataPackage.ToByteArray().Length;

                communication.Send(dataPackage);
            }
        }

        private void OnClassicAction(ICommunication communication, DataPackage data)
        {

            _game.State.SetInput("/classic");

            StateSwitched.WaitOne();
            StateSwitched.Reset();

            var dataPackage = new DataPackage
            {

                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _gameInfoOuptput = _game.State.GameInfoOuptput,
                    _boardOutput = _game.State.BoardOutput,
                    _error = _game.State.Error,
                    _lastinput = _game.State.Lastinput,
                    _afterBoardOutput = _game.State.AfterBoardOutput,
                    _afterTurnOutput = _game.State.AfterTurnOutput
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);

            _game.State.ClearProperties();
            
        }

        private void OnRollDiceAction(ICommunication communication, DataPackage data)
        {
            _game.State.SetInput("/rolldice");

            if(_game.State.ToString() == "GameRunningstate")
            {
                var turnPackage = new DataPackage
                {

                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                    {
                        _gameInfoOuptput = _game.State.GameInfoOuptput,
                        _boardOutput = _game.State.BoardOutput,
                        _error = _game.State.Error,
                        _lastinput = _game.State.Lastinput,
                        _afterBoardOutput = _game.State.AfterBoardOutput,
                        _afterTurnOutput = _game.State.AfterTurnOutput
                    })
                };
                    turnPackage.Size = turnPackage.ToByteArray().Length;

                    communication.Send(turnPackage);

            }
            else if(_game.State.ToString() == "GameFinishedsState")
            {
                var gameEndedPackage = new DataPackage
                {
                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                    {
                        _finishinfo = _game.State.Finishinfo,
                        _finishskull1 = _game.State.Finishskull1,
                        _finishskull2 = _game.State.Finishskull2
                    })
                };
                gameEndedPackage.Size = gameEndedPackage.ToByteArray().Length;

                communication.Send(gameEndedPackage);
            }
            else
            {
                throw new Exception();
            }
        }

    private void OnGetHelpAction(ICommunication communication, DataPackage data)
        {
            
            var clientId = CreateProtocol<PROT_HELPTEXT>(data);

            _game.State.SetInput("/help");

            if (communication.IsMaster)
            {
            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.HelpText,
                Payload = JsonConvert.SerializeObject(new PROT_HELPTEXT
                {
                    _HelpText = _game.State.HelpOutput
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;
            communication.Send(dataPackage);
            }
            else
            {

                var dataPackage = new DataPackage
                {
                    Header = ProtocolActionEnum.HelpText,
                    Payload = JsonConvert.SerializeObject(new PROT_HELPTEXT
                    {
                        _HelpText = _game.State.HelpOutput
                    })
                };
                dataPackage.Size = dataPackage.ToByteArray().Length;
                communication.Send(dataPackage);
            }

        }

        private void OnCloseGameAction(ICommunication communication, DataPackage data)
        {
            _game.State.SetInput("/closegame");
            communication.Stop();
            _server.communicationsToRemove.Add(communication);
            _server.RemoveFromLobby();
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
