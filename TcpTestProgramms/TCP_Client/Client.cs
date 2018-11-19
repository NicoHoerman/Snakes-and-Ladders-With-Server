using System;
using System.ComponentModel;
using System.Threading;
using Wrapper.Implementation;
using Shared.Contract;
using TCP_Client.Actions;
using Shared.Communications;
using Wrapper.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace TCP_Client
{
    public enum ClientView
    {
        Error,
        ServerTable,
    }

    public class Client
    {
        public bool isRunning;
        private string _requiredString = string.Empty;

        private string _serverTable = string.Empty;
        private string _afterConnectMsg = string.Empty;
        private string _UpdatedView = string.Empty;


        private ICommunication _communication;
        private ProtocolAction _ActionHandler;
        private InputAction _InputHandler;
        private OutputWrapper _OutputWrapper;

        private Dictionary<ClientView, IView> _views = new Dictionary<ClientView, IView>
        {
            { ClientView.Error, new ErrorView() },
            { ClientView.ServerTable, new ServerTableView() }
        };

        //<Constructors>
        public Client(ICommunication communication)
        {
            _communication = communication;
            _ActionHandler = new ProtocolAction();
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

            string input = string.Empty;
            isRunning = true;

            while (isRunning)
            {

                _OutputWrapper.Updateview(input, _afterConnectMsg, _serverTable,string.Empty,_UpdatedView);
                _views.Values.ToList().ForEach(x => x.Show());

                _serverTable = string.Empty;
                _afterConnectMsg = string.Empty;
                _UpdatedView = string.Empty;


                Console.SetCursorPosition(17, 0);
                input = Console.ReadLine();
                _InputHandler.ParseAndExecuteCommand(input, _communication);

                SetParameters();
                
            }
        }

        private void SetParameters()
        {
            _afterConnectMsg = _InputHandler.AfterConnectMsg;
            _serverTable = _ActionHandler._serverTable;
            _UpdatedView = _ActionHandler._UpdatedView;

        }
    }

    public interface IView
    {
        void Show();
    }

    public interface IErrorView : IView
    {
        void SetContent(string lastInput, string errorMessage);
    }

    public class ErrorView : IErrorView
    {
        public const int DEFAULT_POSITION_X = 20;
        public const int DEFAULT_POSITION_Y = 10;

        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;
        private string _lastInput = string.Empty;
        private string _errorMessage = string.Empty;

        public ErrorView(IOutputWrapper outputWrapper, int posX, int posY)
        {
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
        }

        public ErrorView()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y)
        { }


        public void SetContent(string lastInput, string errorMessage)
        {
            _lastInput = lastInput;
            _errorMessage = errorMessage;
        }


        public void Show()
        {
            _outputWrapper.WriteOutput(_posX, _posY, _lastInput, ConsoleColor.DarkRed);
            _outputWrapper.WriteOutput(_posX, _posY + 1, _errorMessage, ConsoleColor.Red);
        }
    }

    internal class ServerTableView : IView
    {
        public ServerTableView()
        {

        }

        public void Show()
        {
            throw new NotImplementedException();
        }
    }


}

