
using System;
using System.Net.Sockets;

namespace TCP_Model.Contracts
{
    public interface ICommunication
    {
        bool IsDataAvailable();
        void AddPackage(DataPackage dataPackage);
        DataPackage Receive();
        void ReceiveCallback(Action<DataPackage> receiveCallback);
        void Send(DataPackage data);
        TcpClient _client { get; set; }

    }
}
