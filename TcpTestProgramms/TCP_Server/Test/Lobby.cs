using EandE_ServerModel.EandE.GameAndLogic;
using System;

namespace TCP_Server.Test
{
    public class Lobby
    {
        private bool isRunning;

        public int _MaxPlayerCount { get; set; }
        public int _CurrentPlayerCount { get; set; } = 0;
        public string _LobbyName { get; set; }
        public int _ServerPort { get; set; }

        private Game _game;

        public Lobby(string lobbyname, int maxplayercount, int port, Game game)
        {
            _MaxPlayerCount = maxplayercount;
            _LobbyName = lobbyname;
            _ServerPort = port;
            _game = game;
            RunLobby();
        }

        public void RunLobby()
        {
            isRunning = true;

            while (isRunning)
            {

            }

        }

        public bool IsLobbyComplete()
        {
            if (_CurrentPlayerCount == _MaxPlayerCount)
            {
                return true;
            }
            else
                return false;
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
