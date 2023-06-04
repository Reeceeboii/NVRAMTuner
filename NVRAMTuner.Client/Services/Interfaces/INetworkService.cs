namespace NVRAMTuner.Client.Services.Interfaces
{
    /// <summary>
    /// An interface for a network service
    /// </summary>
    public interface INetworkService
    {
        void LocateLocalSshKeys();

        /// <summary>
        /// Checks whether a given folder contains a private and public SSH key pair
        /// </summary>
        /// <param name="folder">The absolute path of the folder to test</param>
        /// <returns>True if the folder contains an SSH key pair, false if not</returns>
        bool FolderContainsSshKeys(string folder);
    }
}