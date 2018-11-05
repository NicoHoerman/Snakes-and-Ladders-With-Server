using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCP_Model
{

    public class Server
    {
        private const string SERVER_IP = "127.0.0.1";

        private string updateInfo = string.Empty;

        private Dictionary<ProtocolAction, Action<ICommunication, DataPackage>> _protocolActions;

        private List<ICommunication> _communications;
        private TcpListener _listener;
        
        public Server()
        {
            _communications = new List<ICommunication>();

            _protocolActions = new Dictionary<ProtocolAction, Action<ICommunication, DataPackage>>
            {
                { ProtocolAction.RollDice, OnRollDiceAction },
            };

            _listener = new TcpListener(IPAddress.Parse(SERVER_IP), 8080);
            _listener.Start();

            _communications.Add(new TcpCommunication(_listener.AcceptTcpClient()));
        }


        private void CheckForUpdates()
        {
            while (true)
            {
                var communicated = false;
                _communications.ForEach(communication =>
                {
                    if (communication.IsDataAvailable())
                    {
                        var data = communication.Receive();
                        ExecuteDataActionFor(communication, data);
                        communicated = true;
                    }
                });

                if (!communicated)
                    Thread.Sleep(1);
            }

        }

        private void ExecuteDataActionFor(ICommunication communication, DataPackage data)
        {
            if (_protocolActions.TryGetValue(data.Header, out var protocolAction) == false)
                throw new InvalidOperationException("Invalid communication");

            protocolAction(communication, data);
        }

        public void Run()
        {
            

            CheckForUpdates();

        }

        #region Protocol actions

        private void OnRollDiceAction(ICommunication communication, DataPackage data)
        {
            var protocol = CreateProtocol<PROT_ROLLDICE>(data);
            var dataPackage = new DataPackage
            {

                Header = ProtocolAction.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    Updated_Board = "Test: XD",
                    Updated_DiceInformation = "Test : 1",
                    Updated_TurnInformation = "",
                    Game_Finished = true
                })
            };
            communication.Send(dataPackage);
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
