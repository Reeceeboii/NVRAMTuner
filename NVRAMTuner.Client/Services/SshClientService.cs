namespace NVRAMTuner.Client.Services
{
    using Interfaces;
    using Models;
    using Models.Enums;
    using Renci.SshNet;
    using Resources;
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using System.Threading.Tasks;

    /// <summary>
    /// Service class to handle the SSH aspects of NVRAMTuner
    /// </summary>
    public class SshClientService : IDisposable, ISshClientService
    {
        /// <summary>
        /// Instance of <see cref="SshClient"/>
        /// </summary>
        private readonly SshClient client;

        /// <summary>
        /// Instance of <see cref="IFileSystem"/>
        /// </summary>
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Initialises a new instance of the <see cref="SshClientService"/> class
        /// </summary>
        /// <param name="fileSystem">An instance of <see cref="IFileSystem"/></param>
        public SshClientService(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

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
        public async Task<SshConnectionInfo> AttemptConnectionToRouterAsync(Router router)
        {
            ConnectionInfo connectionInfo;

            if (router.AuthType == SshAuthType.PasswordBasedAuth)
            {
                connectionInfo = new PasswordConnectionInfo(
                    router.RouterIpv4Address,
                    router.SshPort,
                    router.SshUsername,
                    router.SshPassword);
            }
            else
            {
                string privateKeyPath = this.fileSystem.Path.Combine(router.SskKeyDir ?? string.Empty, "id_rsa");

                if (!this.fileSystem.File.Exists(privateKeyPath))
                {
                    throw new FileNotFoundException("Private SSH key not found", privateKeyPath);
                }

                using Stream privateKeyStream = this.fileSystem.File.Open(privateKeyPath, FileMode.Open);

                connectionInfo = new PrivateKeyConnectionInfo(
                    router.RouterIpv4Address,
                    router.SshPort,
                    router.SshUsername,
                    new PrivateKeyFile(privateKeyStream));
            }

            using (SshClient tempClient = new SshClient(connectionInfo))
            {
                tempClient.Connect();

                if (tempClient.IsConnected)
                {
                    SshCommand hostName = await Task.Run(() => tempClient.RunCommand(SshCommands.HostName_Command));
                    SshCommand os = await Task.Run(() => tempClient.RunCommand(SshCommands.Uname_Os_Command));

                    tempClient.Disconnect();

                    return new SshConnectionInfo
                    {
                        ConnectionSuccessful = true,
                        HostName = hostName.Result.TrimEnd('\r', '\n'),
                        OperatingSystem = os.Result.TrimEnd('\r', '\n')
                    };
                }

                tempClient.Disconnect();
            }

            return new SshConnectionInfo
            {
                ConnectionSuccessful = false
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