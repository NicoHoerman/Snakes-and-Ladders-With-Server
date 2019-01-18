using System.Collections.Generic;
using System.Xml.Linq;

namespace TCP_Client.Game.EandEContracts
{

    public interface IConfigurationProvider
    {
        List<XElement> GetEntityConfigurations();
    }
}
