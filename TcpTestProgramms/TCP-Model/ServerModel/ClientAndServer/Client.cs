using System;
using System.ComponentModel;
using System.Threading;
using EandE_ServerModel.ServerModel.Contracts;
using EandE_ServerModel.ServerModel.Communications;
using EandE_ServerModel.ServerModel.ProtocolActionStuff;
using TCP_Model.ServerModel.InputActionStuff;
using TCP_Model.ServerModel;

namespace EandE_ServerModel.ServerModel.ClientAndServer
{

    public class Client
    {
        public bool isRunning;
        
        private ICommunication _communication;
        private ProtocolAction _ActionHanlder;
        private InputAction _InputHandler;
        private OutputWrapper _OutputWrapper;

        //<Constructors>
        public Client(ICommunication communication)
        {
            _communication = communication;
            _ActionHanlder = new ProtocolAction();
            _InputHandler = new InputAction();
            _OutputWrapper = new OutputWrapper();
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
                _OutputWrapper.ShowSomething();

                
                var input = Console.ReadLine();
                _InputHandler.ParseAndExecuteCommand(input,_communication);
            }

        }

    }
}

