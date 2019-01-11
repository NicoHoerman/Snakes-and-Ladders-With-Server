using System;
using System.ComponentModel;
using System.Threading;
using Wrapper.Implementation;
using Shared.Contract;
using TCP_Client.Actions;
using Shared.Communications;
using System.Collections.Generic;
using System.Linq;
using Wrapper;
using Wrapper.Contracts;
using Wrapper.View;
using TCP_Client.StateEnum;

namespace TCP_Client
{

    public class Client
    {
        public bool isRunning;

        private ICommunication _communication;
        private ProtocolAction _ActionHandler;
        public InputAction _InputHandler;
        private OutputWrapper _OutputWrapper;
        private ViewUpdater _ViewUpdater;
        private ViewDictionary _viewDictionary;
        private ClientDataPackageProvider clientDataPackageProvider;
        string input = string.Empty;

        ClientStates state { get; set; }



        //<Constructors>
        public Client(ICommunication communication)
        {
            clientDataPackageProvider = new ClientDataPackageProvider();
            _viewDictionary = new ViewDictionary();
            _communication = communication;
            _ActionHandler = new ProtocolAction(_viewDictionary._views, this, clientDataPackageProvider);
            _InputHandler = new InputAction(_ActionHandler, _viewDictionary._views, this, clientDataPackageProvider);
            _OutputWrapper = new OutputWrapper();
            _ViewUpdater = new ViewUpdater(_viewDictionary._views);
            _ActionHandler._enterToRefreshView.viewEnabled = true;
            _ActionHandler._enterToRefreshView.SetUpdateContent("Press enter to refresh\nafter you typed a command.");
        }

        public Client()
            : this(new TcpCommunication())
        { }

        private void CheckTCPUpdates()
        {
            while (isRunning)
            {
                if (_communication.IsDataAvailable())
                {
                    var data = _communication.Receive();
                    _ActionHandler.ExecuteDataActionFor(data, _communication);
                }
                else
                    Thread.Sleep(1);
            }
        }

        public void Run()
        {
            var backgroundworker = new BackgroundWorker();

            backgroundworker.DoWork += (obj, ea) => CheckTCPUpdates();
            backgroundworker.RunWorkerAsync();

            var backgroundworker3 = new BackgroundWorker();

            backgroundworker3.DoWork += (obj, ea) => StateMachine(state);
            backgroundworker3.RunWorkerAsync();

            isRunning = true;

            while (isRunning)
            {
                _ViewUpdater.UpdateView();
                Console.SetCursorPosition(_InputHandler._inputView._xCursorPosition, 0);
                input = _OutputWrapper.ReadInput();
                _OutputWrapper.Clear();
                _InputHandler.ParseAndExecuteCommand(input, _communication);
            }
        }

        public void StateMachine(ClientStates state)
        {
            state = ClientStates.NotConnected;

            while (isRunning)
            {
                switch (state)
                {
                    case ClientStates.NotConnected:
                        _InputHandler._inputActions.Add("/search", _InputHandler.OnSearchAction);
                        _InputHandler._inputActions.Add("/someInt", _InputHandler.OnServerConnectAction);
                        _InputHandler._inputActions.Add("/closegame", _InputHandler.OnCloseGameAction);
                        _ActionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.UpdateView, _ActionHandler.OnUpdateAction);
                        _ActionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.Broadcast, _ActionHandler.OnBroadcastAction);
                        while (state == ClientStates.NotConnected)
                        { }
                        break;

                    case ClientStates.Connecting:
                        _InputHandler._inputActions.Clear();
                        _ActionHandler._protocolActions.Clear();
                        _ActionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.ValidationRequest, _ActionHandler.OnValidationRequestAction);
                        _ActionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.ValidationAccepted, _ActionHandler.OnValidationAcceptedAction);
                        while (state == ClientStates.Connecting)
                        { }
                        break;

                    case ClientStates.WaitingForLobbyCheck:                      
                        _InputHandler._inputActions.Add("/closegame", _InputHandler.OnCloseGameAction);
                        _ActionHandler._protocolActions.Clear();
                        _ActionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.AcceptInfo, _ActionHandler.OnAcceptInfoAction);
                        _ActionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.DeclineInfo, _ActionHandler.OnDeclineInfoAction);
                        _ActionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.LobbyCheckFailed, _ActionHandler.OnLobbyCheckFailedAction);
                        _ActionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.LobbyCheckSuccessful, _ActionHandler.OnLobbyCheckSuccessfulAction);
                        _ActionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.UpdateView, _ActionHandler.OnUpdateAction);
                        while (state == ClientStates.WaitingForLobbyCheck)
                        { }
                        break;

                    case ClientStates.Lobby:
                        _ActionHandler._protocolActions.Clear();
                        _ActionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.UpdateView, _ActionHandler.OnUpdateAction);
                        _ActionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.ServerStartingGame, _ActionHandler.OnServerStartingGameAction);
                        _InputHandler._inputActions.Add("/startgame", _InputHandler.OnStartGameAction);
                        _InputHandler._inputActions.Add("/classic", _InputHandler.OnClassicAction);
                        while (state == ClientStates.Lobby)
                        { }
                        break;

                    case ClientStates.GameRunning:
                        while (state == ClientStates.GameRunning)
                        { }
                        break;                  

                }
            }
        }

        public void SwitchState(ClientStates newState)
        {
            state = newState;
        }

        public void CloseClient()
<<<<<<< HEAD
        {
=======
        {          
>>>>>>> LeonsDeafBranch
            _ViewUpdater.isViewRunning = false;
            _communication.Stop();
            isRunning = false;
        }
    }
}