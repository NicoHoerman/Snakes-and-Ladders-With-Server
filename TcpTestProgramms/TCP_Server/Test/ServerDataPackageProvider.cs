﻿using Newtonsoft.Json;
using Shared.Communications;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCP_Server.PROTOCOLS;

namespace TCP_Server.Test
{
    public class ServerDataPackageProvider
    {
        Dictionary<string, DataPackage> _DataPackages;
        private DataPackage accpetedInfoPackage;
        private DataPackage declinedInfoPackage;
        private DataPackage lobbyDisplayPackage;
        private DataPackage declineUpdatePackage;
        private DataPackage validationRequestPackage;
        private ServerInfo _serverInfo;

        private string servername = string.Empty;
        private int currentplayer;
        private int maxplayer;

        public ServerDataPackageProvider(ServerInfo serverInfo)
        {
            _serverInfo = serverInfo;
            
            accpetedInfoPackage  = new DataPackage
            {
                Header = ProtocolActionEnum.AcceptInfo,
                Payload = JsonConvert.SerializeObject(new PROT_ACCEPT
                {
                    _SmallUpdate = "You are connected to the Server and in the Lobby "
                })
            };
            accpetedInfoPackage.Size = accpetedInfoPackage.ToByteArray().Length;
            declinedInfoPackage  = new DataPackage
            {
                Header = ProtocolActionEnum.DeclineInfo,
                Payload = JsonConvert.SerializeObject(new PROT_DECLINE
                {
                    _SmallUpdate = "You got declind. Lobby is probably full"
                })
            };
            declinedInfoPackage.Size = declinedInfoPackage.ToByteArray().Length;
            declineUpdatePackage = new DataPackage
            {
                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _infoOutput = "A player got declined"
                })
            };
            declineUpdatePackage.Size = declineUpdatePackage.ToByteArray().Length;

            validationRequestPackage = new DataPackage
            {
                Header = ProtocolActionEnum.ValidationRequest,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE { })
            };
            validationRequestPackage.Size = validationRequestPackage.ToByteArray().Length;

            DataPackage validationAcceptedPackage = new DataPackage
            {
                Header = ProtocolActionEnum.ValidationAccepted,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE { })
            };
            validationAcceptedPackage.Size = validationAcceptedPackage.ToByteArray().Length;

            var lobbyCheckFailedPackage = new DataPackage
            {
                Header = ProtocolActionEnum.LobbyCheckFailed,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE { })
            };
            lobbyCheckFailedPackage.Size = lobbyCheckFailedPackage.ToByteArray().Length;

            var lobbyCheckSuccessfulPackage = new DataPackage
            {
                Header = ProtocolActionEnum.LobbyCheckSuccessful,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE { })
            };
            lobbyCheckSuccessfulPackage.Size = lobbyCheckSuccessfulPackage.ToByteArray().Length;

            var serverStartingGamePackage = new DataPackage
            {
                Header = ProtocolActionEnum.ServerStartingGame,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE { })
            };
            serverStartingGamePackage.Size = serverStartingGamePackage.ToByteArray().Length;

            _DataPackages = new Dictionary<string, DataPackage>
            {
                {"AcceptedInfo" ,  accpetedInfoPackage },
                {"DeclinedInfo" ,  declinedInfoPackage },
                {"DeclineUpdate", declineUpdatePackage },
                {"ValidationRequest", validationRequestPackage },
                {"ValidationAccepted", validationAcceptedPackage },
                {"LobbyCheckFailed", lobbyCheckFailedPackage },
                {"LobbyCheckSuccessful", lobbyCheckSuccessfulPackage },
                {"ServerStartingGame", serverStartingGamePackage }
            };
        }

        public DataPackage GetPackage(string key) => _DataPackages[key];

        public DataPackage LobbyDisplay()
        {
            servername = _serverInfo.lobbylist[0]._LobbyName;
            currentplayer = _serverInfo.lobbylist[0]._CurrentPlayerCount;
            maxplayer = _serverInfo.lobbylist[0]._MaxPlayerCount;

            lobbyDisplayPackage = new DataPackage
            {
                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _lobbyDisplay = $"Current Lobby: {servername}. Players [{currentplayer} /{maxplayer}",
                    _commandList = "Commands: \n/search (only available when not connected to a server)" +
                    " \n /startgame \n/closegame \n /rolldice \n /someCommand"
                })
            };
            lobbyDisplayPackage.Size = lobbyDisplayPackage.ToByteArray().Length;
            return lobbyDisplayPackage;
        }
       
    }

    
}
