using System;

namespace TCP_Server.Test
{

    public class LobbyState
    {
        bool isRunning;

        public LobbyState()
        {
            isRunning = true;
        }

        public void Execute()
        {
            while (isRunning)
            {

            }
                Server.state = StateEnum.GameRunningState;
        }
    }
}

