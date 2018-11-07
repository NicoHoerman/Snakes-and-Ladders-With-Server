using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace EelsAndEscalators
{

    public interface IConfigurationProvider
    {
        List<XElement> GetEntityConfigurations();
    }
}