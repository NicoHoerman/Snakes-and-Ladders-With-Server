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
        public InputAction _InputHandler;
        private OutputWrapper _OutputWrapper;
        private ViewUpdater _ViewUpdater;

        public Dictionary<ClientView, IView> _views = new Dictionary<ClientView, IView>
        {
            { ClientView.Error, new ErrorView() },
            { ClientView.ServerTable, new ServerTableView() },
            { ClientView.InfoOutput, new InfoOutputView() },
            { ClientView.CommandList, new CommandListOutputView() },
            { ClientView.Input, new InputView() },
            { ClientView.MenuOutput, new MainMenuOutputView() },
            { ClientView.TurnInfo, new TurnInfoOutputView() },
            { ClientView.GameInfo, new GameInfoOutputView() },
            { ClientView.AfterTurnOutput, new AfterTurnOutputView() },
            { ClientView.Board, new BoardOutputView() },
            { ClientView.LobbyInfoDisplay, new LobbyInfoDisplayView() },
            { ClientView.FinishInfo, new FinishInfoOutputView() },
            { ClientView.FinishSkull1, new FinishSkull1View() },
            { ClientView.FinishSkull2, new FinishSkull2View() }
        };

        //<Constructors>
        public Client(ICommunication communication)
        {
            _communication = communication;
            _ActionHandler = new ProtocolAction(_views, this);
            _InputHandler = new InputAction(_ActionHandler, _views,this);
            _OutputWrapper = new OutputWrapper();
            _ViewUpdater = new ViewUpdater(_views);
        }

        public Client()
            : this(new TcpCommunication())
        { }

        //<Methods>

        private void CheckTCPUpdates()
        {
            while (isRunning)
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

            backgroundworker2.DoWork += (obj, ea) => _ViewUpdater.RunUpdater();
            //backgroundworker2.RunWorkerAsync();

            string input = string.Empty;
            isRunning = true;

            while (isRunning)
            {
                _ViewUpdater.UpdateView();
                Console.SetCursorPosition(_InputHandler._inputView._xCursorPosition, 0);
                input = _OutputWrapper.ReadInput();
                _OutputWrapper.Clear();
                _InputHandler.ParseAndExecuteCommand(input, _communication);
            }
        }    
        
        public void CloseCommunication()
        {
            _communication.Stop();
           
        }

        public void CloseClient()
        {
            
            _ViewUpdater.isViewRunning = false;
            _communication.Stop();
            isRunning = false;
        }

        
    }
}

