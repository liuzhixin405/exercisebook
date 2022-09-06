using System;

namespace Orleans.Common
{
    /// <summary>
    /// silo host configure
    /// </summary>
    public class SiloExOptions
    {
        public string ClusterId { get; set; }
        public string ServiceId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool HostSelf { get; set; }
        public int CounterUpdateIntervalMs { get; set; }
        public int SiloPort { get; set; }
        public int GatewayPort { get; set; }
        public string IpAddress { get; set; }
    }
}
