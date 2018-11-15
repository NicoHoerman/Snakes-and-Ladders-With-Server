
using EandE_ServerModel.ServerModel.Contracts;
using EandE_ServerModel.ServerModel.PROTOCOLS.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TCP_Model.ServerModel;

namespace EandE_ServerModel.ServerModel.ProtocolActionStuff
{
    public class ProtocolAction
    {
        public Dictionary<ProtocolActionEnum, Action<DataPackage>> _protocolActions;
        public Dictionary<int, PROT_BROADCAST> _serverDictionary;

        private OutputWrapper outputWrapper;
        public string _serverTable =string.Empty; 


        public ProtocolAction()
        {
            _serverDictionary = new Dictionary<int, PROT_BROADCAST>();

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

        public PROT_BROADCAST GetServer(int key) => _serverDictionary[key];

        #region Protocol actions


        private void OnHelpTextAction(DataPackage data)
        {
            var helpText = CreateProtocol<PROT_HELPTEXT>(data);

            Console.Write("Received help text: " + helpText._Text);
        }

        private void OnUpdateAction(DataPackage data)
        {
            var updatedView = CreateProtocol<PROT_UPDATE>(data);

            Console.WriteLine("Received update: " + updatedView._Updated_board + "\n" + updatedView._Updated_dice_information
                + "\n" + updatedView._Updated_turn_information);
        }

        private List<string> _ServerIps = new List<string>(); 
        private string[] _Servernames = new string[100];
        private int[] _MaxPlayerCount = new int[100];
        private int[] _CurrentPlayerCount = new int[100];
        private int keyIndex = 0;


        private void OnBroadcastAction(DataPackage data)
        {
            var broadcast = CreateProtocol<PROT_BROADCAST>(data);

            if (_ServerIps.Contains(broadcast._Server_ip))
            {
                var servernumber = _ServerIps.IndexOf(broadcast._Server_ip);
                _Servernames[servernumber] = broadcast._Server_name;
                _MaxPlayerCount[servernumber] = broadcast._MaxPlayerCount;
                _CurrentPlayerCount[servernumber] = broadcast._CurrentPlayerCount;
            }
            else
            {
                _ServerIps.Add(broadcast._Server_ip);

                _serverDictionary.Add(keyIndex, broadcast);

                _Servernames[keyIndex] = broadcast._Server_name;
                _MaxPlayerCount[keyIndex] = broadcast._MaxPlayerCount;
                _CurrentPlayerCount[keyIndex] = broadcast._CurrentPlayerCount;
            }

            var outputFormat = new StringBuilder();

            for (int index = 0; index < _serverDictionary.Count; index++)
                outputFormat.Append(string.Format("{3,2}  [{0,1}/{1,1}]   {2,20}\n", _CurrentPlayerCount[index],
                    _MaxPlayerCount[index], _Servernames[index],(index+1)));

            _serverTable = outputFormat.ToString();
            keyIndex++;
            //      Server  Player  
            //
            //      XD      [0/4]
            //      LuL     [1/2]
        }

        private void OnAcceptAction(DataPackage data)
        {

            throw new NotImplementedException();
            /*
            if (i == 0)
            {
                var accept = CreateProtocol<PROT_ACCEPT>(data);

                Console.WriteLine(accept._Message);
            }
            else Console.WriteLine("Error: You are already connected.");

            _game.Init();

            i++;
            */
        }

        private void OnDeclineAction(DataPackage data)
        {
            var decline = CreateProtocol<PROT_DECLINE>(data);

            Console.WriteLine(decline._Message);

        }
        #endregion

        #region Static helper functions

        private static T CreateProtocol<T>(DataPackage data) where T : IProtocol
        {
            //macht aus einem Objekt String ein wieder das urpsrüngliche Objekt Protokoll
            return JsonConvert.DeserializeObject<T>(data.Payload);
        }

        #endregion
    }
}
