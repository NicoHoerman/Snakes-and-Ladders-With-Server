using EandE_ServerModel.EandE.GameAndLogic;
using Shared.Enums;
using System;
using System.Threading.Tasks;
using TCP_Server.Actions;
using TCP_Server.Enum;

namespace TCP_Server.Support
{
    public class StateMachine
    {
        private bool _isRunning;

        private ServerInfo _serverinfo;
        private ServerActions _actionHandler;
        private Game _game;

        public StateMachine(ServerInfo serverinfo,ServerActions serverActions, Game game)
        {
            _serverinfo = serverinfo;
            _actionHandler = serverActions;
            _game = game;
        }

        public void Start()
        {
            _isRunning = true;
            Core.State = StateEnum.ServerRunningState;

            while (_isRunning)
            {
                switch (Core.State)
                {
                    case StateEnum.ServerRunningState:
                         _actionHandler._protocolActions.Add(ProtocolActionEnum.ValidationAnswer, _actionHandler.OnValidationAction);
                         _serverinfo._lobbylist.Add(new Lobby("Nicos_TestLobby", 2, 8080, _game));
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
            _actionHandler._protocolActions.Clear();
            _actionHandler._protocolActions.Add(ProtocolActionEnum.ValidationAnswer,
                _actionHandler.OnValidationAction);
            _actionHandler._protocolActions.Add(ProtocolActionEnum.Rule,
                _actionHandler.OnRuleAction);
            while (_actionHandler._ruleSet == false)
            { }

            _actionHandler._protocolActions.Clear();
            _actionHandler._protocolActions.Add(ProtocolActionEnum.ValidationAnswer,
                _actionHandler.OnValidationAction);
            _actionHandler._protocolActions.Add(ProtocolActionEnum.StartGame,
               _actionHandler.OnStartGameAction);
            while (Core.State == StateEnum.LobbyState)
            { }
            Task.Run(() => _serverinfo._lobbylist[0].RunGame());
			_game.State.ExecuteStateAction("classic");
            _actionHandler._protocolActions.Clear();
        }
        private void ExecuteGameRunningState()
        {
            _actionHandler._protocolActions.Add(ProtocolActionEnum.ValidationAnswer,
                _actionHandler.OnValidationAction);
            _actionHandler._protocolActions.Add(ProtocolActionEnum.RollDice
                , _actionHandler.OnRollDiceAction);
            _actionHandler._protocolActions.Add(ProtocolActionEnum.CloseGame
                , _actionHandler.OnCloseGameAction);

            while (Core.State == StateEnum.GameRunningState)
            { }
        }
    }
}
