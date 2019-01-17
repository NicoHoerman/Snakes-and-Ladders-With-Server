using Newtonsoft.Json;
using Shared.Communications;
using Shared.Enums;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TCP_Server.PROTOCOLS;
using TCP_Server.Support;

namespace TCP_Server.UDP
{
    public class UdpBroadcast
    {
        private byte[] _lobbyInfo;

        private const string SERVER_IP_WLAN_NICO = "172.22.21.132";
        private const string SERVER_IP_LAN_NICO = "172.22.23.88";
        private const string SERVER_IP_LAN_LEON = "172.22.23.87";
        private const string SERVER_IP_NETWORK = "194.205.205.2";

        private bool _closed = false;
        private bool _isRunning;


        private static ManualResetEvent _MessageReceived = new ManualResetEvent(true);
        private UdpClient _udpServer;
        private IPAddress _clientIP;
		private ServerInfo _serverInfo;

        public UdpBroadcast(ServerInfo serverInfo)
        {
            _udpServer = new UdpClient(7070);
            _serverInfo = serverInfo;
        }

        public void RunUdpServer()
        {
            _isRunning = true;
            while (_isRunning)
            {
                _MessageReceived.WaitOne();
                _MessageReceived.Reset();
                StartListening();
            }
        }
        public void Broadcast(string clientIp)
        {
            _clientIP = IPAddress.Parse(clientIp);
            IPEndPoint _ipEndPoint = new IPEndPoint(_clientIP, 7075);
            _udpServer.Send(_lobbyInfo, _lobbyInfo.Length, _ipEndPoint);
            Thread.Sleep(5000);
        }

        public void StartListening()
        {
            if (_udpServer.Client != null)
                _udpServer.BeginReceive(Receive, new object());
        }

        public void Receive(IAsyncResult ar)
        {
            if (!_closed)
            {
                IPEndPoint _recieverEndPoint = new IPEndPoint(IPAddress.Any, 7070);
                byte[] bytes = _udpServer.EndReceive(ar, ref _recieverEndPoint);
                string recievedMessage = Encoding.ASCII.GetString(bytes);
                if(recievedMessage == "is there a Server")
                {
                    SetBroadcastMsg(_serverInfo);
                    Broadcast(_recieverEndPoint.Address.ToString());
                }
            }
            Thread.Sleep(1);
            _MessageReceived.Set();
        }

        public void SetBroadcastMsg(ServerInfo serverInfo)
        {
            Random random = new Random();
            DataPackage dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Broadcast,
                Payload = JsonConvert.SerializeObject(new PROT_BROADCAST
                {
                    _server_ip = SERVER_IP_LAN_NICO,
                    _server_name = serverInfo._lobbylist[0]._LobbyName,
                    _currentPlayerCount = serverInfo._lobbylist[0]._CurrentPlayerCount,
                    _maxPlayerCount = serverInfo._lobbylist[0]._MaxPlayerCount,
                    _server_Port = serverInfo._lobbylist[0]._ServerPort
                    
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;
            _lobbyInfo = dataPackage.ToByteArray();
        }
    }
}



    

