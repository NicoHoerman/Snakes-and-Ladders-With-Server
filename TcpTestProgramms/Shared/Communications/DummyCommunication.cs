using Shared.Contract;
using System;
using System.Net.Sockets;


namespace Shared.Communications
{

    public class DummyCommunication : ICommunication
    {

        public TcpClient _client { get; set; }

        public bool IsDataAvailable()
        {
            return false;
        }

        public DataPackage Receive()
        {
            throw new NotImplementedException();
        }

        public void ReceiveCallback(Action<DataPackage> receiveCallback)
        {
            throw new NotImplementedException();
        }

        public void Send(DataPackage data)
        {
            Console.WriteLine($"{data.Header}: {data.Payload}");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void AddPackage(DataPackage dataPackage)
        {
            throw new NotImplementedException();
        }

        public void GetStream()
        {
            throw new NotImplementedException();
        }

        public void SetNWStream()
        {
            throw new NotImplementedException();
        }
    }
}
