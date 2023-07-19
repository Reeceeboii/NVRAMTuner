#nullable enable

namespace NVRAMTuner.Client.Services
{
    using CommunityToolkit.Mvvm.Messaging;
    using Interfaces;
    using Messages;
    using Models;
    using Models.Enums;
    using Renci.SshNet;
    using Renci.SshNet.Common;
    using Resources;
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of a service for handling various network related operations
    /// pertinent to the operation of the NVRAMTuner application
    /// </summary>
    public class NetworkService : IDisposable, INetworkService
    {
        /// <summary>
        /// An instance of <see cref="IFileSystem"/>
        /// </summary>
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// An instance of <see cref="IEnvironmentService"/>
        /// </summary>
        private readonly IEnvironmentService environmentService;

        /// <summary>
        /// An instance of <see cref="IMessenger"/>
        /// </summary>
        private readonly IMessenger messenger;

        /// <summary>
        /// An instance of <see cref="ISettingsService"/>
        /// </summary>
        private readonly ISettingsService settingsService;

        /// <summary>
        /// Instance of <see cref="SshClient"/>
        /// </summary>
        private SshClient? client;

        /// <summary>
        /// The default SSH port to assume a router is using
        /// </summary>
        public static string DefaultSshPort = "22";

        /// <summary>
        /// Initialises a new instance of the <see cref="NetworkService"/> class
        /// </summary>
        /// <param name="fileSystem">An instance of <see cref="IFileSystem"/></param>
        /// <param name="environmentService">An instance of <see cref="IEnvironmentService"/></param>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        /// <param name="settingsService">An instance of <see cref="ISettingsService"/></param>
        public NetworkService(
            IFileSystem fileSystem,
            IEnvironmentService environmentService,
            IMessenger messenger,
            ISettingsService settingsService)
        {
            this.fileSystem = fileSystem;
            this.environmentService = environmentService;
            this.messenger = messenger;
            this.settingsService = settingsService;
        }

        /// <summary>
        /// Verifies that a given string can be assumed to be a correctly formatted IPv4 address.
        /// This function does not test that a given address exist and/or is reachable over the local
        /// network, however, it can discern if something silly like "1250.34.2.4" or "  1.1."
        /// (or empty/whitespace strings) was/were entered.
        /// </summary>
        /// <param name="address">The IPv4 address to verify</param>
        /// <returns>A bool representing whether or not the <paramref name="address"/> provided
        /// is a validly formatted IPv4 address or not</returns>
        public static bool VerifyIpv4Address(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return false;
            }

            string[] octets = address.Split('.');
            return octets.Length == 4 && octets.All(octet => byte.TryParse(octet, out _));
        }

        /// <summary>
        /// Verifies that a given network port is of a correct format.
        /// </summary>
        /// <param name="port">A port number, given as an int</param>
        /// <returns>A bool representing whether or not <paramref name="port"/>
        /// is a valid port number</returns>
        public static bool VerifyNetworkPort(int port)
        {
            return port >= 1 && port <= 65535;
        }

        /// <summary>
        /// Verifies that a given network port is of a correct format.
        /// This overload accepts a string, and uses the <see cref="VerifyNetworkPort(int)"/>
        /// signature version of this function to further verify the range of the port once
        /// it has been ascertained that the string correctly converts to an int in the first
        /// place.
        /// </summary>
        /// <param name="port">A port number, in string format</param>
        /// <returns>A bool representing whether or not <paramref name="port"/> is a valid
        /// network port</returns>
        public static bool VerifyNetworkPort(string port)
        {
            if (string.IsNullOrWhiteSpace(port))
            {
                return false;
            }

            return int.TryParse(port, out int intPort) && VerifyNetworkPort(intPort);
        }

        /// <summary>
        /// Gets a bool representing whether or not the SSH client is currently connected to a router
        /// </summary>
        public bool IsConnected => this.client?.IsConnected ?? false;

        /// <summary>
        /// Event to be raised when an error occurs in the connection to the router via the <see cref="SshClient"/>
        /// </summary>
        public event EventHandler? SshClientOnErrorOccurred;

        /// <summary>
        /// Attempts to open an SSH connection to the remote server using either password-based authentication, or public/private key
        /// based authentication. This choice is up to the user and their local network configuration.
        /// If <paramref name="useTempClient"/> is set to false (its non-default value), the service's field instance of
        /// the <see cref="SshClient"/> is used to initiate a "real" connection as opposed to an ephemeral test connection.
        /// </summary>
        /// <param name="router">An instance of <see cref="Router"/> containing details required to initiate a connection</param>
        /// <param name="useTempClient">Set to false if you want to initiate a more permanent session instead
        /// of using a temporary client instance for testing</param>
        /// <returns>
        /// An instance of <see cref="SshConnectionInfo"/> that holds the result of the connection attempt,
        /// along with any other relevant information, wrapped in an asynchronous <see cref="Task{TResult}"/>
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// If the private key located in <see cref="Router.SskKeyDir"/> (in the case of pub key authentication)
        /// does not exist on the user's system
        /// </exception>
        /// <exception cref="SshConnectionException">
        /// If one opts to not use the temp client but the real client is already connected to a server
        /// </exception>
        public async Task<SshConnectionInfo> ConnectToRouterAsync(Router router, bool useTempClient = true)
        {
            if (!useTempClient && this.client is { IsConnected: true })
            {
                throw new SshConnectionException("Client instance is already connected!");
            }

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
                    this.messenger.Send(new LogMessage("Private SSH key not found"));
                    throw new FileNotFoundException("Private SSH key not found", privateKeyPath);
                }

                using Stream privateKeyStream = this.fileSystem.File.Open(privateKeyPath, FileMode.Open);

                connectionInfo = new PrivateKeyConnectionInfo(
                    router.RouterIpv4Address,
                    router.SshPort,
                    router.SshUsername,
                    new PrivateKeyFile(privateKeyStream));
            }

            if (useTempClient)
            {
                using (SshClient tempClient = new SshClient(connectionInfo))
                {
                    tempClient.Connect();

                    if (tempClient.IsConnected)
                    {
                        Tuple<string, string> tempHnAndOs = await this.GetRouterHostnameAndOs(tempClient);

                        this.messenger.Send(new LogMessage($"Connected successfully to {tempHnAndOs.Item1} using temporary client. Disconnecting..."));

                        tempClient.Disconnect();

                        return new SshConnectionInfo
                        {
                            ConnectionSuccessful = true,
                            HostName = tempHnAndOs.Item1,
                            OperatingSystem = tempHnAndOs.Item2
                        };
                    }

                    tempClient.Disconnect();
                }

                this.messenger.Send(new LogMessage("Failed to connect using temporary client"));

                return new SshConnectionInfo
                {
                    ConnectionSuccessful = false
                };
            }

            this.client = new SshClient(connectionInfo)
            {
                KeepAliveInterval = TimeSpan.FromMinutes(this.settingsService.SshKeepAliveIntervalMinutes)
            };

            try
            {
                this.client.Connect();
            }
            catch(Exception ex)
            {
                this.messenger.Send(new LogMessage($"Error during connection to '{router.RouterNickname}': \"{ex.Message}\""));

                return new SshConnectionInfo
                {
                    ConnectionSuccessful = false
                };
            }

            Tuple<string, string> hnAndOs = await this.GetRouterHostnameAndOs();
            this.client.ErrorOccurred += this.ClientOnErrorOccurred;

            this.messenger.Send(new LogMessage($"Connected to {hnAndOs.Item1}"));

            return new SshConnectionInfo
            {
                ConnectionSuccessful = true, 
                HostName = hnAndOs.Item1, 
                OperatingSystem = hnAndOs.Item2
            };
        }

        /// <summary>
        /// Handler for the <see cref="BaseClient.ErrorOccurred"/> event
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The <see cref="ExceptionEventArgs"/> instance</param>
        private void ClientOnErrorOccurred(object sender, ExceptionEventArgs e)
        {
            this.SshClientOnErrorOccurred?.Invoke(this, e);
        }

        /// <summary>
        /// Disconnects from the currently connected router, if a connection is currently established
        /// </summary>
        /// <returns>An asynchronous <see cref="Task"/></returns>
        public async Task DisconnectFromRouter()
        {
            if (!(this.client is { IsConnected: true }))
            {
                return;
            }

            await Task.Run(() =>
            {
                this.client.ErrorOccurred -= this.ClientOnErrorOccurred;
                this.client.Disconnect();
                this.messenger.Send(new LogMessage("Disconnected"));
            });
        }

        /// <summary>
        /// Runs a command against the router using the service's <see cref="SshClient"/> instance
        /// </summary>
        /// <param name="command">The command to run</param>
        /// <param name="clientOverride">An optional <see cref="SshClient"/> parameter that can be used to override
        /// the default behaviour of using the service's instance <see cref="SshClient"/></param>
        /// <returns>A <see cref="SshCommand"/> wrapped in an asynchronous <see cref="Task{TResult}"/></returns>
        /// <exception cref="SshConnectionException">If the client is not yet connected to a server</exception>
        /// <exception cref="InvalidOperationException">If the client is not yet initialised</exception>
        public async Task<SshCommand> RunCommandAgainstRouterAsync(string command, SshClient? clientOverride = null)
        {
            if (this.client is { IsConnected: false } && clientOverride != null)
            {
                // user wants to use instance's client but it is not connected
                throw new SshConnectionException("Client is not connected to a server, cannot run command");
            }

            if (clientOverride != null)
            {
                return await Task.Run(() => clientOverride?.RunCommand(command)) 
                       ?? throw new InvalidOperationException("Client is uninitialised");
            }

            return await Task.Run(() => this.client?.RunCommand(command))
                   ?? throw new InvalidOperationException("Overridden client is uninitialised");
        }

        /// <summary>
        /// Gets the hostname and operating system of the router
        /// </summary>
        /// <param name="clientOverride">An optional <see cref="SshClient"/> parameter that can be used to override
        /// the default behaviour of using the service's instance <see cref="SshClient"/></param>
        /// <returns>A <see cref="Tuple{T1, T2}"/> that contains the hostname and operating system of the router</returns>
        private async Task<Tuple<string, string>> GetRouterHostnameAndOs(SshClient? clientOverride = null)
        {
            SshCommand hostName = await this.RunCommandAgainstRouterAsync(ServiceResources.HostName_Command, clientOverride);
            SshCommand os = await this.RunCommandAgainstRouterAsync(ServiceResources.Uname_Os_Command, clientOverride);

            return new Tuple<string, string>(
                hostName.Result.TrimEnd('\r', '\n'), 
                os.Result.TrimEnd('\r', '\n'));
        }

        /// <summary>
        /// Scans the local system for a pair of SSH keys on behalf of the user
        /// </summary>
        /// <returns>An absolute path to a directory containing a pair of SSH keys,
        /// or null if none were found.</returns>
        public string? ScanLocalSystemForSshKeys()
        {
            string homeDir = this.environmentService.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string sshDir = this.fileSystem.Path.Combine(homeDir, ".ssh");
            return this.FolderContainsSshKeys(sshDir) ? sshDir : null;
        }

        /// <summary>
        /// Checks whether a given folder contains a private and public SSH key pair
        /// </summary>
        /// <param name="folder">The absolute path of the folder to test</param>
        /// <returns>True if the folder contains an SSH key pair, false if not</returns>
        public bool FolderContainsSshKeys(string folder)
        {
            // TODO: DSA/EDCSA etc...?
            string pubKeyPath = this.fileSystem.Path.Combine(folder, "id_rsa.pub");
            string privateKeyPath = this.fileSystem.Path.Combine(folder, "id_rsa");
            bool pubKeyExists = this.fileSystem.File.Exists(pubKeyPath);
            bool privateKeyExists = this.fileSystem.File.Exists(privateKeyPath);

            return pubKeyExists && privateKeyExists;
        }

        /// <summary>
        /// Disposes of any relevant resources
        /// </summary>
        public void Dispose()
        {
            if (this.client is { IsConnected: true })
            {
                this.client.Disconnect();
            }

            this.client?.Dispose();
        }
    }
} 