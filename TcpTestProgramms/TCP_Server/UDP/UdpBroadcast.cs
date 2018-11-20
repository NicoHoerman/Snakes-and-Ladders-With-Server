using Newtonsoft.Json;
using Shared.Communications;
using Shared.Enums;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TCP_Server.PROTOCOLS;

namespace TCP_Server.UDP
{
    public class UdpBroadcast
    {
        private byte[] _ServerInfo;
        private const string SERVER_IP_WLAN = "172.22.21.132";
        private const string SERVER_IP_LAN = "172.22.22.153";
        private bool _closed = false;

        private static ManualResetEvent _MessageSent = new ManualResetEvent(false);
        UdpClient _udpServer;
        IPAddress ClientIP;

        public UdpBroadcast()
        {
            _udpServer = new UdpClient(7070);
        }

        public void Broadcast(string clientIp)
        {
            SetBroadcastMsg();
            ClientIP = IPAddress.Parse(clientIp);
            IPEndPoint _ipEndPoint = new IPEndPoint(ClientIP, 7071);
            _udpServer.Send(_ServerInfo, _ServerInfo.Length, _ipEndPoint);
            _MessageSent.Set();
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
                Broadcast(_recieverEndPoint.Address.ToString());

                _MessageSent.WaitOne();

                _udpServer.Dispose();
                _udpServer.Close();
            }
            Thread.Sleep(1);
            StartListening();
        }

        public void SetBroadcastMsg()
        {
            Random random = new Random();
            DataPackage dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Broadcast,
                Payload = JsonConvert.SerializeObject(new PROT_BROADCAST
                {
                    _Server_ip = SERVER_IP_LAN,
                    _Server_name = "Test_1",
                    _CurrentPlayerCount = random.Next(0, 4),
                    _MaxPlayerCount = 4,
                    _Server_Port = 8080
                    
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;
            _ServerInfo = dataPackage.ToByteArray();
        }
    }
}



    

