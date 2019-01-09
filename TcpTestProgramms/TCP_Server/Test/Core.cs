﻿using EandE_ServerModel.EandE.GameAndLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public StateMachine stateMachine;
        public static StateEnum state;
        public ValidationSystem validationSystem;
        public static ValidationEnum status;
        public static ClientConnectionStatus _ConnectionStatus;

        public Core()
        {
            game = new Game();
            serverInfo = new ServerInfo();

            udpserver = new UdpBroadcast(serverInfo);
            stateMachine = new StateMachine(serverInfo);

            server = new Server(game, serverInfo, stateMachine, validationSystem, _disconnectionHandler);
            _connectionHandler = new ClientConnection(serverInfo);
            _disconnectionHandler = new ClientDisconnection(game, serverInfo);
            validationSystem = new ValidationSystem(serverInfo,_disconnectionHandler,_connectionHandler);
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
