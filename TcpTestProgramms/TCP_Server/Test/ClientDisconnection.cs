﻿using EandE_ServerModel.EandE.GameAndLogic;
using Shared.Communications;
using Shared.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCP_Server.UDP;

namespace TCP_Server.Test
{
    public class ClientDisconnection
    {
        public Game _game;
        public ServerInfo _serverInfo;
        private DataPackageProvider _dataPackageProvider;

        public ClientDisconnection(Game game, ServerInfo serverInfo,DataPackageProvider dataPackageProvider)
        {
            _game = game;
            _serverInfo = serverInfo;
            _dataPackageProvider = dataPackageProvider;
        }

        public void Execute(ICommunication communication )
        {
            DataPackage declinedInfoPackage = _dataPackageProvider.GetPackage("DeclinedInfo");
            communication.Send(declinedInfoPackage);
            DisconnectClient();
            
            DataPackage declineUpdatePackage = _dataPackageProvider.GetPackage("DeclineUpdate");
            for (int i = 0; i <= _serverInfo._communications.Count - 1; i++)
            {
                _serverInfo._communications[i].Send(declineUpdatePackage);
            }
        }

        public void RemoveFromLobby()
        {
            _serverInfo.communicationsToRemove.ForEach(x => _serverInfo.lobbylist[0]._CurrentPlayerCount--);
            RemoveFromList();

            if (_game.isRunning)
                _game.State.SetInput("/closegame");
        }

        private void RemoveFromList()
        {
            _serverInfo.communicationsToRemove.ForEach(x => _serverInfo._communications.Remove(x));
        }

        public void DisconnectClient()
        {
            var currentCommunication = _serverInfo._communications.Last();
            currentCommunication.Stop();

            _serverInfo.communicationsToRemove.Add(currentCommunication);

            RemoveFromList();
        }
    }
}
