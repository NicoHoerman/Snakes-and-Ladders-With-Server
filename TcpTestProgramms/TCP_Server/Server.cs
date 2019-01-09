using EandE_ServerModel.EandE.GameAndLogic;
using Shared.Communications;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TCP_Server.Actions;
using TCP_Server.Test;

namespace TCP_Server
{
    public class Server
    {
        private const string SERVER_IP_WLAN_NICO = "";
        private const string SERVER_IP_LAN_NICO = "172.22.23.88";
        private const string SERVER_IP_LAN_LEON = "172.22.23.87";
        private const string SERVER_IP_NETWORK = "194.205.205.2";
        
        private bool isRunning;

        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        
        private TcpListener _listener;
        private ServerInfo _serverInfo;
        private ServerActions _ActionsHandler;
        public ClientDisconnection _DisconnectionHandler;
        public Game _game;

        public PackageQueue _queue;
        public PackageProcessing _process;

        public StateMachine _stateMachine;
        public ValidationSystem _validationSystem;

        public Server(Game game,ServerInfo serverInfo,StateMachine stateMachine,
            ValidationSystem validationSystem,ClientDisconnection disconnectionHandler)
        {
            _stateMachine = stateMachine;
            var bwStateMachine = new BackgroundWorker();
            bwStateMachine.DoWork += (obj, ea) => stateMachine.Start();
            bwStateMachine.RunWorkerAsync();

            _validationSystem = validationSystem;
            var bwValidationSystem = new BackgroundWorker();
            bwValidationSystem.DoWork += (obj, ea) => validationSystem.Start();
            bwValidationSystem.RunWorkerAsync();
            
            _serverInfo = serverInfo;
            _game = game;
            _ActionsHandler = new ServerActions(_serverInfo,_game);
            _DisconnectionHandler = disconnectionHandler;

            _queue = new PackageQueue();
            _process = new PackageProcessing(_queue, _ActionsHandler);

            _listener = new TcpListener(IPAddress.Parse(SERVER_IP_LAN_NICO), 8080);
        }

        public void StartListening(TcpListener listener)
        {
            _listener.Start();

            while (isRunning)
            {
                DoBeginAcceptTcpClient(listener);
            }
            Thread.Sleep(1000);
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
            var listener = (TcpListener)ar.AsyncState;
            var client = listener.EndAcceptTcpClient(ar);
            AddCommunication(client);
            tcpClientConnected.Set();
            Core.status = ValidationEnum.ValidationState;
        }

        public void AddCommunication(TcpClient client)
        {
            _serverInfo._communications.Add(
                new TcpCommunication(client));

            if (_serverInfo._communications.Count == 1)
                _serverInfo._communications[0].IsMaster = true;

            _serverInfo.PrintPlayerIP();
        } 

        public void Run()
        {
            isRunning = true;

            var backgroundworkerUpdate = new BackgroundWorker();
            backgroundworkerUpdate.DoWork += (obj, ea) => CheckForUpdates();
            backgroundworkerUpdate.RunWorkerAsync();

            var backgroundworkerConnection = new BackgroundWorker();
            backgroundworkerConnection.DoWork += (obj, ea) => StartListening(_listener);
            backgroundworkerConnection.RunWorkerAsync();

            var backgroundworkerGame = new BackgroundWorker();
            backgroundworkerGame.DoWork += (obj, ea) => RunGame();
            backgroundworkerGame.RunWorkerAsync();

            while (isRunning)
            { }
        }

        private void CheckForUpdates()
        {
            while (isRunning)
            {
                _serverInfo.communicationsToRemove.Clear();
                _serverInfo._communications.ForEach(communication =>
                {
                    if (!communication.IsConnected)
                    {
                        communication.Stop();
                        _serverInfo.communicationsToRemove.Add(communication);
                    }
                    else if (communication.IsDataAvailable())
                    {
                        var data = communication.Receive();
                        var communicationPackage = new CommunicationPackage(communication,data);
                        _queue.Push(communicationPackage);

                        // Old Method
                        //Task.Run(() => _ActionsHandler.ExecuteDataActionFor(communication, data));
                    }
                });

                // All elements that lost conenction!
                if(_serverInfo.communicationsToRemove.Count > 0)
                {
                    RemoveFromLobby();
                }

                Thread.Sleep(1);
            }
        }

        
        private void ShutdownServer(string input)
        {
                _game.State.SetInput("/closegame");
                isRunning = false; 
        }

        private void RunGame()
        {
            while (isRunning)
            {
                _game.Init();
            }
        }

    }
}
