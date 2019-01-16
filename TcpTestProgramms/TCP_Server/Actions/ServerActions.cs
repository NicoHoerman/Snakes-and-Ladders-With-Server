using EandE_ServerModel.EandE.GameAndLogic;
using EandE_ServerModel.EandE.States;
using Newtonsoft.Json;
using Shared.Communications;
using Shared.Contract;
using Shared.Contracts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using TCP_Server.PROTOCOLS;
using TCP_Server.Test;

namespace TCP_Server.Actions
{
    public class ServerActions
    {
        private int _currentplayer;
        public bool _gameStarted = false;
        public bool _ruleSet = false;
        
        public Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>> _protocolActions;
        private ServerInfo _serverInfo;
        private Game _game;
        private ClientDisconnection _disconnectionHandler;
        private ServerDataPackageProvider _dataPackageProvider;
        private GameFinishedState _finishedState;

        public static ManualResetEvent TurnFinished = new ManualResetEvent(false);

        public ServerActions(ServerInfo serverInfo, Game game,
            ClientDisconnection disconnectionHandler, ServerDataPackageProvider _dataPackageProvider)
        {
            _protocolActions = new Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>>();
            _disconnectionHandler = disconnectionHandler;
            _serverInfo = serverInfo;
            _game = game;
            this._dataPackageProvider = _dataPackageProvider;

			//_currentplayer = game.State.CurrentPlayer;
			_finishedState = new GameFinishedState(game, _currentplayer);
        }

        public void ExecuteDataActionFor(ICommunication communication, DataPackage data)
        {
            if (_protocolActions.TryGetValue(data.Header, out Action<ICommunication, DataPackage> protocolAction) == false)
                return;

            protocolAction(communication, data);
        }

        #region Protocol actions

        public void OnStartGameAction(ICommunication communication, DataPackage data)
        {
            if (communication.IsMaster)
            {
                if (_serverInfo._lobbylist[0].IsLobbyComplete())
                {
                    _gameStarted = true;
                    Core.State = StateEnum.GameRunningState;
                    for (int i = 0; i <= _serverInfo._communications.Count  -1; i++)
                    {
                        if(!(_serverInfo._communications[i] == communication))
                            communication.Send(_dataPackageProvider.GetPackage("PlayerData"));
						Thread.Sleep(1000);
						communication.Send(_dataPackageProvider.ServerStartingGame());
						communication.Send(_dataPackageProvider.BoardInfo());
                    }
                }
                else
                    communication.Send(_dataPackageProvider.GetPackage("NotEnoughInfo"));
            }
            else
                communication.Send(_dataPackageProvider.GetPackage("OnlyMasterStartInfo"));
            //_game.State.ClearProperties(); 
        }
        public void OnRuleAction(ICommunication communication, DataPackage data)
        {
            if (communication.IsMaster)
                _ruleSet = true;
            else
                communication.Send(_dataPackageProvider.GetPackage("OnlyMasterRuleInfo"));
        }

        public void OnRollDiceAction(ICommunication communication, DataPackage data)
        {
			int currentCommunication = _serverInfo._communications.FindIndex(x => x == communication)+1;
            //if (_game.State.CurrentPlayer ==  currentCommunication)
            //{
            //ExecuteTurn();
                _game.State.SetInput("/rolldice");
                TurnFinished.WaitOne();
                TurnFinished.Reset();

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

                if(!GameRunningState._isRunning)
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

                        _finishedState.ReactivateViews(communication);
                        Core.State = StateEnum.LobbyState;
                        _game.State.ClearProperties();
                    }
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

        public void OnGetHelpAction(ICommunication communication, DataPackage data)
        {

			PROT_HELPTEXT clientId = CreateProtocol<PROT_HELPTEXT>(data);

            _game.State.SetInput("/help");

            if (communication.IsMaster)
            {
                var dataPackage = new DataPackage
                {
                    Header = ProtocolActionEnum.HelpText,
                    Payload = JsonConvert.SerializeObject(new PROT_HELPTEXT
                    {
                        _helpText = _game.State.HelpOutput
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
                        _helpText = _game.State.HelpOutput
                    })
                };
                dataPackage.Size = dataPackage.ToByteArray().Length;
                communication.Send(dataPackage);
            }
        }

        public void OnCloseGameAction(ICommunication communication, DataPackage data)
        {
            _disconnectionHandler.DisconnectClient();
            _game.State.SetInput("/closegame");
        }

        public void OnValidationAction(ICommunication communication, DataPackage data)
        {
            ValidationSystem.isValidated = true;
            Core.State = StateEnum.LobbyState;
			communication.Send(_dataPackageProvider.LobbyDisplay());
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
