using System.Collections.Generic;

namespace Fillament
{
    public class SpeedLimit
    {
        public bool OverrideServerLimit { get; set; }
        
        public SpeedLimitType Type { get; set; }
        
        public int? ConstantLimit { get; set; }

        public ICollection<SpeedLimitRule> Rules { get; set; }
    }
}