using Newtonsoft.Json;
using Shared.Communications;
using Shared.Contracts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TCP_Client.DTO;
using Wrapper.Implementation;
using Wrapper.Contracts;
using Wrapper.View;
using Wrapper;
using System.Threading;
using Shared.Contract;
using System.Linq;

namespace TCP_Client.Actions
{
    public class ProtocolAction
    {
        public Dictionary<ProtocolActionEnum, Action<DataPackage,ICommunication>> _protocolActions;
        public Dictionary<int, BroadcastDTO> _serverDictionary = new Dictionary<int, BroadcastDTO>();
        private Dictionary<ClientView, IView> _views;
        private ClientDataPackageProvider _clientDataPackageProvider;

        private readonly IUpdateOutputView _serverTableView;
        private readonly IUpdateOutputView _commandListOutputView;
        private readonly IUpdateOutputView _infoOutputView;
        private readonly IUpdateOutputView _boardOutputView;
        private readonly IErrorView _errorView;
        private readonly IUpdateOutputView _gameInfoOutputView;
        private readonly IUpdateOutputView _turnInfoOutputView;
        private readonly IUpdateOutputView _afterTurnOutputView;
        private readonly IUpdateOutputView _mainMenuOutputView;
        private readonly IUpdateOutputView _lobbyInfoDisplayView;
        private readonly IUpdateOutputView _finishInfoView;
        private readonly IUpdateOutputView _finishSkull1View;
        private readonly IUpdateOutputView _finishSkull2View;
        private readonly IUpdateOutputView _finishSkull3View;
        public readonly IUpdateOutputView _enterToRefreshView;

        private readonly Client _client;

        public string _serverTable =string.Empty;

        private OutputWrapper _outputWrapper;

        public ProtocolAction(Dictionary<ClientView, IView> views, Client client, ClientDataPackageProvider clientDataPackageProvider)
        {
            _clientDataPackageProvider = clientDataPackageProvider;
            _client = client;
            _views = views;
            _serverTableView = views[ClientView.ServerTable] as IUpdateOutputView;
            _commandListOutputView = views[ClientView.CommandList] as IUpdateOutputView;                   
            _boardOutputView = views[ClientView.Board] as IUpdateOutputView;
            _errorView = views[ClientView.Error] as IErrorView;
            _gameInfoOutputView = views[ClientView.GameInfo] as IUpdateOutputView;
            _turnInfoOutputView = views[ClientView.TurnInfo] as IUpdateOutputView;
            _afterTurnOutputView = views[ClientView.AfterTurnOutput] as IUpdateOutputView;
            _mainMenuOutputView = views[ClientView.MenuOutput] as IUpdateOutputView;
            _lobbyInfoDisplayView = views[ClientView.LobbyInfoDisplay] as IUpdateOutputView;
            _infoOutputView = views[ClientView.InfoOutput] as IUpdateOutputView;
            _finishInfoView = views[ClientView.FinishInfo] as IUpdateOutputView;
            _finishSkull1View = views[ClientView.FinishSkull1] as IUpdateOutputView;
            _finishSkull3View = views[ClientView.FinishSkull3] as IUpdateOutputView;
            _finishSkull2View = views[ClientView.FinishSkull2] as IUpdateOutputView;
            _enterToRefreshView = views[ClientView.EnterToRefresh] as IUpdateOutputView;

            _protocolActions = new Dictionary<ProtocolActionEnum, Action<DataPackage,ICommunication>>
            {
                { ProtocolActionEnum.HelpText, OnHelpTextAction},
                //{ ProtocolActionEnum.UpdateView, OnUpdateAction},
                //{ ProtocolActionEnum.Broadcast, OnBroadcastAction },
                //{ ProtocolActionEnum.AcceptInfo, OnAcceptInfoAction },
                //{ ProtocolActionEnum.DeclineInfo, OnDeclineInfoAction },
                { ProtocolActionEnum.Restart, OnRestartAction },
                //{ ProtocolActionEnum.ValidationRequest, OnValidationRequestAction },
                //{ ProtocolActionEnum.ValidationAccepted, OnValidationAcceptedAction },
                //{ ProtocolActionEnum.LobbyCheckFailed, OnLobbyCheckFailedAction },
                //{ ProtocolActionEnum.LobbyCheckSuccessful, OnLobbyCheckSuccessfulAction },
                //{ ProtocolActionEnum.ServerStartingGame, OnServerStartingGameAction }
            
            };
            _outputWrapper = new OutputWrapper();
        }

        public void ExecuteDataActionFor(DataPackage data,  ICommunication communication)
        {
            if (_protocolActions.TryGetValue(data.Header, out var protocolAction) == false)
                return;

            protocolAction(data, communication);
        }

        public BroadcastDTO GetServer(int key) => _serverDictionary[key];

        #region Protocol actions

        public void OnHelpTextAction(DataPackage data, ICommunication communication)
        {
            var helpText = MapProtocolToDto<HelpTextDTO>(data);
            //_helpOutputView.SetUpdateContent(helpText._HelpText);
        }

        public void OnUpdateAction(DataPackage data, ICommunication communication)
        {
            var updatedView = MapProtocolToDto<UpdateDTO>(data);

			//updatedView._commandList;
			//updatedView._diceResult;
			//updatedView._infoOutput;
			//updatedView._lastPlayer;
			//updatedView._lobbyDisplay;
			//updatedView._turnstate;




           
            if (!(updatedView._lobbyDisplay == null || updatedView._lobbyDisplay.Length == 0))
            {
                _lobbyInfoDisplayView.ViewEnabled = true;
                _lobbyInfoDisplayView.SetUpdateContent(updatedView._lobbyDisplay);
            }
            if(!(updatedView._commandList == null || updatedView._commandList.Length == 0))
            {
                _commandListOutputView.ViewEnabled = true;
                _commandListOutputView.SetUpdateContent(updatedView._commandList);
            }
            if(!(updatedView._infoOutput == null || updatedView._infoOutput.Length == 0))
            {
                _infoOutputView.ViewEnabled = true;
                _infoOutputView.SetUpdateContent(updatedView._infoOutput);
            }


        }

        private List<IPEndPoint> _serverEndpoints = new List<IPEndPoint>(); 
        private string[] _servernames = new string[100];
        private int[] _maxPlayerCount = new int[100];
        private int[] _currentPlayerCount = new int[100];
        private int _keyIndex = 0;

        public void OnBroadcastAction(DataPackage data, ICommunication communication)
        {
            var broadcast = MapProtocolToDto<BroadcastDTO>(data);

            var currentIPEndPoint = new IPEndPoint(IPAddress.Parse(broadcast._server_ip),broadcast._server_Port);

            if (_serverEndpoints.Contains(currentIPEndPoint))
            {
                var servernumber = _serverEndpoints.IndexOf(currentIPEndPoint);
                _servernames[servernumber] = broadcast._server_name;
                _maxPlayerCount[servernumber] = broadcast._maxPlayerCount;
                _currentPlayerCount[servernumber] = broadcast._currentPlayerCount;
            }
            else
            {
                _serverEndpoints.Add(currentIPEndPoint);

                _serverDictionary.Add(_keyIndex, broadcast);

                _servernames[_keyIndex] = broadcast._server_name;
                _maxPlayerCount[_keyIndex] = broadcast._maxPlayerCount;
                _currentPlayerCount[_keyIndex] = broadcast._currentPlayerCount;
                _keyIndex++;
            }

            var outputFormat = new StringBuilder();

            for (int index = 0; index < _serverDictionary.Count; index++)
                outputFormat.Append(string.Format("{3,2}  [{0,1}/{1,1}]   {2,20}\n", _currentPlayerCount[index],
                    _maxPlayerCount[index], _servernames[index].PadRight(20),(index+1)));

            _serverTable = outputFormat.ToString();
            _serverTableView.SetUpdateContent(_serverTable);
            _serverTable = string.Empty;
            _serverTableView.ViewEnabled = true;
        }

        public void OnAcceptInfoAction(DataPackage data, ICommunication communication)

        {
            var accept = MapProtocolToDto<AcceptDTO>(data);
            _infoOutputView.ViewEnabled = true;
            _infoOutputView.SetUpdateContent(accept._smallUpdate);
        }

        public void OnDeclineInfoAction(DataPackage data, ICommunication communication)
        {
            _client._inputHandler._declined = true;
            _client._inputHandler._isConnected = false;

            var decline = MapProtocolToDto<DeclineDTO>(data);
            _infoOutputView.ViewEnabled = true;
            _infoOutputView.SetUpdateContent(decline._smallUpdate);
   
        }

        public void OnRestartAction(DataPackage obj, ICommunication communication)
        {
                _finishInfoView.ViewEnabled = false;
                _finishSkull1View.ViewEnabled = false;
                _finishSkull2View.ViewEnabled = false;
                _finishSkull3View.ViewEnabled = false;
                EnableViews();
        }

        public void OnValidationRequestAction(DataPackage data, ICommunication communication)
        {
            communication.Send(_clientDataPackageProvider.GetPackage("ValidationAnswer"));
        }

        public void OnValidationAcceptedAction(DataPackage data, ICommunication communication)
        {
            _client.SwitchState(StateEnum.ClientStates.WaitingForLobbyCheck);
        }

        public void OnLobbyCheckFailedAction(DataPackage data, ICommunication communication)
        {
            _client.SwitchState(StateEnum.ClientStates.NotConnected);
        }

        public void OnLobbyCheckSuccessfulAction(DataPackage data, ICommunication communication)
        {
            _client.SwitchState(StateEnum.ClientStates.Lobby);
        }

        public void OnServerStartingGameAction(DataPackage data, ICommunication communication)
        {
            _client.SwitchState(StateEnum.ClientStates.GameRunning);
        }

        public void DisableViews()
        {
            _commandListOutputView.ViewEnabled = false;
            _errorView.ViewEnabled = false;
            _gameInfoOutputView.ViewEnabled = false;
            _infoOutputView.ViewEnabled = false;
            _mainMenuOutputView.ViewEnabled = false;
            _boardOutputView.ViewEnabled = false;
            _afterTurnOutputView.ViewEnabled = false;
            _lobbyInfoDisplayView.ViewEnabled = false;
            _turnInfoOutputView.ViewEnabled = false;
            _enterToRefreshView.ViewEnabled = false;
        }

        public void EnableViews()
        {
            _commandListOutputView.ViewEnabled = true;
            _enterToRefreshView.ViewEnabled = true;

            _infoOutputView.ViewEnabled = true;
            _mainMenuOutputView.ViewEnabled = true;
            _lobbyInfoDisplayView.ViewEnabled = true;

            _errorView.ViewEnabled = true;
        }
        #endregion

        #region Static helper functions

        private static T CreateProtocol<T>(DataPackage data) where T : IProtocol
        {
            //macht aus einem Objekt String ein wieder das urpsrüngliche Objekt Protokoll
            return JsonConvert.DeserializeObject<T>(data.Payload);
        }

        public static T MapProtocolToDto<T>(DataPackage data) where T : IProtocol
        {
            //macht aus einem Objekt String ein wieder das urpsrüngliche Objekt Protokoll
            return JsonConvert.DeserializeObject<T>(data.Payload);
        }

        #endregion
    }
}
