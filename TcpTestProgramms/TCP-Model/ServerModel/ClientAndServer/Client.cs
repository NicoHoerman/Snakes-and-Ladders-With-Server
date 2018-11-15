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

        private Dictionary<string, string> _requiredStringForInput = new Dictionary<string, string>();

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

            _requiredStringForInput = new Dictionary<string, string>()
            {
                { "/search",_ActionHanlder._serverTable}
                
            };
        }

        public Client()
            : this(new TcpCommunication())
        { }
        
        //<Methods>
        private string GetString(string key) => _requiredStringForInput[key];

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

                _OutputWrapper.Updateview(input,_requiredString);

                Console.SetCursorPosition(17, 0);
                 input = Console.ReadLine();
                _InputHandler.ParseAndExecuteCommand(input,_communication);
                ChooseString(input);
            }

        }

        private void ChooseString(string _input)
        {

            if (_input == "/search")
                _requiredString = _ActionHanlder._serverTable;
            else if (_input.All(char.IsDigit))
                _requiredString = _InputHandler.isRightInt;
            else
                _requiredString = string.Empty;



            //try
            //{
            //    requiredString = GetString(_input);
            //}
            //catch(Exception e)
            //{
            //    //log
            //    requiredString = string.Empty;
            //}

        }

    }
}

