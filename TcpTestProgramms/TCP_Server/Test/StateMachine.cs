using Shared.Enums;
using System;
using TCP_Server.Actions;

namespace TCP_Server.Test
{
    public class StateMachine
    {
        private ServerInfo _serverinfo;
        private bool isRunning;
        private ServerActions _ActionHandler;

        public StateMachine(ServerInfo serverinfo,ServerActions serverActions)
        {
            _serverinfo = serverinfo;
            _ActionHandler = serverActions;
        }

        public void Start()
        {
            isRunning = true;
            Core.State = StateEnum.ServerRunningState;

            while (isRunning)
            {
                switch (Core.State)
                {
                    case StateEnum.ServerRunningState:
                         _serverinfo.lobbylist.Add(new Lobby("name", 2, 8080));
                         _ActionHandler._protocolActions.Add(ProtocolActionEnum.ValidationAnswer, _ActionHandler.OnValidationAction);
                        while (Core.State == StateEnum.ServerRunningState)
                        { }
                        break;
                    case StateEnum.LobbyState:
                        ExecuteLobbyState();                        
                        break;
                    case StateEnum.GameRunningState:
                        ExecuteGameRunningState();
                        
                        break;
                    case StateEnum.ServerEndingState:
                        break;
                    default:
                        break;
                }
            }
        }
        private void ExecuteLobbyState()
        {
            _ActionHandler._protocolActions.Add(ProtocolActionEnum.StartGame
                             , _ActionHandler.OnStartGameAction);
            _ActionHandler._protocolActions.Add(ProtocolActionEnum.Rule,
                _ActionHandler.OnRuleAction);
            while (Core.State == StateEnum.LobbyState)
            { }
            _ActionHandler._protocolActions.Clear();
        }
        private void ExecuteGameRunningState()
        {
            _ActionHandler._protocolActions.Add(ProtocolActionEnum.RollDice
                , _ActionHandler.OnRollDiceAction);
            _ActionHandler._protocolActions.Add(ProtocolActionEnum.GetHelp
                , _ActionHandler.OnGetHelpAction);
            _ActionHandler._protocolActions.Add(ProtocolActionEnum.CloseGame
                , _ActionHandler.OnCloseGameAction);

            while (Core.State == StateEnum.GameRunningState)
            { }
        }
    }
}