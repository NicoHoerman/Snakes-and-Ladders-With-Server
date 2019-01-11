using Newtonsoft.Json;
using Shared.Communications;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCP_Client.PROTOCOLS;

namespace TCP_Client
{
    public class ClientDataPackageProvider
    {
        Dictionary<string, DataPackage> _DataPackages;               
        
        public ClientDataPackageProvider()
        {                               
            var validationAnswerPackage = new DataPackage
            {
                Header = ProtocolActionEnum.ValidationAnswer,
                Payload = JsonConvert.SerializeObject(new PROT_CONNECTION{})
            };
            validationAnswerPackage.Size = validationAnswerPackage.ToByteArray().Length;

            var helpPackage = new DataPackage
            {

                Header = ProtocolActionEnum.GetHelp, //"Client_wants_to_get_help",
                Payload = JsonConvert.SerializeObject(new PROT_HELP
                {

                })

            };
            helpPackage.Size = helpPackage.ToByteArray().Length;

            var rollDicePackage = new DataPackage
            {
                Header = ProtocolActionEnum.RollDice, //"Client_wants_to_rolldice",
                Payload = JsonConvert.SerializeObject(new PROT_ROLLDICE
                {

                })
            };
            rollDicePackage.Size = rollDicePackage.ToByteArray().Length;

            var closeGamePackage = new DataPackage
            {
                Header = ProtocolActionEnum.CloseGame,
                Payload = JsonConvert.SerializeObject(new PROT_CLOSEGAME
                {

                })
            };
            closeGamePackage.Size = closeGamePackage.ToByteArray().Length;

            var startGamePackage = new DataPackage
            {
                Header = ProtocolActionEnum.StartGame,
                Payload = JsonConvert.SerializeObject(new PROT_STARTGAME
                {

                })
            };
            startGamePackage.Size = startGamePackage.ToByteArray().Length;

            var classicPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Rule,
                Payload = JsonConvert.SerializeObject(new PROT_STARTGAME
                {

                })
            };
            classicPackage.Size = classicPackage.ToByteArray().Length;

            _DataPackages = new Dictionary<string, DataPackage>
            {
                {"ValidationAnswer", validationAnswerPackage },
                {"Help", helpPackage },
                {"RollDice", rollDicePackage },
                {"CloseGame", closeGamePackage },
                {"StartGame", startGamePackage },
                {"Classic", classicPackage }
            };

        }
        public DataPackage GetPackage(string key) => _DataPackages[key];       
    }

}

    

