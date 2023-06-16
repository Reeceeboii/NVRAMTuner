#nullable enable

namespace NVRAMTuner.Client.Models
{
    using Enums;

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

        /// <summary>
        /// Gets or sets the router's SSH username. Will be null if <see cref="AuthType"/>
        /// is set to <see cref="SshAuthType.PubKeyBasedAuth"/>
        /// </summary>
        public string? SshUsername { get; set; }

        /// <summary>
        /// Gets or sets the router's SSH password. Will be null if <see cref="AuthType"/>
        /// is set to <see cref="SshAuthType.PubKeyBasedAuth"/>
        /// </summary>
        public string? SshPassword { get; set; }

        /// <summary>
        /// Gets or sets the directory on the host system within which
        /// </summary>
        public string? SskKeyDir { get; set; }

        /// <summary>
        /// Gets or sets the authentication method being used against the router
        /// </summary>
        public SshAuthType AuthType { get; set; }
    }
}