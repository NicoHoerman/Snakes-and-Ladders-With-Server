using System;

namespace TCP_Server.Test
{
    public class StateMachine
    {
        private ServerInfo _serverinfo;
        private bool isRunning;

        public StateMachine(ServerInfo serverinfo)
        {
            _serverinfo = serverinfo;
        }

        public void Start()
        {
            isRunning = true;

            while (isRunning)
            {
                switch (Server.state)
                {
                    case StateEnum.ServerRunningState:
                        _serverinfo.lobbylist.Add(new Lobby("name", 2,8080));
                       
                        Server.state = StateEnum.LobbyState;

                        break;
                    case StateEnum.LobbyState:
                        var currentState = new LobbyState();
                        currentState.Execute();

                        break;
                    case StateEnum.GameRunningState:
                        
                        Server.state = StateEnum.LobbyState;

                        break;
                    case StateEnum.ServerEndingState:
                        break;
                    default:
                        break;
                }


            }


        }
    }
}