using System;
using System.Net.Sockets;

namespace TCP_Server.Test
{
    public class Validator
    {
        public Validator(TcpClient client)
        {
            Validate(client);
        }

        public void Validate(TcpClient client)
        {
            //Validation
            bool ValidationSucced = true;
            if (ValidationSucced)
                Server.status = ValidationEnum.LobbyCheck;
            else
                Server.status = ValidationEnum.DeclineState;

        }
    }
}
