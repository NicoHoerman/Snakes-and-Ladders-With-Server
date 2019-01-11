﻿using EandE_ServerModel.EandE.GameAndLogic;
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
        public UdpBroadcast udpserver;
        public Server server;
        public ClientConnection _connectionHandler;
        public ClientDisconnection _disconnectionHandler;
        public Game game;
        public ServerInfo serverInfo;
        public ServerActions _ActionsHandler;

        public StateMachine stateMachine;
        public ValidationSystem validationSystem;
        public ServerDataPackageProvider _dataPackageProvider;
        public static StateEnum State;
        public static ValidationEnum ValidationStatus;
        public static ClientConnectionStatus ConnectionStatus;

        public Core()
        {
            game = new Game();
            serverInfo = new ServerInfo();

            _dataPackageProvider = new ServerDataPackageProvider(serverInfo,game);

            udpserver = new UdpBroadcast(serverInfo);

            _connectionHandler = new ClientConnection(serverInfo,_dataPackageProvider);
            _disconnectionHandler = new ClientDisconnection(game, serverInfo,_dataPackageProvider);

            _ActionsHandler = new ServerActions(serverInfo, game, _disconnectionHandler);
            stateMachine = new StateMachine(serverInfo,_ActionsHandler, game);
            validationSystem = new ValidationSystem(serverInfo,_disconnectionHandler,_connectionHandler,_dataPackageProvider, _ActionsHandler);
            server = new Server(_ActionsHandler, serverInfo, stateMachine, validationSystem, _disconnectionHandler);
        }

        public void Start()
        {
            Console.WriteLine("Listening for Clients...");

            var backgroundworker = new BackgroundWorker();
            backgroundworker.DoWork += (obj, ea) => udpserver.RunUdpServer();
            backgroundworker.RunWorkerAsync();

            Console.WriteLine("Waiting for players ");
            server.Run();

        }

    }
}
