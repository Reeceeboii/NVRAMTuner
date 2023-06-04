namespace NVRAMTuner.Client.Services
{
    using Exceptions;
    using Interfaces;
    using Models;
    using Models.Enums;
    using Renci.SshNet;
    using System;

    /// <summary>
    /// Service class to handle the SSH aspects of NVRAMTuner
    /// </summary>
    public class SshClientService : IDisposable, ISshClientService
    {
        /// <summary>
        /// Instance of <see cref="SshClient"/>
        /// </summary>
        private SshClient client;

        /// <summary>
        /// Attempts to open an SSH connection to the remote server using password based authentication.
        /// </summary>
        /// <param name="ip">The IP address of the remote server</param>
        /// <param name="sshPort">The port on the remote server exposing the target SSH server</param>
        /// <param name="credentials">An instance of <see cref="SshCredentialPair"/> to hold credentials</param>
        /// <returns>An instance of <see cref="SshConnectionResult"/> that holds the result of the connection attempt,
        /// along with any other relevant information including any errors raised during the connection process</returns>
        public SshConnectionResult AuthWithPassword(string ip, int sshPort, SshCredentialPair credentials)
        {
            if (!NetworkService.VerifyIpv4Address(ip))
            {
                return new SshConnectionResult()
                {
                    ConnectionSuccessful = false,
                    AuthType = SshAuthType.PasswordBasedAuth,
                    ConnectionException = new SshConnectionException(message: $"{ip} is not a valid IPv4 address.")
                };
            }

            if (!NetworkService.VerifyNetworkPort(sshPort))
            {
                return new SshConnectionResult()
                {
                    ConnectionSuccessful = false,
                    AuthType = SshAuthType.PasswordBasedAuth,
                    ConnectionException = new SshConnectionException(message: $"{sshPort} is not a valid network port.")
                };
            }

            AuthenticationMethod am = new PasswordAuthenticationMethod(credentials.Username, credentials.Password);
            ConnectionInfo connectionInfo = new ConnectionInfo(ip, sshPort, credentials.Username, am);
            this.client = new SshClient(connectionInfo);

            if (this.client.IsConnected)
            {
                return new SshConnectionResult()
                {
                    ConnectionSuccessful = true, AuthType = SshAuthType.PasswordBasedAuth
                };
            }

            this.client.Disconnect();
            return new SshConnectionResult()
            {
                ConnectionSuccessful = false,
                AuthType = SshAuthType.PasswordBasedAuth,
                ConnectionException = new SshConnectionException(message: "Client failed to connect using provided credentials")
            };
        }

        /// <summary>
        /// Dispose of any relevant resources
        /// </summary>
        public void Dispose()
        {
            this.client.Dispose();
        }
    }
}