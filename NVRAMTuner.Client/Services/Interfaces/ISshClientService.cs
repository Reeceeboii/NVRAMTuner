namespace NVRAMTuner.Client.Services.Interfaces
{
    using Models;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for a service that exposes top level actions over an <see cref="Renci.SshNet.SshClient"/>
    /// instance
    /// </summary>
    public interface ISshClientService
    {
        /// <summary>
        /// Attempts to open an SSH connection to the remote server using password based authentication.
        /// </summary>
        /// <param name="router">An instance of <see cref="Router"/> containing details required to initiate a connection</param>
        /// <returns>An instance of <see cref="SshConnectionResult"/> that holds the result of the connection attempt,
        /// along with any other relevant information including</returns>
        Task<SshConnectionResult> AttemptRouterSshAuthWithPassword(Router router);

        /// <summary>
        /// Dispose of any relevant resources
        /// </summary>
        void Dispose();
    }
}