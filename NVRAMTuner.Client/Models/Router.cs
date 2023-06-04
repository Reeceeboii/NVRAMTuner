namespace NVRAMTuner.Client.Models
{
    using Services;

    /// <summary>
    /// Model representing a router
    /// </summary>
    public class Router
    {
        /// <summary>
        /// Gets or sets the IPv4 address of the router
        /// </summary>
        public string RouterIpv4Address { get; set; }

        /// <summary>
        /// Gets or sets the network port on the router used to expose its SSH server
        /// </summary>
        public int SshPort { get; set; }
    }
}