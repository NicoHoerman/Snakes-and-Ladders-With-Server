using EandE_ServerModel.EandE.GameAndLogic;
using Shared.Communications;
using System;
using System.ComponentModel;
using System.Linq;
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
        
        private bool _isRunning;

        public static ManualResetEvent TcpClientConnected = new ManualResetEvent(false);
        
        private TcpListener _listener;
        private ServerInfo _serverInfo;
        public ServerActions _actionsHandler;
        public ClientDisconnection _disconnectionHandler;

        public PackageQueue _queue;
        public PackageProcessing _process;

        public StateMachine _stateMachine;
        public ValidationSystem _validationSystem;

        public Server(ServerActions actionHandler,ServerInfo serverInfo,StateMachine stateMachine,
            ValidationSystem validationSystem,ClientDisconnection disconnectionHandler)
        {
            _stateMachine = stateMachine;
            var bwStateMachine = new BackgroundWorker();
            bwStateMachine.DoWork += (obj, ea) => _stateMachine.Start();
            bwStateMachine.RunWorkerAsync();

            _validationSystem = validationSystem;
            var bwValidationSystem = new BackgroundWorker();
            bwValidationSystem.DoWork += (obj, ea) => _validationSystem.Start();
            bwValidationSystem.RunWorkerAsync();
            
            _serverInfo = serverInfo;
            _actionsHandler = actionHandler;
            _disconnectionHandler = disconnectionHandler;

            _queue = new PackageQueue();
            _process = new PackageProcessing(_queue, _actionsHandler);

            _listener = new TcpListener(IPAddress.Parse(SERVER_IP_LAN_LEON), 8080);
        }

        public void StartListening(TcpListener listener)
        {
            _listener.Start();

            while (_isRunning)
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

            TcpClientConnected.WaitOne();
            TcpClientConnected.Reset();
        }

        public void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            var listener = (TcpListener)ar.AsyncState;
            var client = listener.EndAcceptTcpClient(ar);
            AddCommunication(client);
            TcpClientConnected.Set();
            Core.ValidationStatus = ValidationEnum.ValidationState;
        }

        public void AddCommunication(TcpClient client)
        {
            _serverInfo._communications.Add(
                new TcpCommunication(client));

            _validationSystem.currentcommunication = _serverInfo._communications.Last();

            if (_serverInfo._communications.Count == 1)
                _serverInfo._communications[0].IsMaster = true;

            _serverInfo.PrintPlayerIP();
        } 

        public void Run()
        {
            _isRunning = true;

            var backgroundworkerUpdate = new BackgroundWorker();
            backgroundworkerUpdate.DoWork += (obj, ea) => CheckForUpdates();
            backgroundworkerUpdate.RunWorkerAsync();

            var backgroundworkerConnection = new BackgroundWorker();
            backgroundworkerConnection.DoWork += (obj, ea) => StartListening(_listener);
            backgroundworkerConnection.RunWorkerAsync();

            while (_isRunning)
            { ShutdownServer(Console.ReadLine()); }
        }

        private void CheckForUpdates()
        {
            while (_isRunning)
            {
                _serverInfo._communicationsToRemove.Clear();
                _serverInfo._communications.ForEach(communication =>
                {
                    if (!communication.IsConnected)
                    {
                        communication.Stop();
                        _serverInfo._communicationsToRemove.Add(communication);
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

                if(_serverInfo._communicationsToRemove.Count > 0)
                    _disconnectionHandler.RemoveFromLobby();
                Thread.Sleep(1);
            }
        }
        
        private void ShutdownServer(string input)
        {
            if(input == "shutdown")
            {
                Core.State = StateEnum.ServerEndingState;
                _isRunning = false;
            }
        }
    }
}
