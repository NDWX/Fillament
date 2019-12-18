using System;

namespace Fillament
{
    public class ConfigurationAccessException : Exception
    {
        public ConfigurationAccessException(string message) : base(message)
        {
			
        }

        public ConfigurationAccessException(string message, Exception innerException) : base(message, innerException)
        {
			
        }
    }
}