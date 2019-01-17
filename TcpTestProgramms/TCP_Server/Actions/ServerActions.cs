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
using TCP_Server.DataProvider;
using TCP_Server.Enum;
using TCP_Server.PROTOCOLS;
using TCP_Server.Support;
using TCP_Server.Test;

namespace TCP_Server.Actions
{
    public class ServerActions
    {
        public bool _gameStarted = false;
        public bool _ruleSet = false;
        
        public Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>> _protocolActions;
        private ServerInfo _serverInfo;
        private Game _game;
        private ClientDisconnection _disconnectionHandler;
        private ServerDataPackageProvider _dataPackageProvider;

        public static ManualResetEvent TurnFinished = new ManualResetEvent(false);

        public ServerActions(ServerInfo serverInfo, Game game,
            ClientDisconnection disconnectionHandler, ServerDataPackageProvider _dataPackageProvider)
        {
            _protocolActions = new Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>>();
            _disconnectionHandler = disconnectionHandler;
            _serverInfo = serverInfo;
            _game = game;
            this._dataPackageProvider = _dataPackageProvider;
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
                        communication.Send(_dataPackageProvider.GetPackage("ServerStartingGame"));
                    }
                }
                else
                    communication.Send(_dataPackageProvider.GetPackage("NotEnoughInfo"));
            }
            else
                communication.Send(_dataPackageProvider.GetPackage("OnlyMasterStartInfo"));
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
            if (_game.State.CurrentPlayer ==  currentCommunication)
            {
				_game.State.ExecuteStateAction("rolldice");

                communication.Send(_dataPackageProvider.TurnInfo());
            }
            else
               communication.Send(_dataPackageProvider.GetPackage("NotYourTurn"));

			if (_game.State.TurnStateProp == "GameFinished")
				OnGameFinishedAction(communication, data);
        }

		private void OnGameFinishedAction(ICommunication communication, DataPackage data)
		{
			_game.State.ExecuteStateAction("finished");
			communication.Send(_dataPackageProvider.TurnInfo());
		}

		public void OnCloseGameAction(ICommunication communication, DataPackage data)
        {
			_game.State.ExecuteStateAction("close");
            _disconnectionHandler.DisconnectClient();
		}

        public void OnValidationAction(ICommunication communication, DataPackage data)
        {
            ValidationSystem._isValidated = true;
            Core.State = StateEnum.LobbyState;
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
