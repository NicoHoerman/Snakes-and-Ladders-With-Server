using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.XML_Config
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly string _configurationFile;

        private List<XElement> _configurations;

        public ConfigurationProvider(string configurationFile)
        {
            _configurationFile = configurationFile;

            ReadConfigurationFile();
        }

        public ConfigurationProvider()
            : this(@".\EandE\XML_Config\Configurations.xml")
        { }


        private void ReadConfigurationFile()
        {
            var doc = XDocument.Load(_configurationFile);
            _configurations = doc.Root.Elements().ToList();
        }

        public List<XElement> GetEntityConfigurations() => _configurations;
    }
}
