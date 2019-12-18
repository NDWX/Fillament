using System;
using Pug;

namespace Fillament
{
    public class SpeedLimitRule
    {
        public DateTime Date { get; set; }
        
        public DaysOfWeek? DaysOfWeek { get; set; }
        
        public Range<TimeSpan> TimePeriod { get; set; }
        
        public int SpeedLimit { get; set; }
    }
}