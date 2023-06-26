#nullable enable

namespace NVRAMTuner.Client.Services.Interfaces
{
    using Models;
    using System.Collections.Generic;
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
        /// Returns all routers present in the NVRAMTuner local AppData folder that are
        /// able to be successfully deserialised 
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Router"/> instances</returns>
        IEnumerable<Router> DeserialiseAllPresentRouters();

        /// <summary>
        /// Writes plain text to a file
        /// </summary>
        /// <param name="absoluteFilePath">The absolute path to the file that will be written to</param>
        /// <param name="text">The text data to write to the file</param>
        void WriteTextToFile(string absoluteFilePath, string text);
    }
}