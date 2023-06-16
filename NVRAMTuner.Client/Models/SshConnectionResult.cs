#nullable enable

namespace NVRAMTuner.Client.Models
{
    using Enums;

    /// <summary>
    /// Model representing the result of an attempt to connect with a remote SSH server
    /// </summary>
    public class SshConnectionResult
    {
        /// <summary>
        /// Gets or sets a bool representing, overall, whether the connection was a success or not
        /// </summary>
        public bool ConnectionSuccessful { get; set; }

        /// <summary>
        /// Gets or sets a value from <see cref="SshAuthType"/>, representing what type
        /// of authentication this connection result is pertaining to
        /// </summary>
        public SshAuthType AuthType { get; set; }

        /// <summary>
        /// A <see cref="Router"/> instance containg the details that were used in this specific connection attempt
        /// </summary>
        public Router router { get; set; }
    }
}