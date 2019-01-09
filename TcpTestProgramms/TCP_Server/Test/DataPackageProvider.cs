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
    public class DataPackageProvider
    {
        private DataPackage accpetedInfoPackage;
        private DataPackage declinedInfoPackage;
        private DataPackage lobbyDisplayPackage;
        private DataPackage declineUpdatePackage;

        public DataPackageProvider()
        {
            accpetedInfoPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Accept,
                Payload = JsonConvert.SerializeObject(new PROT_ACCEPT
                {
                    _SmallUpdate = "You are connected to the Server and in the Lobby "
                })
            };
            declinedInfoPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Decline,
                Payload = JsonConvert.SerializeObject(new PROT_DECLINE
                {
                    _SmallUpdate = "You got declind. Lobby is probably full"
                })
            };
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
            declinedInfoPackage = new DataPackage
            {
                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _infoOutput = "A player got declined"
                })
            };

            Dictionary<string, DataPackage> _DataPackages = new Dictionary<string, DataPackage>
            {
                {"AcceptedInfo",  accpetedInfoPackage },
                {"DeclinedInfo",  declinedInfoPackage },
                {"LobbyDisplay",  lobbyDisplayPackage },
                {"DeclineUpdate", declinedInfoPackage },
            };
        }
    }
}
