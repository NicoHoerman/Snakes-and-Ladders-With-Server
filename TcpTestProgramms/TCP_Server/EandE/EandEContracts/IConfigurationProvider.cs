using System.Collections.Generic;
using System.Xml.Linq;

namespace EandE_ServerModel.EandE.EandEContracts
{

    public interface IConfigurationProvider
    {
        List<XElement> GetEntityConfigurations();
    }
}