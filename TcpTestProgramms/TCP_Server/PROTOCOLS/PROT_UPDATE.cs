﻿using Shared.Contracts;

namespace TCP_Server.PROTOCOLS
{
    public class PROT_UPDATE : IProtocol
    {       
        public string _mainMenuOutput;
        public string _additionalInformation;
        public string _lastinput;
        public string _error;
        public string _gameInfoOutput;
        public string _boardOutput;
        public string _turnInfoOutput;
        public string _afterTurnOutput;
        public string _finishinfo;
        public string _finishskull1;
        public string _finishskull2;
        public string _lobbyDisplay;
        public string _commandList;
        public string _infoOutput;
        public string _enterToRefresh;
    }
}
