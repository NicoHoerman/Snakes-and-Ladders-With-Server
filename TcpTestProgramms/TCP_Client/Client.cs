using System;
using System.ComponentModel;
using System.Threading;
using Wrapper.Implementation;
using Shared.Contract;
using TCP_Client.Actions;
using Shared.Communications;
using System.Collections.Generic;
using System.Linq;
using Wrapper;
using Wrapper.Contracts;
using Wrapper.View;

namespace TCP_Client
{

    public class Client
    {
        public bool isRunning;

        private ICommunication _communication;
        private ProtocolAction _ActionHandler;
        private InputAction _InputHandler;
        private OutputWrapper _OutputWrapper;

        private Dictionary<ClientView, IView> _views = new Dictionary<ClientView, IView>
        {
            { ClientView.Error, new ErrorView() },
            { ClientView.ServerTable, new ServerTableView() },
            { ClientView.InfoOutput, new InfoOutputView() },
            { ClientView.HelpOutput, new HelpOutputView() },
            { ClientView.Input, new InputView() }
        };

        //<Constructors>
        public Client(ICommunication communication)
        {
            _communication = communication;
            _ActionHandler = new ProtocolAction(_views);
            _InputHandler = new InputAction(_ActionHandler, _views);
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
                    _ActionHandler.ExecuteDataActionFor(data);
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

            var backgroundworker2 = new BackgroundWorker();

            backgroundworker2.DoWork += (obj, ea) => _OutputWrapper.UpdateView();
            backgroundworker2.RunWorkerAsync();

            string input = string.Empty;
            isRunning = true;

            while (isRunning)
            {
                Console.SetCursorPosition(_InputHandler._inputView._xCursorPosition, 0);
                input = _OutputWrapper.ReadInput();
                _OutputWrapper.Clear();
                _InputHandler.ParseAndExecuteCommand(input, _communication);                
            }
        }        
    }
}

