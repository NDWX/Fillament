using System.Collections.Generic;
using System.Net;

namespace Fillament
{
    public class SecurityOptions
    {
        public bool SslRequired { get; set; }
        
        public ICollection<IPAddress> IpWhitelist { get; set; }
        
        public ICollection<IPAddress> IpBlacklist { get; set; }
    }
}