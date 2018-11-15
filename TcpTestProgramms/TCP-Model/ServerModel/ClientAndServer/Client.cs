using System;
using System.ComponentModel;
using System.Threading;
using EandE_ServerModel.ServerModel.Contracts;
using EandE_ServerModel.ServerModel.Communications;
using EandE_ServerModel.ServerModel.ProtocolActionStuff;
using TCP_Model.ServerModel.InputActionStuff;
using TCP_Model.ServerModel;
using System.Collections.Generic;
using System.Linq;

namespace EandE_ServerModel.ServerModel.ClientAndServer
{

    public class Client
    {
        public bool isRunning;
        private string _requiredString = string.Empty;

        public string _serverTable = string.Empty;
        public string _afterConnectMsg = string.Empty;


        private ICommunication _communication;
        private ProtocolAction _ActionHanlder;
        private InputAction _InputHandler;
        private OutputWrapper _OutputWrapper;

        //<Constructors>
        public Client(ICommunication communication)
        {
            _communication = communication;
            _ActionHanlder = new ProtocolAction();
            _InputHandler = new InputAction(_ActionHanlder);
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

            string input = string.Empty;
            isRunning = true;

            while (isRunning)
            {

                _OutputWrapper.Updateview(input, _afterConnectMsg, _serverTable);
                _serverTable = string.Empty;
                _afterConnectMsg = string.Empty;


                Console.SetCursorPosition(17, 0);
                input = Console.ReadLine();
                _InputHandler.ParseAndExecuteCommand(input, _communication);
                SetParameters();
                
            }
        }

        private void SetParameters()
        {
            _afterConnectMsg = _InputHandler._afterConnectMsg;
            _serverTable = _ActionHanlder._serverTable;
        }
    }
}

