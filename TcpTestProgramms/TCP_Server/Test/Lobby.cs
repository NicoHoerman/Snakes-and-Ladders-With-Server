using System;

namespace TCP_Server.Test
{
    public class Lobby
    {
        public int _MaxPlayerCount { get; set; }
        public int _CurrentPlayerCount { get; set; } = 0;
        public string _LobbyName { get; set; }
        public int _ServerPort { get; set; }

        public Lobby(string lobbyname, int maxplayercount, int port)
        {
            _MaxPlayerCount = maxplayercount;
            _LobbyName = lobbyname;
            _ServerPort = port;
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
