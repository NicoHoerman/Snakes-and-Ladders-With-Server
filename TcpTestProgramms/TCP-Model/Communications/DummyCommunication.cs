using System;
using Newtonsoft.Json;


namespace TCP_Model
{

    public class DummyCommunication : ICommunication
    {
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
    }
}
