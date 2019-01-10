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
        private DataPackage validationAnswerPackage;

        private string servername = string.Empty;
        private int currentplayer;
        private int maxplayer;

        public ClientDataPackageProvider()
        {                               
            validationAnswerPackage = new DataPackage
            {
                Header = ProtocolActionEnum.ValidationRequest,
                Payload = JsonConvert.SerializeObject(new PROT_CONNECTION{})
            };
            validationAnswerPackage.Size = validationAnswerPackage.ToByteArray().Length;

            var _DataPackages = new Dictionary<string, DataPackage>
            {
                {"ValidationAnswer", validationAnswerPackage }
            };

        }
        public DataPackage GetPackage(string key) => _DataPackages[key];       
    }

}

    

