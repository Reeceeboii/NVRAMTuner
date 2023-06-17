#nullable enable

namespace NVRAMTuner.Client.Services
{
    using CommunityToolkit.Mvvm.Messaging;
    using Interfaces;
    using Models;
    using System;
    using System.IO.Abstractions;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of a service for handling various network related operations
    /// pertinent to the operation of the NVRAMTuner application
    /// </summary>
    public class NetworkService : INetworkService
    {
        /// <summary>
        /// An instance of <see cref="ISshClientService"/>
        /// </summary>
        private readonly ISshClientService sshClientService;

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
        /// The default SSH port to assume a router is using
        /// </summary>
        public static string DefaultSshPort = "22";

        /// <summary>
        /// Initialises a new instance of the <see cref="NetworkService"/> class
        /// </summary>
        /// <param name="sshClientService">An instance of <see cref="ISshClientService"/></param>
        /// <param name="fileSystem">An instance of <see cref="IFileSystem"/></param>
        /// <param name="environmentService">An instance of <see cref="IEnvironmentService"/></param>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        public NetworkService(
            ISshClientService sshClientService,
            IFileSystem fileSystem,
            IEnvironmentService environmentService,
            IMessenger messenger)
        {
            this.sshClientService = sshClientService;
            this.fileSystem = fileSystem;
            this.environmentService = environmentService;
            this.messenger = messenger;
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
            string privKeyPath = this.fileSystem.Path.Combine(folder, "id_rsa");
            bool pubKeyExists = this.fileSystem.File.Exists(pubKeyPath);
            bool privKeyExists = this.fileSystem.File.Exists(privKeyPath);

            return pubKeyExists && privKeyExists;
        }

        /// <summary>
        /// Attempts a connection with the user's router based on the information they have provided about it
        /// </summary>
        /// <param name="router">A <see cref="Router"/> instance containing the relevant information
        /// required to initiate an SSH connection and verify the resulting connection's status</param>
        /// <returns>A <see cref="SshConnectionInfo"/> instance</returns>
        public async Task<SshConnectionInfo> AttemptConnectionToRouterAsync(Router router)
        {
            return await this.sshClientService.AttemptConnectionToRouterAsync(router);
        }
    }
} 