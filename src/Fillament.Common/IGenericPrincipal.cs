using System.Collections.Generic;

namespace Fillament
{
    public interface IGenericPrincipal<out TInfo, out TSecurityOptions> where TInfo : PrincipalInfo where TSecurityOptions : SecurityOptions
    {
        TInfo Info { get;  }
        
        ConnectionLimitOptions Options { get; }
        
        TSecurityOptions Security { get; }
        
        Directory HomeDirectory { get; }
        
        ICollection<VirtualDirectory> VirtualDirectories { get; }
        
        SpeedLimits SpeedLimits { get; }
    }
}