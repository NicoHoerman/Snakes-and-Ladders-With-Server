using System;
using System.ComponentModel;
using System.Threading;
using Wrapper.Implementation;
using Shared.Contract;
using TCP_Client.Actions;
using Shared.Communications;

namespace TCP_Client
{

    public class Client
    {
        public bool isRunning;
        private string _requiredString = string.Empty;

        private string _serverTable = string.Empty;
        private string _afterConnectMsg = string.Empty;
        private string _errorMsg = string.Empty;
        private string _UpdatedView = string.Empty;


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

        private void CheckTCPUpdates()
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

            backgroundworker.DoWork += (obj, ea) => CheckTCPUpdates();
            backgroundworker.RunWorkerAsync();

            string input = string.Empty;
            isRunning = true;

            while (isRunning)
            {

                _OutputWrapper.Updateview(input, _afterConnectMsg, _serverTable,_errorMsg,_UpdatedView);
                _serverTable = string.Empty;
                _afterConnectMsg = string.Empty;
                _errorMsg = string.Empty;
                _UpdatedView = string.Empty;


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
            _errorMsg = _InputHandler._errorMsg;
            _UpdatedView = _ActionHanlder._UpdatedView;

        }
    }
}

