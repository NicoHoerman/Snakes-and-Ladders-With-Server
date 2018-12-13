using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.GameAndLogic;
using EandE_ServerModel.EandE.States;
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
using Wrapper;


namespace TCP_Server.Actions
{
    public class ServerActions
    {
        private string servername = string.Empty;
        private int currentplayer;
        private int maxplayer;
        private bool gameStarted = false;
        private bool ruleSet = false;

        private Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>> _protocolActions;
        private Server _server;
        private ServerInfo _ServerInfo;
        private Game _game;

        public static ManualResetEvent verificationVariableSet = new ManualResetEvent(false);
        public static ManualResetEvent MessageSent = new ManualResetEvent(false);
        public static ManualResetEvent StateSwitched = new ManualResetEvent(false);
        public static ManualResetEvent TurnFinished = new ManualResetEvent(false);
        public static ManualResetEvent EndscreenSet = new ManualResetEvent(false);

        public ClientConnectionAttempt _ConnectionStatus = ClientConnectionAttempt.NotSet;

        public ServerActions(ServerInfo serverInfo, Server server, Game game)
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
                        _lobbyDisplay = $"Current Lobby: {servername}. Players [{currentplayer}/{maxplayer}]",
                        _commandList = "Commands:\n/search (only available when not connected to a server)\n/startgame\n/closegame\n/rolldice\n/someCommand"

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
                        _infoOutput = "A player got declined"
                    })
                };
                updatePackage.Size = updatePackage.ToByteArray().Length;
                //a send that sends to everyone except the current comunication
                for (int i = 0; i <= _ServerInfo._communications.Count - 1; i++)
                {
                    if (!(_ServerInfo._communications[i] == communication))
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
            if (ruleSet)
                return;
            if (communication.IsMaster)
            {
                if (_server.isLobbyComplete())
                {
                    var masterDataPackage = new DataPackage
                    {

                        Header = ProtocolActionEnum.UpdateView,
                        Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                        {
                            _mainMenuOutput = "Choose a rule.\nRuleslist:\n/classic",
                            _error = _game.State.Error,
                            _lastinput = _game.State.Lastinput
                        })
                    };
                    masterDataPackage.Size = masterDataPackage.ToByteArray().Length;

                    communication.Send(masterDataPackage);

                    var playerDataPackage = new DataPackage
                    {

                        Header = ProtocolActionEnum.UpdateView,
                        Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                        {
                            _infoOutput = "Master is starting the Game"
                        })
                    };
                    playerDataPackage.Size = playerDataPackage.ToByteArray().Length;

                    for (int i = 0; i <= _ServerInfo._communications.Count - 1; i++)
                    {
                        if (!(_ServerInfo._communications[i] == communication))
                            _ServerInfo._communications[i].Send(playerDataPackage);
                    }

                    _game.State.ClearProperties();
                    gameStarted = true;
                }
                else
                {
                    var dataPackage = new DataPackage
                    {

                        Header = ProtocolActionEnum.UpdateView,
                        Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                        {
                            _infoOutput = "Not enough Players to start the game "
                        })
                    };
                    dataPackage.Size = dataPackage.ToByteArray().Length;

                    communication.Send(dataPackage);
                }
            }
            else
            {
                var dataPackage = new DataPackage
                {

                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                    {
                        _infoOutput = "Only the Master can start the Game"
                    })
                };
                dataPackage.Size = dataPackage.ToByteArray().Length;

                communication.Send(dataPackage);
            }
        }
        private void OnClassicAction(ICommunication communication, DataPackage data)
        {
            if (!gameStarted)
            {
                return;
            }
            if (communication.IsMaster)
            {
                _game.State.SetInput("/classic");

                StateSwitched.WaitOne();
                StateSwitched.Reset();


                var dataPackage = new DataPackage
                {

                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                    {
                        _gameInfoOutput = _game.State.GameInfoOuptput,
                        _boardOutput = _game.State.BoardOutput,
                        _error = _game.State.Error,
                        _lastinput = _game.State.Lastinput,
                        _turnInfoOutput = _game.State.TurnInfoOutput,
                        _afterTurnOutput = _game.State.AfterTurnOutput,
                        
                    })
                };
                dataPackage.Size = dataPackage.ToByteArray().Length;

                _game.State.ClearProperties();


                for (int i = 0; i <= _ServerInfo._communications.Count - 1; i++)
                {
                        _ServerInfo._communications[i].Send(dataPackage);
                }

                ruleSet = true;
            }
            else
            {
                var dataPackage = new DataPackage
                {

                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                    {
                        _infoOutput = "Only the Master can set the rules"
                    })
                };
                dataPackage.Size = dataPackage.ToByteArray().Length;

                communication.Send(dataPackage);
            }
        }

        private void OnRollDiceAction(ICommunication communication, DataPackage data)
        {
            if (!ruleSet)
            {
                return;
            }
            int currentCommunication = _ServerInfo._communications.FindIndex(x => x == communication)+1;

            //if (_game.State.CurrentPlayer ==  currentCommunication)
            //{
                _game.State.SetInput("/rolldice");
                TurnFinished.WaitOne();
                TurnFinished.Reset();

                var test = _game.State.ToString();
            if (test == "EandE_ServerModel.EandE.States.GameRunningState")
            {
                var turnPackage = new DataPackage
                {

                    Header = ProtocolActionEnum.UpdateView,
                    Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                    {
                        _gameInfoOutput = _game.State.GameInfoOuptput,
                        _boardOutput = _game.State.BoardOutput,
                        _error = _game.State.Error,
                        _lastinput = _game.State.Lastinput,
                        _turnInfoOutput = _game.State.TurnInfoOutput,
                        _afterTurnOutput = _game.State.AfterTurnOutput,
                        
                    })
                };
                turnPackage.Size = turnPackage.ToByteArray().Length;

                communication.Send(turnPackage);

                if(!GameRunningState.isRunning)
                    GameRunningState.GameFinished.Set();


                if (_game.State.ToString() == "EandE_ServerModel.EandE.States.GameFinishedState")
                {
                    EndscreenSet.WaitOne();
                    EndscreenSet.Reset();
                    {
                        var gameEndedPackage = new DataPackage
                        {
                            Header = ProtocolActionEnum.UpdateView,
                            Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                            {
                                _finishinfo = _game.State.FinishInfo,
                                _finishskull1 = _game.State.Finishskull1,
                                _finishskull2 = _game.State.Finishskull2
                            })
                        };
                        gameEndedPackage.Size = gameEndedPackage.ToByteArray().Length;

                        communication.Send(gameEndedPackage);
                    }

                }
            }
            else
            {
                return;
            }
             /*}
             else
             {
                 var dataPackage = new DataPackage
                 {

                     Header = ProtocolActionEnum.UpdateView,
                     Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                     {
                         _infoOutput = "Not your Turn"
                     })
                 };
                 dataPackage.Size = dataPackage.ToByteArray().Length;

                 communication.Send(dataPackage);
             }*/

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
