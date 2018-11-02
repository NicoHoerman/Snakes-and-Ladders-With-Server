using System;


namespace TCP_Model
{
    public interface ICommunication
    {
        bool IsDataAvailable();
        DataPackage Receive();
        void ReceiveCallback(Action<DataPackage> receiveCallback);
        void Send(DataPackage data);

    }
}
