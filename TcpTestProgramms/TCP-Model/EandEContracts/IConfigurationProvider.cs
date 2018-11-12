using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TCP_Model.EandEContracts
{

    public interface IConfigurationProvider
    {
        List<XElement> GetEntityConfigurations();
    }
}