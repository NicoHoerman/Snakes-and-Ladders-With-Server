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

namespace TCP_Client.Actions
{
    public class ProtocolAction
    {
        public Dictionary<ProtocolActionEnum, Action<DataPackage>> _protocolActions;
        public Dictionary<int, BroadcastDTO> _serverDictionary = new Dictionary<int, BroadcastDTO>();

        private OutputWrapper outputWrapper;
        public string _serverTable =string.Empty;
        public string _UpdatedView = string.Empty;

        public ProtocolAction()
        {
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

            Console.Write("Received help text: " + helpText._Text);
        }

        private void OnUpdateAction(DataPackage data)
        {
            var updatedView = MapProtocolToDto<UpdateDTO>(data);
            _UpdatedView = updatedView._Updated_View;
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
            //      Server  Player  
            //
            //  1   XD      [0/4]
            //  2   LuL     [1/2]
        }

        private void OnAcceptAction(DataPackage data)
        {
            var accept = MapProtocolToDto<AcceptDTO>(data);

            throw new NotImplementedException();
            
        }

        private void OnDeclineAction(DataPackage data)
        {
            var decline = MapProtocolToDto<DeclineDTO>(data);

            throw new NotImplementedException();
            Console.WriteLine(decline._Message);

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
