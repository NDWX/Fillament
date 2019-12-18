using IO = System.IO;

namespace Fillament.ConfigurationAccessProvider.Xml
{
    public class XmlConfigurationAccessFactory : IConfigurationAccessFactory
    {
        private readonly string _configurationFile;

        public XmlConfigurationAccessFactory(string configurationFile)
        {
            _configurationFile = configurationFile;
		    
            if( !IO.File.Exists(_configurationFile))
                throw new IO.FileNotFoundException("Unable to find instance configuration file.");
        }
        
        public IConfigurationAccessProvider GetInstance()
        {
            return new XmlConfigurationAccessProvider(_configurationFile);
        }
    }
}