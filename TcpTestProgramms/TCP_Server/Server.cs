﻿using Shared.Communications;
using Shared.Contract;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TCP_Server.Actions;

namespace TCP_Server
{

    public class Server
    {
        private const string SERVER_IP = "172.22.21.132";

        private string _updateInfo = string.Empty;
        private int _x;
        private static TcpClient _client;

        private List<IPAddress> _WhiteList = new List<IPAddress>();
        private List<IPAddress> _Blacklist = new List<IPAddress>();
       
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        private TcpListener _listener;
        private ServerInfo _serverInfo;
        private ServerActions _ActionsHandler;


        public Server(string lobbyname, int maxplayercount)
        {
            _ActionsHandler = new ServerActions();

            _serverInfo = new ServerInfo( lobbyname, maxplayercount);
            _serverInfo._communications = new List<ICommunication>();

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
                        _ActionsHandler.ExecuteDataActionFor(communication, data);
                        communicated = true;
                    }
                });

                if (!communicated)
                    Thread.Sleep(1);
            }

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

       /* #region Accept and Decline
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
        */
       
    }
}
