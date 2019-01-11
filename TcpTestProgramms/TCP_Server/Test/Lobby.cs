using EandE_ServerModel.EandE.GameAndLogic;
using System;
using System.ComponentModel;

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
        }

        public void RunLobby()
        {
            isRunning = true;

            while (isRunning)
            {
                
            }
        }

        public void RunGame()
        {
            while (isRunning)
            {
                var backgroundworkerGame = new BackgroundWorker();
                backgroundworkerGame.DoWork += (obj, ea) => _game.Init(); ;
                backgroundworkerGame.RunWorkerAsync();
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
