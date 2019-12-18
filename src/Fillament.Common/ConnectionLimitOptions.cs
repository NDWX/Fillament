namespace Fillament
{
    public class ConnectionLimitOptions
    {
        public bool OverrideServerLimits { get; set; }
        
        public int MaximumConnections { get; set; }
        
        public int MaximumConnectionsPerIp { get; set; }
    }
}