namespace NVRAMTuner.Client.Services
{
    using Interfaces;
    using Models;
    using Models.Enums;
    using Renci.SshNet;
    using System;
    using System.Threading.Tasks;

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
        /// <param name="router">An instance of <see cref="Router"/> containing details required to initiate a connection</param>
        /// <returns>An instance of <see cref="SshConnectionResult"/> that holds the result of the connection attempt,
        /// along with any other relevant information including</returns>
        public async Task<SshConnectionResult> AttemptRouterSshAuthWithPassword(Router router)
        {
            ConnectionInfo connectionInfo = new PasswordConnectionInfo(router.RouterIpv4Address, router.SshUsername, router.SshPassword);

            using (SshClient tempClient = new SshClient(connectionInfo))
            {
                await Task.Run(() => tempClient.Connect());

                if (tempClient.IsConnected)
                {
                    tempClient.Disconnect();

                    return new SshConnectionResult
                    {
                        ConnectionSuccessful = true, 
                        AuthType = SshAuthType.PasswordBasedAuth, 
                        router = router
                    };
                }

                tempClient.Disconnect();
            }

            return new SshConnectionResult
            {
                ConnectionSuccessful = false,
                AuthType = SshAuthType.PasswordBasedAuth,
                router = router
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