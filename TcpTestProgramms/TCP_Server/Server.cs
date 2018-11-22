using Shared.Communications;
using Shared.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TCP_Server.Actions;
using TCP_Server.Enum;
using TCP_Server.UDP;

namespace TCP_Server
{
    public class Server
    {
        private const string SERVER_IP_WLAN = "172.22.21.132";
        private const string SERVER_IP_LAN = "172.22.22.207";
        
        //private string _updateInfo = string.Empty;
        private bool isRunning;
        
       
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        
        private TcpListener _listener;
        private ServerInfo _serverInfo;
        private ServerActions _ActionsHandler;
        private UdpBroadcast _udpServer;
        private TcpClient _client;

        public Server(ServerInfo serverInfo,UdpBroadcast udpBroadcast)
        {
            _serverInfo = serverInfo;
            _udpServer = udpBroadcast;
            _ActionsHandler = new ServerActions(_serverInfo);

            _serverInfo._communications = new List<ICommunication>();

            _listener = new TcpListener(IPAddress.Parse(SERVER_IP_LAN), 8080);
            
        }

        public void CLientConnection(TcpListener listener)
        {
            _listener.Start();

            while (isRunning)
            {
                DoBeginAcceptTcpClient(listener);
                if (_client != null)
                {
                    
                    if (!isLobbyComplete())
                    {
                        _ActionsHandler._ConnectionStatus = ClientConnectionAttempt.Accepted;
                        AddCommunication(_client);
                        _client = null;
                        _serverInfo._CurrentPlayerCount++;
                        _udpServer.SetBroadcastMsg(_serverInfo);
                        Console.WriteLine("Test in client Connection");
                    }
                    else if (isLobbyComplete())
                    {
                        _ActionsHandler._ConnectionStatus = ClientConnectionAttempt.Declined;
                    }

                    ServerActions.verificationVariableSet.Set();
                }
                Thread.Sleep(1000);
            }
        }

        public void DoBeginAcceptTcpClient(TcpListener listener)
        {

            listener.BeginAcceptTcpClient(
                new AsyncCallback(DoAcceptTcpClientCallback),
                listener);

            tcpClientConnected.WaitOne();
            tcpClientConnected.Reset();
        }

        public void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            _client = listener.EndAcceptTcpClient(ar);
            
            //Conected
            Console.WriteLine("Client connected completed");
            
            tcpClientConnected.Set();
        }

        public bool isLobbyComplete()
        {
            if (_serverInfo._communications.Count == _serverInfo._MaxPlayerCount)
            {
                return true;
            }
            else
                return false;
        }

        public void AddCommunication(TcpClient client)
        {
            _serverInfo._communications.Add(
                new TcpCommunication(client));

            if (_serverInfo._communications.Count == 1)
                _serverInfo._communications[0].IsMaster = true;

            _serverInfo.PrintPlayerIP();
            _client = null;
        } 

        public void Run()
        {
            isRunning = true;

            var backgroundworker = new BackgroundWorker();

            backgroundworker.DoWork += (obj, ea) => CheckForUpdates();
            backgroundworker.RunWorkerAsync();

            var backgroundworker2 = new BackgroundWorker();

            backgroundworker2.DoWork += (obj, ea) => CLientConnection(_listener);
            backgroundworker2.RunWorkerAsync();

            while (isRunning)
            {

            }

        }

        private void CheckForUpdates()
        {
            var elementsToRemove = new List<ICommunication>();
            while (isRunning)
            {
                elementsToRemove.Clear();
                _serverInfo._communications.ForEach(communication =>
                {
                    if (!communication.IsConnected)
                    {
                        communication.Stop();
                        elementsToRemove.Add(communication);
                    }
                    else
                    {
                        if (communication.IsDataAvailable())
                        {
                            var data = communication.Receive();
                            _ActionsHandler.ExecuteDataActionFor(communication, data);
                            //communicated = true;
                        }
                    }
                });

                // All elements that lost conenction!
                if(elementsToRemove.Count > 0)
                {
                    elementsToRemove.ForEach(x => _serverInfo._CurrentPlayerCount--);
                    _udpServer.SetBroadcastMsg(_serverInfo);
                    elementsToRemove.ForEach(x => _serverInfo._communications.Remove(x));
                }

                Thread.Sleep(1);
            }
          
        }

        


       /* #region Accept and Decline
        private void DeclineClient(ICommunication communication, DataPackage data, PROT_CONNECT clientId)
        {
            
            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Decline,
                Payload = JsonConvert.SerializeObject(new PROT_DECLINE
                {
                    _Message = $"Your connection was declined because the Client ID({clientId._Client_id}) is on our Blacklist."
                    
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);

            communication._client.Close();

        }

        private void AcceptClient(ICommunication communication, DataPackage data, PROT_CONNECT clientId)
        {

            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Accept,
                Payload = JsonConvert.SerializeObject(new PROT_ACCEPT
                {
                    _Message = "Congratulations! Your client has been accepted."
                    + "\nGet ready to play a fun round of Eels and Escalators!"
                    + $"\nYou have  been assigned player number {clientId._Client_id}."
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }
        #endregion
        */
       
    }
}
