using System;
using System.Net.Sockets;

namespace TCP_Server.Test
{
    public class Validator
    {

        public void Validate()
        {
            //Validation
            bool ValidationSucced = true;
            if (ValidationSucced)
               Core.status = ValidationEnum.LobbyCheck;
            else
               Core.status = ValidationEnum.DeclineState;

        }
    }
}
