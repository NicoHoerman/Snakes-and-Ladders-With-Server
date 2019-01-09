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
               Core.ValidationStatus = ValidationEnum.LobbyCheck;
            else
               Core.ValidationStatus = ValidationEnum.DeclineState;

        }
    }
}
