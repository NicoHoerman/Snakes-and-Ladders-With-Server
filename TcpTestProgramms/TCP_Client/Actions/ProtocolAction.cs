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

        private readonly IServerTableView _serverTableView;
        private readonly IHelpOutputView _helpOutputView;
        private readonly IUpdateOutputView _infoOutputView;
        private readonly IUpdateOutputView _gameOutputView;

        public string _serverTable =string.Empty;

        private OutputWrapper outputWrapper;

        public ProtocolAction(Dictionary<ClientView, IView> views)
        {
            _views = views;
            _serverTableView = views[ClientView.ServerTable] as IServerTableView;
            _helpOutputView = views[ClientView.HelpOutput] as IHelpOutputView;
            _infoOutputView = views[ClientView.InfoOutput] as IUpdateOutputView;
            _gameOutputView = views[ClientView.Game] as IUpdateOutputView;

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
            
            
            _helpOutputView.SetHelp(helpText._HelpText);
            
        }

        private void OnUpdateAction(DataPackage data)
        {
            var updatedView = MapProtocolToDto<UpdateDTO>(data);
            if (updatedView._GameViewUpdate == null)
                return;
            else if(updatedView._GameViewUpdate.Length != 0)
            {
                _gameOutputView.viewEnabled = true;
                _gameOutputView.SetUpdateContent(updatedView._GameViewUpdate);
            }
            if(updatedView._SmallUpdate == null)
                return;
            if(updatedView._SmallUpdate.Length != 0)
            {
                _infoOutputView.viewEnabled = true;
                _infoOutputView.SetUpdateContent(updatedView._SmallUpdate);
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
            _serverTableView.SetServerTableContent(_serverTable);
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
            var decline = MapProtocolToDto<DeclineDTO>(data);
            _infoOutputView.viewEnabled = true;
            _infoOutputView.SetUpdateContent(decline._SmallUpdate);
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
