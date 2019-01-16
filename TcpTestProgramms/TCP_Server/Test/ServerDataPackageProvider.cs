using EandE_ServerModel.EandE.GameAndLogic;
using Newtonsoft.Json;
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
        Dictionary<string, DataPackage> _dataPackages;
        private ServerInfo _serverInfo;
        private Game _game;

        public ServerDataPackageProvider(ServerInfo serverInfo, Game game)
        {
            _serverInfo = serverInfo;
            _game = game;

            #region StaticDataPackages

            var accpetedInfoPackage = new DataPackage
            {
                Header = ProtocolActionEnum.AcceptInfo,
                Payload = JsonConvert.SerializeObject(new PROT_ACCEPT
                {
                    _smallUpdate = "You are connected to the Server and in the Lobby "
                })
            };
            accpetedInfoPackage.Size = accpetedInfoPackage.ToByteArray().Length;

            var declinedInfoPackage = new DataPackage
            {
                Header = ProtocolActionEnum.DeclineInfo,
                Payload = JsonConvert.SerializeObject(new PROT_DECLINE
                {
                    _smallUpdate = "You got declind. Lobby is probably full"
                })
            };
            declinedInfoPackage.Size = declinedInfoPackage.ToByteArray().Length;

            var declineUpdatePackage = new DataPackage
            {
                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _infoOutput = "A player got declined"
                })
            };
            declineUpdatePackage.Size = declineUpdatePackage.ToByteArray().Length;

            var validationRequestPackage = new DataPackage
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

            var playerDataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _infoOutput = "Master is starting the Game"
                })
            };
            playerDataPackage.Size = playerDataPackage.ToByteArray().Length;

            var notEnoughDP = new DataPackage
            {
                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _infoOutput = "Not enough Players to start the game "
                })
            };
            notEnoughDP.Size = notEnoughDP.ToByteArray().Length;

            var onlyMasterStartDP = new DataPackage
            {

                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _infoOutput = "Only the Master can start the Game"
                })
            };
            onlyMasterStartDP.Size = onlyMasterStartDP.ToByteArray().Length;

            var onlyMasterRuleDP = new DataPackage
            {
                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _infoOutput = "Only the Master can set the rules"
                })
            };
            onlyMasterRuleDP.Size = onlyMasterRuleDP.ToByteArray().Length;

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

            #endregion

            _dataPackages = new Dictionary<string, DataPackage>
            {
                {"AcceptedInfo" ,  accpetedInfoPackage },
                {"DeclinedInfo" ,  declinedInfoPackage },
                {"DeclineUpdate", declineUpdatePackage },
                {"ValidationRequest", validationRequestPackage },
                {"ValidationAccepted", validationAcceptedPackage },
                {"PlayerData",playerDataPackage },
                {"NotEnoughInfo",notEnoughDP },
                {"OnlyMasterStartInfo",onlyMasterStartDP},
                {"OnlyMasterRuleInfo",onlyMasterRuleDP},
                {"LobbyCheckFailed", lobbyCheckFailedPackage },
                {"LobbyCheckSuccessful", lobbyCheckSuccessfulPackage },
                {"ServerStartingGame", serverStartingGamePackage }
            };
        }

        public DataPackage GetPackage(string key) => _dataPackages[key];

        #region DataPackages

        public DataPackage LobbyDisplay()
        {
            var servername = _serverInfo._lobbylist[0]._LobbyName;
            var currentplayer = _serverInfo._lobbylist[0]._CurrentPlayerCount;
            var maxplayer = _serverInfo._lobbylist[0]._MaxPlayerCount;

            var lobbyDisplayPackage = new DataPackage
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

        public DataPackage TurnInfo()
        {
			var turninfoPackage = new DataPackage
			{
				Header = ProtocolActionEnum.UpdateView,
				Payload = JsonConvert.SerializeObject(new PROT_UPDATE
				{
					_diceResult = _game.Rules.DiceResult,
					_lastPlayer = _game.State.LastPlayer
                })
            };
            turninfoPackage.Size = turninfoPackage.ToByteArray().Length;
            return turninfoPackage;
        }
            #endregion
    }
}
