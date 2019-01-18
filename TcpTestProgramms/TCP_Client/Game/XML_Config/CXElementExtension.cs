using System;
using System.Xml.Linq;

namespace TCP_Client.Game.XML_Config
{
    public static class XElementExtension
    {
        public static T Get<T>(this XElement element, string key, Func<string, T> converter)
        {
            var valueString = string.Empty;
            try
            {
                valueString = element.Element(key).Value;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Configuration file key {key} not found in element {element}", e);
            }

            try
            {
                return converter(valueString);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Type mismatch in element {element} in node {key}.", e);
            }
        }
    }
}
