using EandE_ServerModel.EandE.GameAndLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCP_Server.Actions;
using TCP_Server.Enum;
using TCP_Server.UDP;

namespace TCP_Server.Test
{
    public class Core
    {
        public UdpBroadcast _udpserver;
        public Server _server;
        public ClientConnection _connectionHandler;
        public ClientDisconnection _disconnectionHandler;
        public Game _game;
        public ServerInfo _serverInfo;
        public ServerActions _actionsHandler;

        public StateMachine _stateMachine;
        public ValidationSystem _validationSystem;
        public ServerDataPackageProvider _dataPackageProvider;
        public static StateEnum State;
        public static ValidationEnum ValidationStatus;
        public static ClientConnectionStatus ConnectionStatus;

        public Core()
        {
            _game = new Game();			
            _serverInfo = new ServerInfo();
            _udpserver = new UdpBroadcast(_serverInfo);

            _dataPackageProvider = new ServerDataPackageProvider(_serverInfo,_game);
            _connectionHandler = new ClientConnection(_serverInfo,_dataPackageProvider);
            _disconnectionHandler = new ClientDisconnection(_game, _serverInfo,_dataPackageProvider);

            _actionsHandler = new ServerActions(_serverInfo, _game, _disconnectionHandler, _dataPackageProvider);
            _stateMachine = new StateMachine(_serverInfo,_actionsHandler, _game);
            _validationSystem = new ValidationSystem(_serverInfo,_disconnectionHandler,_connectionHandler,_dataPackageProvider, _actionsHandler);
            _server = new Server(_actionsHandler, _serverInfo, _stateMachine, _validationSystem, _disconnectionHandler);
        }

        public void Start()
        {
            Console.WriteLine("Listening for Clients...");

            var backgroundworker = new BackgroundWorker();
            backgroundworker.DoWork += (obj, ea) => _udpserver.RunUdpServer();
            backgroundworker.RunWorkerAsync();

            Console.WriteLine("Waiting for players ");
            _server.Run();

        }

    }
}
