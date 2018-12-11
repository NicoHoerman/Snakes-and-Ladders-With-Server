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

namespace TCP_Client.Actions
{
    public class ProtocolAction
    {
        public Dictionary<ProtocolActionEnum, Action<DataPackage>> _protocolActions;
        public Dictionary<int, BroadcastDTO> _serverDictionary = new Dictionary<int, BroadcastDTO>();
        private Dictionary<ClientView, IView> _views;

        private readonly IUpdateOutputView _serverTableView;
        private readonly IUpdateOutputView _helpOutputView;
        private readonly IUpdateOutputView _smallUpdateOutputView;
        
        private readonly IUpdateOutputView _additionalInformationView;
        private readonly IUpdateOutputView _boardOutputView;
        private readonly IErrorView _errorView;
        private readonly IUpdateOutputView _gameInfoOutputView;
        private readonly IUpdateOutputView _turnInfoOutputView;
        private readonly IUpdateOutputView _afterTurnOutputView;
        private readonly IUpdateOutputView _mainMenuOutputView;
        private readonly Client _client;

        public string _serverTable =string.Empty;

        private OutputWrapper outputWrapper;

        public ProtocolAction(Dictionary<ClientView, IView> views, Client client)
        {
            _client = client;
            _views = views;
            _serverTableView = views[ClientView.ServerTable] as IUpdateOutputView;
            _helpOutputView = views[ClientView.HelpOutput] as IUpdateOutputView;
            _smallUpdateOutputView = views[ClientView.InfoOutput] as IUpdateOutputView;          
            _boardOutputView = views[ClientView.Board] as IUpdateOutputView;
            _errorView = views[ClientView.Error] as IErrorView;
            _gameInfoOutputView = views[ClientView.GameInfo] as IUpdateOutputView;
            _turnInfoOutputView = views[ClientView.TurnInfo] as IUpdateOutputView;
            _afterTurnOutputView = views[ClientView.AfterTurnOutput] as IUpdateOutputView;
            _mainMenuOutputView = views[ClientView.MenuOutput] as IUpdateOutputView;

            _protocolActions = new Dictionary<ProtocolActionEnum, Action<DataPackage>>
            {
                { ProtocolActionEnum.HelpText, OnHelpTextAction},
                { ProtocolActionEnum.UpdateView, OnUpdateAction},
                { ProtocolActionEnum.Broadcast, OnBroadcastAction },
                { ProtocolActionEnum.Accept, OnAcceptAction },
                { ProtocolActionEnum.Decline, OnDeclineAction }
            
            };

            outputWrapper = new OutputWrapper();
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


        private void OnHelpTextAction(DataPackage data)
        {
            var helpText = MapProtocolToDto<HelpTextDTO>(data);
            
            
            _helpOutputView.SetUpdateContent(helpText._HelpText);
            
        }

        private void OnUpdateAction(DataPackage data)
        {
            var updatedView = MapProtocolToDto<UpdateDTO>(data);
           
            if(!(updatedView._SmallUpdate == null || updatedView._SmallUpdate.Length == 0))
            {
                _smallUpdateOutputView.viewEnabled = true;
                _smallUpdateOutputView.SetUpdateContent(updatedView._SmallUpdate);
            }                  
            if(!(updatedView._boardOutput == null || updatedView._boardOutput.Length == 0))
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
            if(!(updatedView._mainMenuOutput == null || updatedView._mainMenuOutput.Length == 0))
            {
                _mainMenuOutputView.viewEnabled = true;
                _mainMenuOutputView.SetUpdateContent(updatedView._mainMenuOutput);
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
                    _MaxPlayerCount[index], _Servernames[index],(index+1)));

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
            _smallUpdateOutputView.viewEnabled = true;
            _smallUpdateOutputView.SetUpdateContent(accept._SmallUpdate);
        }

        private void OnDeclineAction(DataPackage data)
        {
            _client._InputHandler.Declined = true;
            _client._InputHandler.isConnected = false;
            var decline = MapProtocolToDto<DeclineDTO>(data);
            _smallUpdateOutputView.viewEnabled = true;
            _smallUpdateOutputView.SetUpdateContent(decline._SmallUpdate);
   
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
