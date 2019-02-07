using System;
using System.ComponentModel;
using System.Threading;
using Wrapper.Implementation;
using Shared.Contract;
using TCP_Client.Actions;
using Shared.Communications;
using Wrapper;
using TCP_Client.StateEnum;
using TCP_Client.GameStuff;
using TCP_Client.GameStuff.ClassicEandE;
using System.Diagnostics;
using TCP_Client.DTO;
using System.Net;

namespace TCP_Client
{

	public class Client
    {
        public bool _isRunning;

        private ICommunication _communication;
        private ProtocolAction _actionHandler;
        public InputAction _inputHandler;
        private OutputWrapper _outputWrapper;
        private ViewUpdater _viewUpdater;
        private ViewDictionary _viewDictionary;
        private readonly ClientDataPackageProvider _clientDataPackageProvider;
        string _input = string.Empty;
		private Game _game;
		private  ClientStates State { get; set; }

		private object _metaData;

		//<Constructors>
		public Client(ICommunication communication)
        {
			_game = new Game();
            _clientDataPackageProvider = new ClientDataPackageProvider();
            _viewDictionary = new ViewDictionary();
            _communication = communication;
            _actionHandler = new ProtocolAction(_viewDictionary._views, this, _clientDataPackageProvider,_game);
            _inputHandler = new InputAction(_actionHandler, _viewDictionary._views, this, _clientDataPackageProvider);
            _outputWrapper = new OutputWrapper();
            _viewUpdater = new ViewUpdater(_viewDictionary._views);
            _actionHandler._enterToRefreshView.ViewEnabled = true;
            _actionHandler._enterToRefreshView.SetUpdateContent("Press enter to refresh\nafter you typed a command.");
		}

        public Client()
            : this(new TcpCommunication())
        { }

        private void CheckTCPUpdates()
        {
            while (_isRunning)
            {
                if (_communication.IsDataAvailable())
                {
					DataPackage data = _communication.Receive();
					Debug.WriteLine($"Package received:{data.Header.ToString()} Payload:{data.Payload.ToString()}");
					_actionHandler.ExecuteDataActionFor(data, _communication);
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

            backgroundworker3.DoWork += (obj, ea) => StateMachine();
            backgroundworker3.RunWorkerAsync();

            _isRunning = true;

            while (_isRunning)
            {
                _viewUpdater.UpdateView();
                Console.SetCursorPosition(_inputHandler._inputView._xCursorPosition, 0);
                _input = _outputWrapper.ReadInput();
                _outputWrapper.Clear();
                _inputHandler.ParseAndExecuteCommand(_input, _communication);
            }
        }

        public void StateMachine()
        {
            State = ClientStates.NotConnected;

            while (_isRunning)
            {
                switch (State)
                {
                    case ClientStates.NotConnected:
                        _inputHandler._inputActions.Add("/search", _inputHandler.OnSearchAction);
                        _inputHandler._inputActions.Add("/someInt", _inputHandler.OnServerConnectAction);
                        _inputHandler._inputActions.Add("/closegame", _inputHandler.OnCloseGameAction);
                        //_actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.UpdateView, _actionHandler.OnUpdateAction);
                        _actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.Broadcast, _actionHandler.OnBroadcastAction);
                        while (State == ClientStates.NotConnected)
                        { }
						Debug.WriteLine($"State switched: From: {State.ToString()}");
                        break;

                    case ClientStates.Connecting:
						Debug.WriteLine($"State switched: From NotConnected To {State.ToString()}");
						_inputHandler._inputActions.Clear();
                        _actionHandler._protocolActions.Clear();
                        _actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.ValidationRequest, _actionHandler.OnValidationRequestAction);
                        _actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.ValidationAccepted, _actionHandler.OnValidationAcceptedAction);

						//hier
						BroadcastDTO current = _actionHandler.GetServer((int)_metaData - 1);
						_communication._client.Connect(IPAddress.Parse(current._server_ip), current._server_Port);
						_communication.SetNWStream();

						while (State == ClientStates.Connecting)
                        { }
                        break;

                    case ClientStates.WaitingForLobbyCheck:
						Debug.WriteLine($"State switched: From Connecting To {State.ToString()}");
						_inputHandler._inputActions.Add("/closegame", _inputHandler.OnCloseGameAction);
                        _actionHandler._protocolActions.Clear();
						_actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.AcceptInfo, _actionHandler.OnAcceptInfoAction);
						_actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.DeclineInfo, _actionHandler.OnDeclineInfoAction);
                        _actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.LobbyCheckFailed, _actionHandler.OnLobbyCheckFailedAction);
                        _actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.LobbyCheckSuccessful, _actionHandler.OnLobbyCheckSuccessfulAction);
                        _actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.UpdateView, _actionHandler.OnUpdateAction);
                        while (State == ClientStates.WaitingForLobbyCheck)
                        { }
                        break;

                    case ClientStates.Lobby:
						Debug.WriteLine($"State switched: From WaitingForLobbycheck To {State.ToString()}");
						_actionHandler._protocolActions.Clear();
                        _actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.AcceptInfo, _actionHandler.OnAcceptInfoAction);
                        _actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.UpdateView, _actionHandler.OnUpdateAction);
                        _actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.ServerStartingGame, _actionHandler.OnServerStartingGameAction);
                        _inputHandler._inputActions.Add("/startgame", _inputHandler.OnStartGameAction);
                        _inputHandler._inputActions.Add("/classic", _inputHandler.OnClassicAction);
                        while (State == ClientStates.Lobby)
                        { }
                        break;

                    case ClientStates.GameRunning:
						Debug.WriteLine($"State switched: From Lobby To {State.ToString()}");
						_game.CreateRules();
						_game.Rules.SetupEntitites();
						_game.MakeBoardView();

						_actionHandler._protocolActions.Clear();
                        _inputHandler._inputActions.Clear();
                        _actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.UpdateView
                            ,_actionHandler.OnUpdateAction);
						_actionHandler._protocolActions.Add(Shared.Enums.ProtocolActionEnum.TurnResult
							, _actionHandler.OnTurnResultAction);
                        _inputHandler._inputActions.Add("/rolldice", _inputHandler.OnInputRollDiceAction);
                        _inputHandler._inputActions.Add("/help", _inputHandler.OnInputHelpAction);
                        _inputHandler._inputActions.Add("/closegame", _inputHandler.OnCloseGameAction);

                        while (State == ClientStates.GameRunning)
                        { }
                        break;
                }
            }
        }

        public void SwitchState(ClientStates newState, object metaData = null)
        {
            State = newState;
			_metaData = metaData;
        }

        public void CloseClient()
        {          
            _viewUpdater._isViewRunning = false;
            _communication.Stop();
            _isRunning = false;
        }
    }
}
