using Shared.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server.Test
{
    public class DataPackageProvider
    {
        private Dictionary<string, DataPackage> _DataPackages = new Dictionary<string, DataPackage>
        {
            {
                "",
                DataPackage dataPackage = new DataPackage
                {
                Header = ProtocolActionEnum.Accept,
                Payload = JsonConvert.SerializeObject(new PROT_ACCEPT
                {
                    _SmallUpdate = "You are connected to the Server and in the Lobby "
                })
                },
                "435",
                DataPackage dataPackage = new DataPackage
                {
                Header = ProtocolActionEnum.Accept,
                Payload = JsonConvert.SerializeObject(new PROT_ACCEPT
                {
                    _SmallUpdate = "You are connected to the Server and in the Lobby "
                })
                }
            }
        };
    }
}
