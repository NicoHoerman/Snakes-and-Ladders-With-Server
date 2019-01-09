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
using TCP_Server.Test;
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
        private ServerInfo _serverInfo;
        private Game _game;
        private GameFinishedState finishedState;

        public static ManualResetEvent verificationVariableSet = new ManualResetEvent(false);
        public static ManualResetEvent MessageSent = new ManualResetEvent(false);
        public static ManualResetEvent StateSwitched = new ManualResetEvent(false);
        public static ManualResetEvent TurnFinished = new ManualResetEvent(false);

        public ServerActions(ServerInfo serverInfo, Game game)
        {
            finishedState = new GameFinishedState(game, currentplayer);

            _protocolActions = new Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>>
            {
                { ProtocolActionEnum.RollDice,  OnRollDiceAction },
                { ProtocolActionEnum.GetHelp,   OnGetHelpAction },
                { ProtocolActionEnum.StartGame, OnStartGameAction },
                { ProtocolActionEnum.CloseGame, OnCloseGameAction },
                { ProtocolActionEnum.OnConnection, OnConnectionAction },
                { ProtocolActionEnum.Rule, OnRuleAction }
            };
          
            _serverInfo = serverInfo;
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
            //Work in Progress
        }

        private void OnStartGameAction(ICommunication communication, DataPackage data)
        {
            if (ruleSet)
                return;
            if (communication.IsMaster)
            {
                if (_serverInfo.lobbylist[0].IsLobbyComplete())
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

                    for (int i = 0; i <= _serverInfo._communications.Count - 1; i++)
                    {
                        if (!(_serverInfo._communications[i] == communication))
                            _serverInfo._communications[i].Send(playerDataPackage);
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
        private void OnRuleAction(ICommunication communication, DataPackage data)
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


                for (int i = 0; i <= _serverInfo._communications.Count - 1; i++)
                {
                        _serverInfo._communications[i].Send(dataPackage);
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
            int currentCommunication = _serverInfo._communications.FindIndex(x => x == communication)+1;

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

                Thread.Sleep(100);
                if (_game.State.ToString() == "EandE_ServerModel.EandE.States.GameFinishedState")
                {
                    
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
                        gameEndedPackage.Size = gameEndedPackage.ToByteArrayUTF().Length;

                        communication.Send(gameEndedPackage);

                        Thread.Sleep(5000);

                        finishedState.reactivateViews(communication);
                        _game.State.ClearProperties();

                    }

                }
            }
            else
            {
                return;
            }
            /* }
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
            communicationsToRemove.Add(communication);
            RemoveFromLobby();
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
