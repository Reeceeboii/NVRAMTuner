#nullable enable

namespace NVRAMTuner.Client.Services.Interfaces
{
    using Models;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for a service that handles persisting data to a file
    /// </summary>
    public interface IDataPersistenceService
    {
        /// <summary>
        /// Serialises a <see cref="Router"/> object to XML, encrypts the raw bytes via DPAPI
        /// and dumps the contents to a file in the user's local application data folder
        /// </summary>
        /// <param name="router">The <see cref="Router"/> instance that is to be encrypted & persisted</param>
        /// <returns>An asynchronous <see cref="Task"/></returns>
        Task SerialiseRouterToEncryptedFileAsync(Router router);

        /// <summary>
        /// Loads raw encrypted bytes from a file in the user's local application data folder, decrypts them and
        /// and deserialises them into a <see cref="Router"/> instance
        /// </summary>
        /// <returns>A <see cref="Router"/> instance that was previously encrypted and serialised</returns>
        /// <exception cref="FileNotFoundException">If the router file does not exist on disk</exception>
        Router? DeserialiseRouterFromEncryptedFile();
    }
}