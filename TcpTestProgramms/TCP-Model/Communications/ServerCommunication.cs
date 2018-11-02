using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP_Model
{
    public class ServerCommunication : ICommunication
    {
        const int PORT_NO = 8080;
        const string SERVER_IP = "127.0.0.1";

        private IPAddress ipAddress;
        private TcpListener listener;
        private TcpClient client;
        private NetworkStream nwStream;

        public ServerCommunication()
        {
            ipAddress = IPAddress.Parse(SERVER_IP);
            listener = new TcpListener(ipAddress, PORT_NO);
        }


        public void ConnectClient()
        {
            listener.Start();
            client = listener.AcceptTcpClient();
            nwStream = client.GetStream();
        }
        
        public bool IsDataAvailable()
        {
            nwStream = client.GetStream();
            return nwStream.DataAvailable;
        }

        public DataPackage Receive()
        {
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received : " + dataReceived);

            return JsonConvert.DeserializeObject<DataPackage>(dataReceived);
        }

        public void ReceiveCallback(Action<DataPackage> receiveCallback)
        {
            throw new NotImplementedException();
        }

        public void Send(DataPackage data)
        {
            var dataToSend = JsonConvert.SerializeObject(data);

            byte[] bytesToSend = Encoding.ASCII.GetBytes(dataToSend);

            Console.WriteLine("Sending : " + dataToSend);
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }
    
    }
}
