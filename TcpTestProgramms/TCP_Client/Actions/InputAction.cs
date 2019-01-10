﻿using Newtonsoft.Json;
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
        public bool isConnected = false;
        public bool Declined = false;
        private bool Searched = false;
        private System.Timers.Timer timer;

        public string AfterConnectMsg { get; set; } = string.Empty;

        public Dictionary<string, Action<List<string>, ICommunication>> _inputActions;
        private Dictionary<ClientView, IView> _views;
        private ClientDataPackageProvider _clientDataPackageProvider;

        private readonly IErrorView _errorView;
        private readonly IUpdateOutputView _commandListOutputView;
        public readonly IInputView _inputView;
        private readonly IUpdateOutputView _serverTableView;
        private readonly IUpdateOutputView _infoOutputView;
        private readonly IUpdateOutputView _mainMenuOutputView;
        private Client _client;
        
        private OutputWrapper _OutputWrapper;  
        private ProtocolAction _ActionHandler;
        private UdpClientUnit _UdpListener;

        public InputAction(ProtocolAction protocolAction, Dictionary<ClientView, IView> views,Client client, ClientDataPackageProvider clientDataPackageProvider)
        {
            _clientDataPackageProvider = clientDataPackageProvider;
            _client = client;
            _ActionHandler = protocolAction;
            _views = views;
            _errorView = views[ClientView.Error] as IErrorView; // Potential null exception error.
            _commandListOutputView = views[ClientView.CommandList] as IUpdateOutputView; //Potenzieller Null Ausnahmen Fehler
            _inputView = views[ClientView.Input] as IInputView;
            _serverTableView = views[ClientView.ServerTable] as IUpdateOutputView;
            _infoOutputView = views[ClientView.InfoOutput] as IUpdateOutputView;
            _mainMenuOutputView = views[ClientView.MenuOutput] as IUpdateOutputView;

            _OutputWrapper = new OutputWrapper();

            _inputActions = new Dictionary<string, Action<List<string>,ICommunication>>
            {
                { "/help", OnInputHelpAction },
                { "/rolldice", OnInputRollDiceAction },
                { "/closegame", OnCloseGameAction },
                {"/server" , OnServerConnectAction },
                {"/search", OnSearchAction },
                {"/startgame", OnStartGameAction },
                {"/classic", OnClassicAction }
            };

            _UdpListener = new UdpClientUnit();

        }


        public void ParseAndExecuteCommand(string input,ICommunication communication)
        {
            var receivedInput = input.Split(' ').ToList();
            if (input == "")
                return;
            if (input == "/someInt")
            {
                _errorView.viewEnabled = true;
                _errorView.SetContent(input, "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }
            if (input.All(char.IsDigit))
            {
                
                input = "/someInt";
            }

            if (_inputActions.TryGetValue(receivedInput[0], out var action) == false)
            {
                _errorView.viewEnabled = true;
                _errorView.SetContent(input, "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }

            action(receivedInput,communication);
        }

        #region Input actions

        public void OnInputHelpAction(List<string> input, ICommunication communication)
        {
            if (!isConnected)
            {
                _errorView.viewEnabled = true;
                _errorView.SetContent(input[0], "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }
            if (_commandListOutputView.viewEnabled)
            {
                _commandListOutputView.viewEnabled = false;
            }
            else if(!_commandListOutputView.viewEnabled)
            {
                _commandListOutputView.viewEnabled = true;
            }
                            

            var dataPackage = new DataPackage
            {

                Header = ProtocolActionEnum.GetHelp, //"Client_wants_to_get_help",
                Payload = JsonConvert.SerializeObject(new PROT_HELP
                {

                })

            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        public void OnInputRollDiceAction(List<string> input, ICommunication communication)
        {
            if (!isConnected)
            {
                _errorView.viewEnabled = true;
                _errorView.SetContent(input[0], "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }

            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.RollDice, //"Client_wants_to_rolldice",
                Payload = JsonConvert.SerializeObject(new PROT_ROLLDICE
                {

                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        public void OnServerConnectAction(List<string> input, ICommunication communication)
        {

            if (isConnected | !Searched | Declined)
            {
                _errorView.viewEnabled = true;
                _errorView.SetContent(input[0], "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }
            if (input.Count != 2)
            {
                _errorView.viewEnabled = true;
                _errorView.SetContent(input[0], "Error: " + "Server id missing or too many parameters.");
                return;
            }
            int chosenServerId = Int32.Parse(input[1]);
            if (_ActionHandler._serverDictionary.Count >= chosenServerId)
            {
                _client.SwitchState(StateEnum.ClientStates.Connecting);

                BroadcastDTO current = _ActionHandler.GetServer(chosenServerId-1);
                try
                {
                    communication._client.Connect(IPAddress.Parse(current._Server_ip), current._Server_Port);    
                }
                catch
                {

                }
                AfterConnectMsg = $"Server {chosenServerId} chosen";
                isConnected = true;
                _infoOutputView.viewEnabled = true;
                _infoOutputView.SetUpdateContent(AfterConnectMsg +"\nYou established a connection with the server. Verifying Player Information...");
                communication.SetNWStream();
                var dataPackage = new DataPackage
                {
                    Header = ProtocolActionEnum.OnConnection, 
                    Payload = JsonConvert.SerializeObject(new PROT_CONNECTION
                    {

                    })
                };
                dataPackage.Size = dataPackage.ToByteArray().Length;

                communication.Send(dataPackage);
            }
            else
            {
                _errorView.viewEnabled = true;
                _errorView.SetContent(input[1], "There is no server with this ID");
            }

            _inputView.SetInputLine("Type a command:", 16);
            _serverTableView.viewEnabled = false;

        }

        public void OnSearchAction(List<string> input, ICommunication communication)
        {
            if (isConnected)
            {
                _errorView.viewEnabled = true;
                _errorView.SetContent(input[0], "Error: " + "This command does not exist or isn't enabled at this time");
                return;
            }

            _OutputWrapper.WriteOutput(0, 1, "Searching...", ConsoleColor.DarkGray);                                                                                   

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _UdpListener._closed = false;
            _UdpListener.SendRequest();

            while (stopwatch.ElapsedMilliseconds < 3000)
            {
                if (_UdpListener._DataList.Count > 0)
                {
                    communication.AddPackage(_UdpListener._DataList.First());
                    _UdpListener._DataList.RemoveAt(0);
                }
            }
            stopwatch.Stop();
            _UdpListener.StopListening();
            _OutputWrapper.Clear();
            Searched = true;
            Declined = false;
            _inputView.viewEnabled = true;
            if(_ActionHandler._serverTable.Length != 0)
            _inputView.SetInputLine("Enter the server number you want to connect to.",49);
            
        }

        public void OnCloseGameAction(List<string> obj,ICommunication communication)
        {
            if (!isConnected)
            {
                Thread.Sleep(5000);
                _client.CloseClient();
                return;
            }
            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.CloseGame,
                Payload = JsonConvert.SerializeObject(new PROT_CLOSEGAME
                {

                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
            Thread.Sleep(5000);
            _client.CloseClient();


        }

        public void OnStartGameAction(List<string> arg1, ICommunication communication)
        {
            if (isConnected)
            {
                var dataPackage = new DataPackage
                {
                    Header = ProtocolActionEnum.StartGame,
                    Payload = JsonConvert.SerializeObject(new PROT_STARTGAME
                    {
                        
                    })
                };
                dataPackage.Size = dataPackage.ToByteArray().Length;

                communication.Send(dataPackage);
            }
        }

        public void OnClassicAction(List<string> input, ICommunication communication)
        {
            if (isConnected)
            {
                _mainMenuOutputView.viewEnabled = false;

                var dataPackage = new DataPackage
                {
                    Header = ProtocolActionEnum.Rule,
                    Payload = JsonConvert.SerializeObject(new PROT_STARTGAME
                    {

                    })
                };
                dataPackage.Size = dataPackage.ToByteArray().Length;

                communication.Send(dataPackage);
            }
        }
        #endregion


    }
}
