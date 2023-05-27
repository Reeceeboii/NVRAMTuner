namespace NVRAMTuner.Client.Services
{
    using Exceptions;
    using Models;
    using Models.Enums;
    using Renci.SshNet;
    using System;

    /// <summary>
    /// Service class to handle the SSH aspects of NVRAMTuner
    /// </summary>
    public class SshClientService : IDisposable
    {
        /// <summary>
        /// Instance of <see cref="SshClient"/>
        /// </summary>
        private SshClient client;

        
        public SshConnectionResult AuthWithPassword(string ip, int sshPort, SshPasswordCredentialPair credentials)
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

            ConnectionInfo connectionInfo = new ConnectionInfo()
            this.client = new SshClient()
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