using EandE_ServerModel.EandE.GameAndLogic;
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

        public ClientDisconnection(Game game, ServerInfo serverInfo)
        {
            _game = game;
            _serverInfo = serverInfo;
        }

        public void Execute()
        {

        }

        public void RemoveFromLobby()
        {
            _serverInfo.communicationsToRemove.ForEach(x => _serverInfo.lobbylist[0]._CurrentPlayerCount--);
            RemoveFromList();

            if (_game.isRunning)
                _game.State.SetInput("/closegame");
        }

        public void RemoveFromList()
        {
            _serverInfo.communicationsToRemove.ForEach(x => _serverInfo._communications.Remove(x));
        }

        private void DisconnectClient()
        {
            var currentCommunication = _serverInfo._communications.Last();
            currentCommunication.Stop();

            _serverInfo.communicationsToRemove.Add(currentCommunication);

            RemoveFromList();
        }
    }
}
