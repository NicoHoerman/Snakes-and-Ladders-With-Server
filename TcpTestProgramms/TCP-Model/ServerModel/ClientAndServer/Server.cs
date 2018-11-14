using EandE_ServerModel.ServerModel.Communications;
using EandE_ServerModel.ServerModel.Contracts;
using EandE_ServerModel.ServerModel.ProtocolActionStuff;
using EandE_ServerModel.ServerModel.PROTOCOLS.Client;
using EandE_ServerModel.ServerModel.PROTOCOLS.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EandE_ServerModel.ServerModel.ClientAndServer
{

    public class Server
    {
        private const string SERVER_IP = "172.22.22.153";

        private string _updateInfo = string.Empty;
        private int _x;
        private static TcpClient _client;

        private List<IPAddress> _WhiteList = new List<IPAddress>();
        private List<IPAddress> _Blacklist = new List<IPAddress>();

        private Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>> _protocolActions;

       
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        private TcpListener _listener;
        private ServerInfo _serverInfo;


        public Server(string lobbyname, int maxplayercount)
        {
            _serverInfo = new ServerInfo( lobbyname, maxplayercount);
            _serverInfo._communications = new List<ICommunication>();

            _protocolActions = new Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>>
            {
                { ProtocolActionEnum.RollDice, OnRollDiceAction },
                { ProtocolActionEnum.GetHelp, OnGetHelpAction }
                //{ ProtocolAction.Connect, OnConnectAction}
            };

            _listener = new TcpListener(IPAddress.Parse(SERVER_IP), 8080);
            CLientConnection(_listener);
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

        public void CLientConnection(TcpListener listener)
        {
            _listener.Start();

            while (!isLobbyComplete())
            {
                DoBeginAcceptTcpClient(listener);
                if (_client != null)
                    AddCommunication(_client);
               Thread.Sleep(1000);
            }
        }

        public static void DoBeginAcceptTcpClient(TcpListener listener)
        {

            listener.BeginAcceptTcpClient(
                new AsyncCallback(DoAcceptTcpClientCallback),
                listener);

            tcpClientConnected.WaitOne();
        }

        public static void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            TcpClient client = listener.EndAcceptTcpClient(ar);
            _client = client;
            
            Console.WriteLine("Client connected completed");
            tcpClientConnected.Set();
        }

        public bool isLobbyComplete()
        {
            if (_serverInfo._communications.Count == _serverInfo._MaxPlayerCount)
            {
                _listener.Stop();
                return true;
            }
            else
                return false;
        }

        public void AddCommunication(TcpClient client)
        {
            _serverInfo._communications.Add(
                new TcpCommunication(client));
            _serverInfo.PrintPlayerIP();
            _client = null;
        } 

        #region Protocol actions

        private void OnRollDiceAction(ICommunication communication, DataPackage data)
        {
            
            var dataPackage = new DataPackage
            {

                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _Updated_board = "Test: XD",
                    _Updated_dice_information = "You rolled a 4",
                    _Updated_turn_information = "Its Player 1's turn",                  
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        private void OnGetHelpAction(ICommunication communication, DataPackage data)
        {

            var clientId = CreateProtocol<PROT_HELPTEXT>(data);

            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.HelpText,
                Payload = JsonConvert.SerializeObject(new PROT_HELPTEXT
                {
                    _Text = $"Here is your help Player {clientId._Text}.\n " +
                    $"Commands:\n/rolldice\n/closegame\n"
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        /*private void OnConnectAction(ICommunication communication, DataPackage data)
        {
            var clientId = CreateProtocol<PROT_CONNECT>(data);

            if (_WhiteList.Contains(communication._clientId))
                AcceptClient(communication, data, clientId);

            else if (_Blacklist.Contains(communication._clientIP))
                DeclineClient(communication, data, clientId);
        }*/
        #endregion

        #region Accept and Decline
        private void DeclineClient(ICommunication communication, DataPackage data, PROT_CONNECT clientId)
        {
            
            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Decline,
                Payload = JsonConvert.SerializeObject(new PROT_DECLINE
                {
                    _Message = $"Your connection was declined because the Client ID({clientId._Client_id}) is on our Blacklist."
                    
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);

            communication._client.Close();

        }

        private void AcceptClient(ICommunication communication, DataPackage data, PROT_CONNECT clientId)
        {

            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Accept,
                Payload = JsonConvert.SerializeObject(new PROT_ACCEPT
                {
                    _Message = "Congratulations! Your client has been accepted."
                    + "\nGet ready to play a fun round of Eels and Escalators!"
                    + $"\nYou have  been assigned player number {clientId._Client_id}."
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
