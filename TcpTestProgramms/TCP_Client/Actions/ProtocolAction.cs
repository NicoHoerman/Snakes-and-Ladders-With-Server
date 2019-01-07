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


namespace TCP_Client.Actions
{
    public class ProtocolAction
    {
        public Dictionary<ProtocolActionEnum, Action<DataPackage>> _protocolActions;
        public Dictionary<int, BroadcastDTO> _serverDictionary = new Dictionary<int, BroadcastDTO>();
        public Dictionary<ClientView, IView> _views;
        private List<IView> _viewList;
        #region ViewInitialization
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
        #endregion

        private readonly Client _client;

        public string _serverTable =string.Empty;

        private OutputWrapper outputWrapper;

        public ProtocolAction(Dictionary<ClientView, IView> views, Client client)
        {
            _client = client;
            _views = views;

            #region ViewConstructing
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
            #endregion

            _protocolActions = new Dictionary<ProtocolActionEnum, Action<DataPackage>>
            {
                { ProtocolActionEnum.UpdateView, OnUpdateAction},
                { ProtocolActionEnum.Broadcast, OnBroadcastAction },
                { ProtocolActionEnum.Accept, OnAcceptAction },
                { ProtocolActionEnum.Decline, OnDeclineAction },
                { ProtocolActionEnum.Restart, OnRestartAction }           
            };

            outputWrapper = new OutputWrapper();
            _viewList.AddRange(_serverTableView, _commandListOutputView, _infoOutputView, 
                _errorView, _boardOutputView, _gameInfoOutputView, _turnInfoOutputView, 
                _afterTurnOutputView, _mainMenuOutputView, _lobbyInfoDisplayView, 
                _finishInfoView, _finishSkull1View, _finishSkull2View, _finishSkull3View, 
                _enterToRefreshView);
        }

        

        public void ExecuteDataActionFor(DataPackage data)
        {
            //weißt dem packet die richtige funktion zu
            if (_protocolActions.TryGetValue(data.Header, out var protocolAction) == false)
                throw new InvalidOperationException("Invalid communication");
            //führt die bekommene methode mit dem datapackage aus
            protocolAction(data);
        }

        public BroadcastDTO GetServer(int key) => _serverDictionary[key];

        #region Protocol actions

        private void OnUpdateAction(DataPackage data)
        {
            
            var updatedView = MapProtocolToDto<UpdateDTO>(data);
           
            if(!(updatedView._mainMenuOutput == null || updatedView._mainMenuOutput.Length == 0))
            {
                _mainMenuOutputView.viewEnabled = true;
                _mainMenuOutputView.SetUpdateContent(updatedView._mainMenuOutput);
            }

            //_lobbyInfoDisplayView.viewEnabled = updatedView._lobbyDisplay != null && updatedView._lobbyDisplay.Length != 0; ;
            //_lobbyInfoDisplayView.SetUpdateContent(updatedView._lobbyDisplay);
            if (!(updatedView._lobbyDisplay == null || updatedView._lobbyDisplay.Length == 0))
            {
                _lobbyInfoDisplayView.viewEnabled = true;
                _lobbyInfoDisplayView.SetUpdateContent(updatedView._lobbyDisplay);
            }

            if (!(updatedView._boardOutput == null || updatedView._boardOutput.Length == 0))
            {
                _boardOutputView.viewEnabled = true;
                _boardOutputView.SetUpdateContent(updatedView._boardOutput);
            }      
            if(!(updatedView._error == null || updatedView._error.Length == 0))
            {
                _errorView.viewEnabled = true;
                _errorView.SetContent(updatedView._lastinput, updatedView._error);
            }
            if(!(updatedView._gameInfoOutput == null || updatedView._gameInfoOutput.Length == 0))
            {
                _gameInfoOutputView.viewEnabled = true;
                _gameInfoOutputView.SetUpdateContent(updatedView._gameInfoOutput);
            }
            if(!(updatedView._turnInfoOutput == null || updatedView._turnInfoOutput.Length == 0))
            {
                _turnInfoOutputView.viewEnabled = true;
                _turnInfoOutputView.SetUpdateContent(updatedView._turnInfoOutput);
            }
            if(!(updatedView._afterTurnOutput == null || updatedView._afterTurnOutput.Length == 0))
            {
                _afterTurnOutputView.viewEnabled = true;
                _afterTurnOutputView.SetUpdateContent(updatedView._afterTurnOutput);
            }
            if(!(updatedView._commandList == null || updatedView._commandList.Length == 0))
            {
                _commandListOutputView.viewEnabled = true;
                _commandListOutputView.SetUpdateContent(updatedView._commandList);
            }
            if(!(updatedView._infoOutput == null || updatedView._infoOutput.Length == 0))
            {
                _infoOutputView.viewEnabled = true;
                _infoOutputView.SetUpdateContent(updatedView._infoOutput);
            }
            if(!(updatedView._finishinfo == null || updatedView._finishinfo.Length == 0))
            {
                DisableViews(); 
                
                _finishInfoView.viewEnabled = true;
                _finishInfoView.SetUpdateContent(updatedView._finishinfo);
            }
            if(!(updatedView._finishskull1 == null || updatedView._finishskull1.Length == 0))
            {
                _finishSkull1View.viewEnabled = true;
                _finishSkull3View.viewEnabled = true;
                _finishSkull1View.SetUpdateContent(updatedView._finishskull1);
                _finishSkull3View.SetUpdateContent(updatedView._finishskull1);
            }
            if(!(updatedView._finishskull2 == null || updatedView._finishskull2.Length == 0))
            {
                _finishSkull2View.viewEnabled = true;
                _finishSkull2View.SetUpdateContent(updatedView._finishskull2);
            }
        }

        private List<IPEndPoint> _ServerEndpoints = new List<IPEndPoint>(); 
        private string[] _Servernames = new string[100];
        private int[] _MaxPlayerCount = new int[100];
        private int[] _CurrentPlayerCount = new int[100];
        private int keyIndex = 0;


        private void OnBroadcastAction(DataPackage data)
        {
            var broadcast = MapProtocolToDto<BroadcastDTO>(data);

            var currentIPEndPoint = new IPEndPoint(IPAddress.Parse(broadcast._Server_ip),broadcast._Server_Port);

            if (_ServerEndpoints.Contains(currentIPEndPoint))
            {
                var servernumber = _ServerEndpoints.IndexOf(currentIPEndPoint);
                _Servernames[servernumber] = broadcast._Server_name;
                _MaxPlayerCount[servernumber] = broadcast._MaxPlayerCount;
                _CurrentPlayerCount[servernumber] = broadcast._CurrentPlayerCount;
            }
            else
            {
                _ServerEndpoints.Add(currentIPEndPoint);

                _serverDictionary.Add(keyIndex, broadcast);

                _Servernames[keyIndex] = broadcast._Server_name;
                _MaxPlayerCount[keyIndex] = broadcast._MaxPlayerCount;
                _CurrentPlayerCount[keyIndex] = broadcast._CurrentPlayerCount;
                keyIndex++;
            }

            var outputFormat = new StringBuilder();

            for (int index = 0; index < _serverDictionary.Count; index++)
                outputFormat.Append(string.Format("{3,2}  [{0,1}/{1,1}]   {2,20}\n", _CurrentPlayerCount[index],
                    _MaxPlayerCount[index], _Servernames[index].PadRight(20),(index+1)));

            _serverTable = outputFormat.ToString();
            _serverTableView.SetUpdateContent(_serverTable);
            _serverTable = string.Empty;
            _serverTableView.viewEnabled = true;
            // Key Player Server 
            //
            //  1  [0/4]  XD
            //  2  [1/2]  LuL
        }

        private void OnAcceptAction(DataPackage data)

        {
            var accept = MapProtocolToDto<AcceptDTO>(data);
            _infoOutputView.viewEnabled = true;
            _infoOutputView.SetUpdateContent(accept._SmallUpdate);
        }

        private void OnDeclineAction(DataPackage data)
        {
            _client._InputHandler.Declined = true;
            _client._InputHandler.isConnected = false;

            var decline = MapProtocolToDto<DeclineDTO>(data);
            _infoOutputView.viewEnabled = true;
            _infoOutputView.SetUpdateContent(decline._SmallUpdate);
   
        }

        private void OnRestartAction(DataPackage obj)
        {
                _finishInfoView.viewEnabled = false;
                _finishSkull1View.viewEnabled = false;
                _finishSkull2View.viewEnabled = false;
                _finishSkull3View.viewEnabled = false;
                EnableViews();
        }

        public void DisableViews()
        {
            
            _commandListOutputView.viewEnabled = false;
            _errorView.viewEnabled = false;
            _gameInfoOutputView.viewEnabled = false;
            _infoOutputView.viewEnabled = false;
            _mainMenuOutputView.viewEnabled = false;
            _boardOutputView.viewEnabled = false;
            _afterTurnOutputView.viewEnabled = false;
            _lobbyInfoDisplayView.viewEnabled = false;
            _turnInfoOutputView.viewEnabled = false;
            _enterToRefreshView.viewEnabled = false;
        }

        public void EnableViews()
        {
            _commandListOutputView.viewEnabled = true;
            _enterToRefreshView.viewEnabled = true;

            _infoOutputView.viewEnabled = true;
            _mainMenuOutputView.viewEnabled = true;
            _lobbyInfoDisplayView.viewEnabled = true;

            _errorView.viewEnabled = true;
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
