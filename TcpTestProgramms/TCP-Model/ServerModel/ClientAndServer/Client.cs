using System;
using System.ComponentModel;
using System.Threading;
using EandE_ServerModel.ServerModel.Contracts;
using EandE_ServerModel.ServerModel.Communications;
using EandE_ServerModel.ServerModel.ProtocolActionStuff;
using TCP_Model.ServerModel.InputActionStuff;

namespace EandE_ServerModel.ServerModel.ClientAndServer
{

    public class Client
    {
        public bool isRunning;
        
        private ICommunication _communication;
        private ProtocolAction _ActionHanlder;
        private InputAction _InputHandler;

        //<Constructors>
        public Client(ICommunication communication)
        {
            _communication = communication;
            _ActionHanlder = new ProtocolAction();
            _InputHandler = new InputAction();
        }

        public Client()
            : this(new TcpCommunication())
        { }
        
        //<Methods>

        private void CheckForUpdates()
        {
            while (true)
            {
                if (_communication.IsDataAvailable())
                {
                    var data = _communication.Receive();
                    _ActionHanlder.ExecuteDataActionFor(data);
                }
                else
                    Thread.Sleep(1);
            }
        }

        public void Run()
        {
            var backgroundworker = new BackgroundWorker();

            backgroundworker.DoWork += (obj, ea) => CheckForUpdates();
            backgroundworker.RunWorkerAsync();


            isRunning = true;
            while (isRunning)
            {
                //Console.Clear();

                Console.WriteLine("Type /search for Servers");
                Console.SetCursorPosition(0,1);
                Console.WriteLine("Type an Command: ");
                Console.SetCursorPosition(17,1);

                var input = Console.ReadLine();
                _InputHandler.ParseAndExecuteCommand(input,_communication);
            }

        }

    }
}

