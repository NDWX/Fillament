
using System.Collections.Generic;

namespace Fillament
{
    public class Principal<TInfo,  TSecurityOptions> : IGenericPrincipal<TInfo, TSecurityOptions> where TSecurityOptions : SecurityOptions
        where TInfo : PrincipalInfo
    {
        public TInfo Info { get; set; }
        
        public ConnectionLimitOptions Options { get; set; }
        
        public TSecurityOptions Security { get; set; }
        
        public Directory HomeDirectory { get; set; }
        
        public ICollection<VirtualDirectory> VirtualDirectories { get; set; }
        
        public SpeedLimits SpeedLimits { get; set; }
    }
}