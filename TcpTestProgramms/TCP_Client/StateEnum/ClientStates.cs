using System;
using System.Collections.Generic;
using System.Text;

namespace TCP_Client.StateEnum
{
    public enum ClientStates
    {
        NotConnected,
        Connecting,
        Handshake,
        WaitingForLobbyCheck,
        Lobby,
        GameRunning,
        ClientClosing,
		GameFinished
    }
}
