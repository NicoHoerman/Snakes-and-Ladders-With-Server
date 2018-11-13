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
using System.Text;
using System.Net;
using System.Timers;
using System.Diagnostics;

namespace TCP_Model.ClientAndServer
{

    public class Client
    {
        public bool isRunning;
        private string updateInfo = string.Empty;
        private int i;
        private int someInt;
        private bool isConnected = false;
        private System.Timers.Timer timer;

        private Dictionary<ProtocolAction, Action<DataPackage>> _protocolActions;
        private Dictionary<string, Action<string>> _inputActions;
        private Dictionary<int, PROT_BROADCAST> _serverDictionary;
        private Receiver _udplistener;

        private readonly IGame _game;
        private ICommunication _communication;

        public Client(ICommunication communication, IGame game)
        {
            _game = game;
            _communication = communication;
            _udplistener = new Receiver();
            _serverDictionary = new Dictionary<int, PROT_BROADCAST>();

            _protocolActions = new Dictionary<ProtocolAction, Action<DataPackage>>
            {
                { ProtocolAction.HelpText, OnHelpTextAction },
                { ProtocolAction.UpdateView, OnUpdateAction},
                { ProtocolAction.Broadcast, OnBroadcastAction },
                { ProtocolAction.Accept, OnAcceptAction },
                { ProtocolAction.Decline, OnDeclineAction }
            
            };

            _inputActions = new Dictionary<string, Action<string>>
            {
                { "/help", OnInputHelpAction },
                { "/rolldice", OnInputRollDiceAction },
                { "/closegame", OnCloseGameAction },
                {$"/{someInt}" ,OnIntAction },
                {"/search", OnSearchAction }
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

            Console.WriteLine("Type Search for Servers");

           
            isRunning = true;
            while (isRunning)
            {
                var input = Console.ReadLine();
                ParseAndExecuteCommand(input);
            }

        }

        private void ParseAndExecuteCommand(string input)
        {
            if (_inputActions.TryGetValue(input, out var action) == false)
            {
                Console.WriteLine($"Invalid command: {input}");
                return;
            }

            action(input);
        }

        #region Input actions

        private void OnInputHelpAction(string obj)
        {
            if (!isConnected)
                return;
            var dataPackage = new DataPackage
            {
                
                Header = ProtocolAction.GetHelp, //"Client_wants_to_get_help",
                Payload = JsonConvert.SerializeObject(new PROT_HELP
                {

                    _Client_id = 5 //actual implementation: smth like this Player_ID = CurrentPawn.playerId;

                })
                
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            _communication.Send(dataPackage);
        }

        private void OnInputRollDiceAction(string obj)
        {
            if (!isConnected)
                return;

            var dataPackage = new DataPackage
            {
                Header = ProtocolAction.RollDice, //"Client_wants_to_rolldice",
                Payload = JsonConvert.SerializeObject(new PROT_ROLLDICE
                {
                    _Client_id = 2 //Player_ID = actual implementation: smth like this(CurrentPawn.playerId)

                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            _communication.Send(dataPackage);
        }
        
        public PROT_BROADCAST GetServer(int key) => _serverDictionary[key];

        private void OnIntAction(string obj)
        {
            if (isConnected)
                return;

            int chosenServerId = Int32.Parse(obj);
            if(_serverDictionary.Count >= chosenServerId)
            {
                PROT_BROADCAST current = GetServer(chosenServerId);
                _communication._client.Connect(IPAddress.Parse(current._Server_ip),8080);
                isConnected = true;
            }
            
        }

        private void OnSearchAction(string obj)
        {
            if (isConnected)
                return;


            _udplistener.StartListening();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while(stopwatch.ElapsedMilliseconds < 10000)
            {
                if (_udplistener.data != null)
                    _communication.AddPackage(_udplistener.data);
            }
            stopwatch.Stop();
            _udplistener.StopListening();
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

            Console.Write("Received help text: " + helpText._Text);
        }

        private void OnUpdateAction(DataPackage data)
        {
            var updatedView = CreateProtocol<PROT_UPDATE>(data);

            Console.WriteLine("Received update: " + updatedView._Updated_board+"\n"+updatedView._Updated_dice_information
                +"\n"+updatedView._Updated_turn_information);
        }

        private string[] _Servernames = new string[100];
        private int[] _MaxPlayerCount = new int[100];
        private int[] _CurrentPlayerCount = new int[100];
        private int keyIndex = 1;
        

        private void OnBroadcastAction(DataPackage data)
        {
            var broadcast = CreateProtocol<PROT_BROADCAST>(data);
            _serverDictionary.Add(keyIndex, broadcast);
            
            _Servernames[keyIndex] = broadcast._Server_name;
            _MaxPlayerCount[keyIndex] = broadcast._MaxPlayerCount;
            _CurrentPlayerCount[keyIndex] = broadcast._CurrentPlayerCount;

            Console.WriteLine(string.Format("{0,10} {1,10}\n\n", "Server", "Player"));

            var outputFormat = new StringBuilder();

            for (int index = 0; index < _serverDictionary.Count; index++)
                outputFormat.Append(string.Format("{0,20} [{1,2}/{2,2}]\n", _Servernames[index],
                    _CurrentPlayerCount[index], _MaxPlayerCount[index]));

            Console.WriteLine(outputFormat);
        //      Server  Player  
        //
        //      XD      [0/4]
        //      LuL     [1/2]
            keyIndex++;
        }

        private void OnAcceptAction(DataPackage data)
        {

            if (i == 0)
            {
                var accept = CreateProtocol<PROT_ACCEPT>(data);

                Console.WriteLine(accept._Message);
            }
            else Console.WriteLine("Error: You are already connected.");

            _game.Init();

            i++;

        }

        private void OnDeclineAction(DataPackage data)
        {
            var decline = CreateProtocol<PROT_DECLINE>(data);

            Console.WriteLine(decline._Message);
            
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

