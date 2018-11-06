using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TCP_Model.PROTOCOLS.Server;

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
                { ProtocolAction.GetHelp, OnGetHelpAction }
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
            //var protocol = CreateProtocol<PROT_ROLLDICE>(data);
            
            var dataPackage = new DataPackage
            {

                Header = ProtocolAction.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    Updated_Board = "Test: XD",
                    Updated_DiceInformation = "You rolled a 4",
                    Updated_TurnInformation = "Its Player 1's turn",                  
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        private void OnGetHelpAction(ICommunication communication, DataPackage data)
        {

            var clientId = CreateProtocol<PROT_HELP>(data);

            var dataPackage = new DataPackage
            {
                Header = ProtocolAction.HelpText,
                Payload = JsonConvert.SerializeObject(new PROT_HELPTEXT
                {
                    help_text = $"Here is your help Player {clientId.Client_ID}.\n " +
                    $"Commands:{Environment.NewLine}/rolldice{Environment.NewLine}/closegame"
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

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
