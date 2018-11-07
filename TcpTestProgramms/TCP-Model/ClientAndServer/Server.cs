using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TCP_Model.PROTOCOLS.Client;
using TCP_Model.PROTOCOLS.Server;
using TCP_Model.Contracts;
using TCP_Model.Communications;

namespace TCP_Model.ClientAndServer
{

    public class Server
    {
        private const string SERVER_IP = "127.0.0.1";

        private string updateInfo = string.Empty;

        private Dictionary<ProtocolAction, Action<ICommunication, DataPackage>> _protocolActions;

        private List<ICommunication> _communications;
        private TcpListener _listener;

        int x;

        public Server()
        {
            _communications = new List<ICommunication>();

            _protocolActions = new Dictionary<ProtocolAction, Action<ICommunication, DataPackage>>
            {
                { ProtocolAction.RollDice, OnRollDiceAction },
                { ProtocolAction.GetHelp, OnGetHelpAction },
                { ProtocolAction.Connect, OnConnectAction}
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
                    if (communicated == false && x == 0)
                    {
                        x++;
                        
                        var dataPackage = new DataPackage
                        {
                            Header = ProtocolAction.Broadcast,
                            Payload = JsonConvert.SerializeObject(new PROT_BROADCAST
                            {
                                server_ip = "172.22.22.184",
                                server_name = "Eels and Escalators Server_1",
                                player_slot_info = "[0/4] Players"
                            })
                            
                        };
                        dataPackage.Size = dataPackage.ToByteArray().Length;

                        communication.Send(dataPackage);
                    }

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
                    updated_board = "Test: XD",
                    updated_dice_information = "You rolled a 4",
                    updated_turn_information = "Its Player 1's turn",                  
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
                    text = $"Here is your help Player {clientId.client_id}.\n " +
                    $"Commands:\n/rolldice\n/closegame\n"
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        private void OnConnectAction(ICommunication communication, DataPackage data)
        {
           
            var clientId = CreateProtocol<PROT_CONNECT>(data);

            var dataPackage = new DataPackage
            {
                Header = ProtocolAction.Accept,
                Payload = JsonConvert.SerializeObject(new PROT_ACCEPT
                {
                    message = "Congratulations! Your client has been accepted."
                    + "\nGet ready to play a fun round of Eels and Escalators!"
                    + $"\nYou have  been assigned player number {clientId.client_id}."
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            if (clientId.client_id >= 4) //just some non-sense logic to test client declining
                DeclineClient(communication, data, clientId);

            else communication.Send(dataPackage);
        }
        #endregion

        private void DeclineClient(ICommunication communication, DataPackage data, PROT_CONNECT clientId)
        {
            
            var dataPackage = new DataPackage
            {
                Header = ProtocolAction.Decline,
                Payload = JsonConvert.SerializeObject(new PROT_DECLINE
                {
                    message = $"Your connection was declined because the Client ID({clientId.client_id}) is on our Blacklist."
                    
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);

            communication._client.Close();

        }

        #region Static helper functions

        private static T CreateProtocol<T>(DataPackage data) where T : IProtocol
        {
            return JsonConvert.DeserializeObject<T>(data.Payload);
        }

        #endregion
    }
}
