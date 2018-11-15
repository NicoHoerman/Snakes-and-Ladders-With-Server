using EandE_ServerModel.ServerModel;
using EandE_ServerModel.ServerModel.ClientAndServer;
using EandE_ServerModel.ServerModel.Contracts;
using EandE_ServerModel.ServerModel.ProtocolActionStuff;
using EandE_ServerModel.ServerModel.PROTOCOLS.Client;
using EandE_ServerModel.ServerModel.PROTOCOLS.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace TCP_Model.ServerModel.InputActionStuff
{
    public class InputAction
    {
        private int someInt;
        private bool isConnected = false;
        private System.Timers.Timer timer;
        public string _afterConnectMsg = string.Empty;

        public Dictionary<string, Action<string,ICommunication>> _inputActions;

        private ProtocolAction _ActionHanlder;
        OutputWrapper _OutputWrapper;  

        private Receiver _UdpListener;

        public InputAction(ProtocolAction protocolAction)
        {

            _ActionHanlder = protocolAction;
            _OutputWrapper = new OutputWrapper();

            _inputActions = new Dictionary<string, Action<string,ICommunication>>
            {
                { "/help", OnInputHelpAction },
                { "/rolldice", OnInputRollDiceAction },
                { "/closegame", OnCloseGameAction },
                {"/someInt" ,OnIntAction },
                {"/search", OnSearchAction },
                {"/startgame", OnStartGameAction }
            };

            _UdpListener = new Receiver();

        }

       

        public void ParseAndExecuteCommand(string input,ICommunication communication)
        {
            string receivedInput = input;
            if (input.All(char.IsDigit))
            {
                input = "/someInt";
            }

            if (_inputActions.TryGetValue(input, out var action) == false)
            {
                //log
                return;
            }

            action(receivedInput,communication);
        }

        #region Input actions

        private void OnInputHelpAction(string obj,ICommunication communication)
        {
            if (!isConnected)
            {
                _OutputWrapper.WriteOutput(0, 2, "Invalid Command", ConsoleColor.Red);
                return;
            }

            var dataPackage = new DataPackage
            {

                Header = ProtocolActionEnum.GetHelp, //"Client_wants_to_get_help",
                Payload = JsonConvert.SerializeObject(new PROT_HELP
                {

                    _Client_id = 5 //actual implementation: smth like this Player_ID = CurrentPawn.playerId;

                })

            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        private void OnInputRollDiceAction(string obj,ICommunication communication)
        {
            if (!isConnected)
            {
                _OutputWrapper.WriteOutput(0, 2, "Invalid Command", ConsoleColor.Red);
                return;
            }

            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.RollDice, //"Client_wants_to_rolldice",
                Payload = JsonConvert.SerializeObject(new PROT_ROLLDICE
                {
                    _Client_id = 2 //Player_ID = actual implementation: smth like this(CurrentPawn.playerId)

                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        private void OnIntAction(string obj,ICommunication communication)
        {
            if (isConnected)
            {
                _OutputWrapper.WriteOutput(0,2,"Invalid Command",ConsoleColor.Red);
                return;
            }
            int chosenServerId = Int32.Parse(obj);
            if (_ActionHanlder._serverDictionary.Count >= chosenServerId)
            {
                PROT_BROADCAST current = _ActionHanlder.GetServer(chosenServerId-1);
                communication._client.Connect(IPAddress.Parse(current._Server_ip), 8080);
                _afterConnectMsg = $"Server {chosenServerId} chosen";
                isConnected = true;
            }
            else
            {
                _afterConnectMsg = "no Server with this indentifier";
            }

        }

        private void OnSearchAction(string obj,ICommunication communication)
        {
            if (isConnected)
            {
                _OutputWrapper.WriteOutput(0, 2, "Invalid Command", ConsoleColor.Red);
                return;
            }

            _OutputWrapper.WriteOutput(0, 1, "Searching...", ConsoleColor.DarkGray);                                                                                   
            if (isConnected)
                return;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _UdpListener.StartListening();

            while (stopwatch.ElapsedMilliseconds < 10000)
            {
                if (_UdpListener._DataList.Count > 0)
                {
                    communication.AddPackage(_UdpListener._DataList.First());
                    _UdpListener._DataList.RemoveAt(0);
                }
            }
            stopwatch.Stop();
            _UdpListener.StopListening();
        }

        private void OnCloseGameAction(string obj,ICommunication communication)
        {
            throw new NotImplementedException();
        }

        private void OnStartGameAction(string arg1, ICommunication arg2)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
