﻿using System;
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
using Shared.Contracts;
using TCP_Client.StateEnum;

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
        private ViewDictionary _viewDictionary;
        string input = string.Empty;
        public ClientStates state { get; set; }
        
        //<Constructors>
        public Client(ICommunication communication)
        {
            _viewDictionary = new ViewDictionary();
            _communication = communication;
            _ActionHandler = new ProtocolAction(_viewDictionary._views, this);
            _InputHandler = new InputAction(_ActionHandler, _viewDictionary._views,this);
            _OutputWrapper = new OutputWrapper();
            _ViewUpdater = new ViewUpdater(_viewDictionary._views);
            _ActionHandler._enterToRefreshView.viewEnabled = true;
            _ActionHandler._enterToRefreshView.SetUpdateContent("Press enter to refresh\nafter you typed a command.");
            
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

            state = ClientStates.NotConnected;
            var backgroundworker3 = new BackgroundWorker();

            backgroundworker3.DoWork += (obj, ea) => StateMachine(state);
            backgroundworker3.RunWorkerAsync();

            
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

        public void StateMachine(ClientStates state)
        {
            while (isRunning)
            {
                switch (state)
                {
                    case ClientStates.NotConnected:
                        _InputHandler._inputActions.Add("/search", _InputHandler.OnSearchAction);
                        _InputHandler._inputActions.Add("/someInt", _InputHandler.OnIntAction);
                        input = _OutputWrapper.ReadInput();
                        _InputHandler.ParseAndExecuteCommand(input, _communication);                       
                        break;

                    case ClientStates.Connecting:
                        _InputHandler._inputActions.Clear();
                        WaitForHandshake();
                        break;

                    case ClientStates.Connected:
                        break;

                    case ClientStates.GameRunning:
                        break;

                    case ClientStates.Handshake:
                        break;

                    case ClientStates.Lobby:
                        break;
                }
            }
            
        }

        private void WaitForHandshake()
        {
            
        }

        public void SwitchState(ClientStates newState)
        {
            state = newState;
        }
    }
}

