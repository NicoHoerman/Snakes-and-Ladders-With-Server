using System;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using TCP_Model.PROTOCOLS.Server;
using TCP_Model.ClassicEandE;
using TCP_Model.EandEContracts;
using TCP_Model.Contracts;
using TCP_Model.Communications;
using TCP_Model.GameAndLogic;
using TCP_Model.PROTOCOLS.Client;

namespace TCP_Model.ClientAndServer
{

    public class Client
    {
        public bool isRunning;
        private string updateInfo = string.Empty;
        private int i;
        private Dictionary<ProtocolAction, Action<DataPackage>> _protocolActions;
        private Dictionary<string, Action<string>> _inputActions;
        private readonly IGame _game;
        private ICommunication _communication;
        private Receiver _udplistener;

        public Client(ICommunication communication, IGame game)
        {
            _game = game;
            _communication = communication;
            _udplistener = new Receiver();

            //wen ein Paket ankommt hat es im header einen int der eine ProtocolAction ist 
            // ist work in Progress wie die heißen und welche Zahl ist nur zum Testen
            _protocolActions = new Dictionary<ProtocolAction, Action<DataPackage>>
            {
                { ProtocolAction.HelpText, OnHelpTextAction },
                { ProtocolAction.UpdateView, OnUpdateAction},
                { ProtocolAction.Broadcast, OnBroadcastAction },
                { ProtocolAction.Accept, OnAcceptAction },
                { ProtocolAction.Decline, OnDeclineAction }
            
            };
            //sind für den Client. Bei dem input ... mach ...
            _inputActions = new Dictionary<string, Action<string>>
            {
                { "/help", OnInputHelpAction },
                { "/rolldice", OnInputRollDiceAction },
                { "/connect", OnInputConnectAction },
                { "/closegame", OnCloseGameAction }
            };
        }

        public Client()
            : this(new TcpCommunication(), new Game())
        { }
        


        private void CheckForUpdates()
        {
            while (true)
            {
                if (_communication.IsDataAvailable())
                {
                    var data = _communication.Receive();
                    ExecuteDataActionFor(data);
                }
                else
                    Thread.Sleep(1);
            }

        }

        private void ExecuteDataActionFor(DataPackage data)
        {
            //weißt dem packet die richtige funktion zu
            if (_protocolActions.TryGetValue(data.Header, out var protocolAction) == false)
                throw new InvalidOperationException("Invalid communication");
            //führt die bekommene methode mit dem datapackage aus
            protocolAction(data);
        }

        public void Run()
        {
            var backgroundworker = new BackgroundWorker();

            backgroundworker.DoWork += (obj, ea) => CheckForUpdates();
            backgroundworker.RunWorkerAsync();

            var backgroundworker2 = new BackgroundWorker();
            backgroundworker2.DoWork += (obj, ea) => _udplistener.StartListening();
            backgroundworker2.RunWorkerAsync();
           
            isRunning = true;
            while (isRunning)
            {
                var input = Console.ReadLine();
                ParseAndExecuteCommand(input);
            }

        }

        private void ParseAndExecuteCommand(string input)
        {
            //weißt dem input die richtige action zu
            if (_inputActions.TryGetValue(input, out var action) == false)
            {
                Console.WriteLine($"Invalid command: {input}");
                return;
            }

            //führt die action mit dem input aus 
            action(input);
        }

        #region Input actions

        //onInputHelp ertell ein DatenPacket und schick es ab
        private void OnInputHelpAction(string obj)
        {

            var dataPackage = new DataPackage
            {
                
                Header = ProtocolAction.GetHelp, //"Client_wants_to_get_help",
                Payload = JsonConvert.SerializeObject(new PROT_HELP
                {

                    client_id = 5 //actual implementation: smth like this Player_ID = CurrentPawn.playerId;

                })
                
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            _communication.Send(dataPackage);
        }

        private void OnInputRollDiceAction(string obj)
        {
            var dataPackage = new DataPackage
            {
                Header = ProtocolAction.RollDice, //"Client_wants_to_rolldice",
                Payload = JsonConvert.SerializeObject(new PROT_ROLLDICE
                {
                    client_id = 2 //Player_ID = actual implementation: smth like this(CurrentPawn.playerId)

                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            _communication.Send(dataPackage);
        }

        private void OnInputConnectAction(string obj)
        {
                       
            var dataPackage = new DataPackage
            {
                Header = ProtocolAction.Connect,
                Payload = JsonConvert.SerializeObject(new PROT_CONNECT
                {
                    client_id = 3

                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            _communication.Send(dataPackage);
        }

        private void OnCloseGameAction(string obj)
        {
            isRunning = false;
        }



        #endregion


        #region Protocol actions

        
        private void OnHelpTextAction(DataPackage data)
        {
            var helpText = CreateProtocol<PROT_HELPTEXT>(data);

            Console.Write("Received help text: " + helpText.text);
        }

        private void OnUpdateAction(DataPackage data)
        {
            var updatedView = CreateProtocol<PROT_UPDATE>(data);

            Console.WriteLine("Received update: " + updatedView.updated_board+"\n"+updatedView.updated_dice_information
                +"\n"+updatedView.updated_turn_information);
        }

        private void OnBroadcastAction(DataPackage data)
        {
            var broadcast = CreateProtocol<PROT_BROADCAST>(data);

            Console.WriteLine("Server detected!\nServer name: " + broadcast.server_name + 
                "\nServer IP: " + broadcast.server_ip + "\nPlayer slots: " + 
                broadcast.player_slot_info+"\nIf you want to connect, type in /connect.");
        }

        private void OnAcceptAction(DataPackage data)
        {

            if (i == 0)
            {
                var accept = CreateProtocol<PROT_ACCEPT>(data);

                Console.WriteLine(accept.message);
            }
            else Console.WriteLine("Error: You are already connected.");

            _game.Init();

            i++;

        }

        private void OnDeclineAction(DataPackage data)
        {
            var decline = CreateProtocol<PROT_DECLINE>(data);

            Console.WriteLine(decline.message);
            
        }
        #endregion


        #region Static helper functions

        //erstellt Protokolle
        private static T CreateProtocol<T>(DataPackage data) where T : IProtocol
        {
            //macht aus einem Objekt String ein wieder das urpsrüngliche Objekt Protokoll
            return JsonConvert.DeserializeObject<T>(data.Payload);
        }

        #endregion
    }
}

