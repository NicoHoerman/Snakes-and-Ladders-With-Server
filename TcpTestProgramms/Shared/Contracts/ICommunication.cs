using Shared.Communications;
using System;
using System.Net.Sockets;

namespace Shared.Contract
{
    public interface ICommunication
    {
        bool IsDataAvailable();
        void AddPackage(DataPackage dataPackage);
        DataPackage Receive();
        void ReceiveCallback(Action<DataPackage> receiveCallback);
        void Send(DataPackage data);
        TcpClient _client { get; set; }
        //NetworkStream GetStream();
        void SetNWStream();
        bool IsMaster { get; set; }

        bool IsConnected { get; }
        void Stop();
    }
}
