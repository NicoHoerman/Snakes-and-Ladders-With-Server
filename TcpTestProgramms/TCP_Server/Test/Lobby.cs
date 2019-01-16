using EandE_ServerModel.EandE.GameAndLogic;
using System;
using System.ComponentModel;

namespace TCP_Server.Test
{
    public class Lobby
    {
        private bool _isRunning;

        public int _MaxPlayerCount { get; set; }
        public int _CurrentPlayerCount { get; set; } = 1;
        public string _LobbyName { get; set; }
        public int _ServerPort { get; set; }

        private Game _game;

        public Lobby(string lobbyname, int maxplayercount, int port, Game game)
        {
            _MaxPlayerCount = maxplayercount;
            _LobbyName = lobbyname;
            _ServerPort = port;
            _game = game;
        }

        public void RunLobby()
        {
            _isRunning = true;

            while (_isRunning)
            {
                
            }
        }

        public void RunGame()
        {
            _isRunning = true;
            while (_isRunning)
            {
                _game.Init(); ;
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

    }
}
