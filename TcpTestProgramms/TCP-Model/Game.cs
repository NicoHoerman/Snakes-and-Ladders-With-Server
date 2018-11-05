using System;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;

namespace TCP_Model
{

    public class Game
    {

        private string updateInfo = string.Empty;

        private Dictionary<ProtocolAction, Action<DataPackage>> _protocolActions;
        private Dictionary<string, Action<string>> _inputActions;

        private ICommunication _communication;

        public Game(ICommunication communication)
        {
            _communication = communication;

            _protocolActions = new Dictionary<ProtocolAction, Action<DataPackage>>
            {
                { ProtocolAction.RollDice, OnRollDiceAction },
                { ProtocolAction.UpdateView, OnUpdateAction}
            

            };
            _inputActions = new Dictionary<string, Action<string>>
            {
                { "/help", OnInputHelpAction },
                {"/rolldice",OnInputRollDiceAction }
            };
        }

        public Game()
            :this(new TcpCommunication())
        {
        }


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
            if (_protocolActions.TryGetValue(data.Header, out var protocolAction) == false)
                throw new InvalidOperationException("Invalid communication");

            protocolAction(data);
        }

        public void Run()
        {
            var backgroundworker = new BackgroundWorker();

            backgroundworker.DoWork += (obj, ea) => CheckForUpdates();
            backgroundworker.RunWorkerAsync();

            Console.WriteLine("Test JSON packages ");

            var isRunning = true;
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
            var dataPackage = new DataPackage
            {
                Header = ProtocolAction.GetHelp, //"Client_wants_to_get_help",
                Payload = JsonConvert.SerializeObject(new PROT_HELP
                {
                    Client_IP = "127.0.0.1"
                })
            };

            _communication.Send(dataPackage);
        }

        private void OnInputRollDiceAction(string obj)
        {
            var dataPackage = new DataPackage
            {
                Header = ProtocolAction.RollDice, //"Client_wants_to_rolldice",
                Payload = JsonConvert.SerializeObject(new PROT_ROLLDICE
                {
                    Client_IP = "127.0.0.1",
                    Its_the_clients_turn = true
                })
            };

            _communication.Send(dataPackage);
        }



        #endregion


        #region Protocol actions

        private void OnRollDiceAction(DataPackage data)
        {
            var protocol = CreateProtocol<PROT_ROLLDICE>(data);
            Console.WriteLine(protocol.Client_IP);
        }

        private void OnUpdateAction(DataPackage data)
        {
            var protocol = CreateProtocol<PROT_UPDATE>(data);
            Console.WriteLine("Update bekommen " + protocol.Updated_Board);
        }
        #endregion


        #region Static helper functions

        private static T CreateProtocol<T>(DataPackage data) where T : IProtocol
        {
            return JsonConvert.DeserializeObject<T>(data.Payload);
        }

        #endregion
    }
}

/* IPAddress leonsadress = IPAddress.Parse("172.22.22.184");
 IPAddress myadress = IPAddress.Parse("172.22.22.153");
*/

//TCP_Client client = new TCP_Client();

//Console.WriteLine("Suche nach Servern");

//case "/rolldice":
//    var rolldiceData = new PROT_ROLLDICE
//    {
//        Client_IP = "127.0.0.1",
//        Its_the_clients_turn = true,
//    };

//    dataPackage.Header = "Client_wants_to_rolldice";
//    dataPackage.Payload = JsonConvert.SerializeObject(rolldiceData);

//    _communication.Send(dataPackage);
//    break;

