#nullable enable

namespace NVRAMTuner.Client.Models
{
    /// <summary>
    /// Model representing the result of an attempt to connect with a remote SSH server
    /// </summary>
    public class SshConnectionInfo
    {
        /// <summary>
        /// Gets or sets a bool representing, overall, whether the connection was a success or not
        /// </summary>
        public bool ConnectionSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the hostname of the router to which a successful SSH connection was made.
        /// If <see cref="ConnectionSuccessful"/> is false, this will be null
        /// </summary>
        public string? HostName { get; set; }

        /// <summary>
        /// Gets or sets the operating system of the router to which a successful SSH connection was
        /// made. If <see cref="ConnectionSuccessful"/> is false, this will be null
        /// </summary>
        public string? OperatingSystem { get; set; }
    }
}