#nullable enable

namespace NVRAMTuner.Client.Models
{
    using Enums;
    using Exceptions;

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
        /// Gets or sets an instance of <see cref="SshConnectionException"/> representing any errors that occurred
        /// during the connection attempt that this result is pertaining to. If the connection was a success, this
        /// property will be null.
        /// </summary>
        public SshConnectionException? ConnectionException { get; set; }
    }
}