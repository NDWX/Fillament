using System.Xml;

namespace Fillament.ConfigurationAccessProvider.Xml
{
    public static class XmlNodeListExtensions
    {
        public static XmlNode First(this XmlNodeList xmlNodeList)
        {
            if (xmlNodeList.Count > 0)
                return xmlNodeList[0];

            return null;
        }
    }
}