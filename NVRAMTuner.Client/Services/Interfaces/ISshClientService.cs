namespace NVRAMTuner.Client.Services.Interfaces
{
    using Models;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for a service that exposes top level actions over an <see cref="Renci.SshNet.SshClient"/>
    /// instance
    /// </summary>
    public interface ISshClientService
    {
        /// <summary>
        /// Attempts to open an SSH connection to the remote server using either password-based authentication, or public/private key
        /// based authentication. This choice is up to the user and their local network configuration
        /// </summary>
        /// <param name="router">An instance of <see cref="Router"/> containing details required to initiate a connection</param>
        /// <returns>
        /// An instance of <see cref="SshConnectionInfo"/> that holds the result of the connection attempt,
        /// along with any other relevant information
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// If the private key located in <see cref="Router.SskKeyDir"/> (in the case of pub key authentication)
        /// does not exist on the user's system
        /// </exception>
        Task<SshConnectionInfo> AttemptConnectionToRouterAsync(Router router);

        /// <summary>
        /// Dispose of any relevant resources
        /// </summary>
        void Dispose();
    }
}