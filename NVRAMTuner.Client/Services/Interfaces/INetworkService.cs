namespace NVRAMTuner.Client.Services.Interfaces
{
    using Models;
    using Renci.SshNet;
    using Renci.SshNet.Common;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface for a service that handles network operations pertinent to NVRAMTuner
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Gets a bool representing whether or not the SSH client is currently connected to a router
        /// </summary>
        bool IsConnected { get; }

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
        Task<SshConnectionInfo> ConnectToRouterAsync(Router router, bool useTempClient = true);

        /// <summary>
        /// Runs a command against the router using the service's <see cref="SshClient"/> instance
        /// </summary>
        /// <param name="command">The command to run</param>
        /// <param name="clientOverride">An optional <see cref="SshClient"/> parameter that can be used to override
        /// the default behaviour of using the service's instance <see cref="SshClient"/></param>
        /// <returns>A <see cref="SshCommand"/> wrapped in an asynchronous <see cref="Task{TResult}"/></returns>
        /// <exception cref="SshConnectionException">If the client is not yet connected to a server</exception>
        /// <exception cref="InvalidOperationException">If the client is not yet initialised</exception>
        Task<SshCommand> RunCommandAgainstRouter(string command, SshClient clientOverride = null);

        /// <summary>
        /// Scans the local system for a pair of SSH keys on behalf of the user
        /// </summary>
        /// <returns>An absolute path to a directory containing a pair of SSH keys,
        /// or null if none were found.</returns>
        string ScanLocalSystemForSshKeys();

        /// <summary>
        /// Checks whether a given folder contains a private and public SSH key pair
        /// </summary>
        /// <param name="folder">The absolute path of the folder to test</param>
        /// <returns>True if the folder contains an SSH key pair, false if not</returns>
        bool FolderContainsSshKeys(string folder);

        /// <summary>
        /// Disposes of any relevant resources
        /// </summary>
        void Dispose();
    }
}