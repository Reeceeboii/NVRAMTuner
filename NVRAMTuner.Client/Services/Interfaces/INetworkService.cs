#nullable enable

namespace NVRAMTuner.Client.Services.Interfaces
{
    using Models;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface for a network service
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Scans the local system for a pair of SSH keys on behalf of the user
        /// </summary>
        /// <returns>An absolute path to a directory containing a pair of SSH keys,
        /// or null if none were found.</returns>
        string? ScanLocalSystemForSshKeys();

        /// <summary>
        /// Checks whether a given folder contains a private and public SSH key pair
        /// </summary>
        /// <param name="folder">The absolute path of the folder to test</param>
        /// <returns>True if the folder contains an SSH key pair, false if not</returns>
        bool FolderContainsSshKeys(string folder);

        /// <summary>
        /// Attempts to a connect to a router and returns, asynchronously, a <see cref="SshConnectionInfo"/> instance
        /// </summary>
        /// <param name="router">A <see cref="Router"/> instance, containing the router details that are to be used for
        /// the connection test</param>
        /// <returns>A <see cref="Task{TResult}"/> wrapping a <see cref="SshConnectionInfo"/></returns>
        Task<SshConnectionInfo> AttemptConnectionToRouterAsync(Router router);
    }
}