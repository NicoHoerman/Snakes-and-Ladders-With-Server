using EandE_ServerModel.EandE.GameAndLogic;
using Shared.Communications;
using Shared.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TCP_Server.Actions;
using TCP_Server.Enum;
using TCP_Server.UDP;

namespace TCP_Server
{
    public class Server
    {
        private const string SERVER_IP_WLAN_NICO = "";
        private const string SERVER_IP_LAN_NICO = "172.22.23.88";
        private const string SERVER_IP_LAN_LEON = "172.22.23.87";
        private const string SERVER_IP_NETWORK = "194.205.205.2";
        
        private bool isRunning;
        public List<ICommunication> communicationsToRemove = new List<ICommunication>();
        public List<DataPackage> dataQueue = new List<DataPackage>();
        public List<ICommunication> communicationsQueue = new List<ICommunication>();
       
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        
        private TcpListener _listener;
        private ServerInfo _serverInfo;
        private ServerActions _ActionsHandler;
        private UdpBroadcast _udpServer;
        private TcpClient _client;
        public Game _game;

        public Server(ServerInfo serverInfo,UdpBroadcast udpBroadcast)
        {
            _game = new Game();
            _serverInfo = serverInfo;
            _udpServer = udpBroadcast;
            _ActionsHandler = new ServerActions(_serverInfo,this,_game,communicationsToRemove);

            _serverInfo._communications = new List<ICommunication>();

            _listener = new TcpListener(IPAddress.Parse(SERVER_IP_LAN_LEON), 8080);
        }

        public void CLientConnection(TcpListener listener)
        {
            _listener.Start();

            while (isRunning)
            {
                DoBeginAcceptTcpClient(listener);
                if (_client != null)
                {
                    
                    AddCommunication(_client);
                    

                    if (!isLobbyComplete())
                    {
                        _ActionsHandler._ConnectionStatus = ClientConnectionAttempt.Accepted;
                        ServerActions.verificationVariableSet.Set();
                        _serverInfo._CurrentPlayerCount++;
                        _udpServer.SetBroadcastMsg(_serverInfo);
                    }
                    else if (isLobbyComplete())
                    {
                        _ActionsHandler._ConnectionStatus = ClientConnectionAttempt.Declined;
                        ServerActions.verificationVariableSet.Set();

                        ServerActions.MessageSent.WaitOne();
                        ServerActions.MessageSent.Reset();
                        var currentCommunication = _serverInfo._communications.Last();
                        currentCommunication.Stop();
                        
                        communicationsToRemove.Add(currentCommunication);

                        RemoveFromList();
                    }
                    _client = null;
                }
                Thread.Sleep(1000);
            }
        }

        public void DoBeginAcceptTcpClient(TcpListener listener)
        {

            listener.BeginAcceptTcpClient(
                new AsyncCallback(DoAcceptTcpClientCallback),
                listener);

            tcpClientConnected.WaitOne();
            tcpClientConnected.Reset();
        }

        public void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            _client = listener.EndAcceptTcpClient(ar);
            
            //Conected
            Console.WriteLine("Client succesfully connected");
            
            tcpClientConnected.Set();
        }

        public bool isLobbyComplete()
        {
            if (_serverInfo._CurrentPlayerCount == _serverInfo._MaxPlayerCount)
            {
                return true;
            }
            else
                return false;
        }

        public void AddCommunication(TcpClient client)
        {
            _serverInfo._communications.Add(
                new TcpCommunication(client));

            if (_serverInfo._communications.Count == 1)
                _serverInfo._communications[0].IsMaster = true;

            _serverInfo.PrintPlayerIP();
            _client = null;
        } 

        public void Run()
        {
            isRunning = true;

            var backgroundworkerUpdate = new BackgroundWorker();

            backgroundworkerUpdate.DoWork += (obj, ea) => CheckForUpdates();
            backgroundworkerUpdate.RunWorkerAsync();

            var backgroundworkerConnection = new BackgroundWorker();

            backgroundworkerConnection.DoWork += (obj, ea) => CLientConnection(_listener);
            backgroundworkerConnection.RunWorkerAsync();

            var backgroundworkerGame = new BackgroundWorker();

            backgroundworkerGame.DoWork += (obj, ea) => _game.Init();
            backgroundworkerGame.RunWorkerAsync();

            var backgroundworkerDataExe = new BackgroundWorker();

            backgroundworkerDataExe.DoWork += (obj, ea) => ExecuteData();
            //backgroundworkerDataExe.RunWorkerAsync();

            while (isRunning)
            {
                var input = Console.ReadLine();
                ShutdownServer(input);
            }

        }

        private void CheckForUpdates()
        {
            
            while (isRunning)
            {
                communicationsToRemove.Clear();
                _serverInfo._communications.ForEach(communication =>
                {
                    if (!communication.IsConnected)
                    {
                        communication.Stop();
                        communicationsToRemove.Add(communication);
                    }
                    else
                    {
                        if (communication.IsDataAvailable())
                        {
                            if (!(_game.State.ToString() == "EandE_ServerModel.EandE.States.GameFinishedState"))
                            {
                                var data = communication.Receive();
                                communicationsQueue.Add(communication);
                                dataQueue.Add(data);
                                Task.Run(() => _ActionsHandler.ExecuteDataActionFor(communication, data));
                            }
                        }
                    }
                });

                // All elements that lost conenction!
                if(communicationsToRemove.Count > 0)
                {
                    RemoveFromLobby();
                }

                Thread.Sleep(1);
            }
        }


        
        public void RemoveFromLobby()
        {
            communicationsToRemove.ForEach(x => _serverInfo._CurrentPlayerCount--);
            _udpServer.SetBroadcastMsg(_serverInfo);
            RemoveFromList();
        }

        public void RemoveFromList()
        {
            communicationsToRemove.ForEach(x => _serverInfo._communications.Remove(x));
        }

        private void ShutdownServer(string input)
        {
            if (input == "x")
            {
                _game.State.SetInput("/closegame");
                isRunning = false; 
            }

        }

        private void ExecuteData()
        {
            while (isRunning)
            {
                //var dataAvailable = dataQueue.Any() && communicationsQueue.Any();
                if (!(dataQueue.Count == 0 & communicationsQueue.Count == 0))
                {

                    _ActionsHandler.ExecuteDataActionFor(communicationsQueue[0], dataQueue.First());
                    communicationsQueue.RemoveAt(0);
                    dataQueue.RemoveAt(0);
                }
            }
        }

    }
}
