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
using System.Net;

namespace TCP_Model.ServerModel.InputActionStuff
{
    public class InputAction
    {
        private int someInt;
        private bool isConnected = false;
        private System.Timers.Timer timer;

        public Dictionary<string, Action<string,ICommunication>> _inputActions;

        private ProtocolAction _ActionHanlder;

        private Receiver _UdpListener;

        public InputAction()
        {

            _ActionHanlder = new ProtocolAction();

            _inputActions = new Dictionary<string, Action<string,ICommunication>>
            {
                { "/help", OnInputHelpAction },
                { "/rolldice", OnInputRollDiceAction },
                { "/closegame", OnCloseGameAction },
                {$"/{someInt}" ,OnIntAction },
                {"/search", OnSearchAction }
            };

            _UdpListener = new Receiver();

        }

        public void ParseAndExecuteCommand(string input,ICommunication communication)
        {
            if (_inputActions.TryGetValue(input, out var action) == false)
            {
                Console.WriteLine($"Invalid command: {input}");
                return;
            }

            action(input,communication);
        }

        #region Input actions

        private void OnInputHelpAction(string obj,ICommunication communication)
        {
            if (!isConnected)
                return;
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
                return;

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
                return;

            int chosenServerId = Int32.Parse(obj);
            if (_ActionHanlder._serverDictionary.Count >= chosenServerId)
            {
                PROT_BROADCAST current = _ActionHanlder.GetServer(chosenServerId);
                communication._client.Connect(IPAddress.Parse(current._Server_ip), 8080);
                isConnected = true;
            }

        }

        private void OnSearchAction(string obj,ICommunication communication)
        {
            if (isConnected)
                return;



            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _Udplistener.StartListening();

            while (stopwatch.ElapsedMilliseconds < 10000)
            {
                if (_Udplistener._DataList.Count > 0)
                {
                    communication.AddPackage(_Udplistener._DataList.First());
                    _Udplistener._DataList.RemoveAt(0);
                }
            }
            stopwatch.Stop();
            _Udplistener.StopListening();
        }

        private void OnCloseGameAction(string obj,ICommunication communication)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
