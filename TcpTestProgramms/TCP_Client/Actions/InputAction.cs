using Newtonsoft.Json;
using Shared.Communications;
using Shared.Contract;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using TCP_Client.DTO;
using TCP_Client.PROTOCOLS;
using TCP_Client.UDP;
using Wrapper;
using Wrapper.Contracts;
using Wrapper.Implementation;

namespace TCP_Client.Actions
{
    public class InputAction
    {
        public bool _isConnected = false;
        public bool _declined = false;
        private bool _searched = false;

        public string AfterConnectMsg { get; set; } = string.Empty;

        public Dictionary<string, Action<string,ICommunication>> _inputActions;
        private Dictionary<ClientView, IView> _views;
        private ClientDataPackageProvider _clientDataPackageProvider;

        private readonly IErrorView _errorView;
        private readonly IUpdateOutputView _commandListOutputView;
        public readonly IInputView _inputView;
        private readonly IUpdateOutputView _serverTableView;
        private readonly IUpdateOutputView _infoOutputView;
        private readonly IUpdateOutputView _mainMenuOutputView;
        private Client _client;
        
        private OutputWrapper _outputWrapper;  
        private ProtocolAction _actionHandler;
        private UdpClientUnit _udpListener;

        public InputAction(ProtocolAction protocolAction, Dictionary<ClientView, IView> views,Client client, ClientDataPackageProvider clientDataPackageProvider)
        {
            _clientDataPackageProvider = clientDataPackageProvider;
            _client = client;
            _actionHandler = protocolAction;
            _views = views;
            _errorView = views[ClientView.Error] as IErrorView; // Potential null exception error.
            _commandListOutputView = views[ClientView.CommandList] as IUpdateOutputView; //Potenzieller Null Ausnahmen Fehler
            _inputView = views[ClientView.Input] as IInputView;
            _serverTableView = views[ClientView.ServerTable] as IUpdateOutputView;
            _infoOutputView = views[ClientView.InfoOutput] as IUpdateOutputView;
            _mainMenuOutputView = views[ClientView.MenuOutput] as IUpdateOutputView;

            _outputWrapper = new OutputWrapper();

            _inputActions = new Dictionary<string, Action<string,ICommunication>>
            {
                
            };

            _udpListener = new UdpClientUnit();
        }


        public void ParseAndExecuteCommand(string input,ICommunication communication)
        {
			string receivedInput = input;        
            if (input == "")
                return;
            if (input == "/someInt")
            {
                _errorView.ViewEnabled = true;
                _errorView.SetContent(input, "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }
            if (input.All(char.IsDigit))
            {
                
                input = "/someInt";
            }

            if (_inputActions.TryGetValue(input, out Action<string, ICommunication> action) == false)
            {
                _errorView.ViewEnabled = true;
                _errorView.SetContent(input, "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }

            action(receivedInput,communication);
        }

        #region Input actions

        public void OnInputHelpAction(string input, ICommunication communication)
        {
            if (!_isConnected)
            {
                _errorView.ViewEnabled = true;
                _errorView.SetContent(input, "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }
            if (_commandListOutputView.ViewEnabled)
            {
                _commandListOutputView.ViewEnabled = false;
            }
            else if(!_commandListOutputView.ViewEnabled)
            {
                _commandListOutputView.ViewEnabled = true;
            }                                      

            communication.Send(_clientDataPackageProvider.GetPackage("Help"));
        }

        public void OnInputRollDiceAction(string input, ICommunication communication)
        {
            if (!_isConnected)
            {
                _errorView.ViewEnabled = true;
                _errorView.SetContent(input, "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }
            communication.Send(_clientDataPackageProvider.GetPackage("RollDice"));
        }

        public void OnServerConnectAction(string input, ICommunication communication)
        {
            if (_isConnected | !_searched | _declined)
            {
                _errorView.ViewEnabled = true;
                _errorView.SetContent(input, "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }
			//if (input.Count != 2)
			//{
			//    _errorView.viewEnabled = true;
			//    _errorView.SetContent(input, "Error: " + "Server id missing or too many parameters.");
			//    return;
			//}
			int chosenServerId = Int32.Parse(input);
            if (_actionHandler._serverDictionary.Count >= chosenServerId)
            {

				BroadcastDTO current = _actionHandler.GetServer(chosenServerId-1);
                communication._client.Connect(IPAddress.Parse(current._server_ip), current._server_Port);    
                communication.SetNWStream();
                _client.SwitchState(StateEnum.ClientStates.Connecting);
                _isConnected = true;
                //fertig 
                //AfterConnectMsg = $"Server {chosenServerId} chosen";
                //_infoOutputView.viewEnabled = true;
                //_infoOutputView.SetUpdateContent(AfterConnectMsg +"\nYou established a connection with the server. Verifying Player Information...");
            }
            else
            {
                _errorView.ViewEnabled = true;
                _errorView.SetContent(input, "There is no server with this ID");
            }
            //_inputView.SetInputLine("Type a command:", 16);
            //_serverTableView.viewEnabled = false;
        }

        public void OnSearchAction(string input, ICommunication communication)
        {
            if (_isConnected)
            {
                _errorView.ViewEnabled = true;
                _errorView.SetContent(input, "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }

            _outputWrapper.WriteOutput(0, 1, "Searching...", ConsoleColor.DarkGray);                                                                                   

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _udpListener._closed = false;
            _udpListener.SendRequest();

            while (stopwatch.ElapsedMilliseconds < 3000)
            {
                if (_udpListener._dataList.Count > 0)
                {
                    communication.AddPackage(_udpListener._dataList.First());
                    _udpListener._dataList.RemoveAt(0);
                }
            }
            stopwatch.Stop();
            _udpListener.StopListening();
            _outputWrapper.Clear();
            _searched = true;
            _declined = false;
            _inputView.ViewEnabled = true;
            if(_actionHandler._serverTable.Length != 0)
            _inputView.SetInputLine("Enter the server number you want to connect to.",49);
        }

        public void OnCloseGameAction(string obj,ICommunication communication)
        {
            if (!_isConnected)
            {
                Thread.Sleep(5000);
                _client.CloseClient();
                return;
            }
            
            communication.Send(_clientDataPackageProvider.GetPackage("CloseGame"));
            Thread.Sleep(5000);
            _client.CloseClient();
        }

        public void OnStartGameAction(string arg1, ICommunication communication)
        {
            if (_isConnected)
                communication.Send(_clientDataPackageProvider.GetPackage("StartGame"));
        }

        public void OnClassicAction(string input, ICommunication communication)
        {
            if (_isConnected)
            {
                _mainMenuOutputView.ViewEnabled = false;
                communication.Send(_clientDataPackageProvider.GetPackage("Classic"));
            }
        }
        #endregion
    }
}
