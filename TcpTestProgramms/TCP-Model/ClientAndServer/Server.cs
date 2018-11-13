﻿using Newtonsoft.Json;
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

        private string _updateInfo = string.Empty;
        private int _x;

        //private List<IPAddress> _WhiteList = new List<IPAddress>();
        //private List<IPAddress> _Blacklist = new List<IPAddress>();

        private Dictionary<ProtocolAction, Action<ICommunication, DataPackage>> _protocolActions;

       
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        private TcpListener _listener;
        private ServerInfo _serverInfo;


        public Server(string lobbyname, int maxplayercount)
        {
            _serverInfo = new ServerInfo( lobbyname, maxplayercount);
            _serverInfo._communications = new List<ICommunication>();

            _protocolActions = new Dictionary<ProtocolAction, Action<ICommunication, DataPackage>>
            {
                { ProtocolAction.RollDice, OnRollDiceAction },
                { ProtocolAction.GetHelp, OnGetHelpAction },
                { ProtocolAction.Connect, OnConnectAction}
            };

            _listener = new TcpListener(IPAddress.Parse(SERVER_IP), 8080);
            DoBeginAcceptTcpClient(_listener);
        }


        private void CheckForUpdates()
        {
            while (true)
            {
                

                var communicated = false;

                _serverInfo._communications.ForEach(communication =>
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

        public static void DoBeginAcceptTcpClient(TcpListener listener)
        {
            listener.Start();
            Console.WriteLine("Waiting for a connection...");

            listener.BeginAcceptTcpClient(
                new AsyncCallback(DoAcceptTcpClientCallback),
                listener);

            tcpClientConnected.WaitOne();
        }

        public static void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            TcpClient client = listener.EndAcceptTcpClient(ar);
            _serverInfo._communications.Add(
            new TcpCommunication(client));

            Console.WriteLine("Client connected completed");
            tcpClientConnected.Set();
        }

        public void isLobbyComplete()
        {
            if (_serverInfo._communications.Count == _serverInfo._MaxPlayerCount)
                _listener.Stop();
        }

        #region Protocol actions

        private void OnRollDiceAction(ICommunication communication, DataPackage data)
        {
            
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
