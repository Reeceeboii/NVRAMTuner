namespace NVRAMTuner.Client.Services.Interfaces
{
    using Models;

    /// <summary>
    /// Interface for a service that exposes top level actions over an <see cref="Renci.SshNet.SshClient"/>
    /// instance
    /// </summary>
    public interface ISshClientService
    {
        /// <summary>
        /// Attempts to open an SSH connection to the remote server using password based authentication.
        /// </summary>
        /// <param name="ip">The IP address of the remote server</param>
        /// <param name="sshPort">The port on the remote server exposing the target SSH server</param>
        /// <param name="credentials">An instance of <see cref="SshCredentialPair"/> to hold credentials</param>
        /// <returns>An instance of <see cref="SshConnectionResult"/> that holds the result of the connection attempt,
        /// along with any other relevant information including any errors raised during the connection process</returns>
        SshConnectionResult AuthWithPassword(string ip, int sshPort, SshCredentialPair credentials);

        /// <summary>
        /// Dispose of any relevant resources
        /// </summary>
        void Dispose();
    }
}