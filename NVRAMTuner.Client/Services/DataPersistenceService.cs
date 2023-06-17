#nullable enable

namespace NVRAMTuner.Client.Services
{
    using Interfaces;
    using Models;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Abstractions;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// A service class for handling data persistence required by NVRAMTuner
    /// </summary>
    public class DataPersistenceService : IDataPersistenceService
    {
        /// <summary>
        /// Instance of <see cref="IDataEncryptionService"/>
        /// </summary>
        private readonly IDataEncryptionService dataEncryptionService;

        /// <summary>
        /// Instance of <see cref="IEnvironmentService"/>
        /// </summary>
        private readonly IEnvironmentService environmentService;

        /// <summary>
        /// Instance of <see cref="IFileSystem"/>
        /// </summary>
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// A filename that is used to refer to a file within which serialised and encrypted <see cref="Router"/>
        /// instances are stored
        /// </summary>
        private const string RouterBinaryFileName = "Router.bin";

        /// <summary>
        /// An <see cref="XmlSerializer"/> instance used to carry out the serialisation
        /// </summary>
        private readonly XmlSerializer routerSerialiser;

        /// <summary>
        /// Initialises a new instance of the <see cref="DataPersistenceService"/> class
        /// </summary>
        /// <param name="dataEncryptionService">An instance of <see cref="IDataEncryptionService"/></param>
        /// <param name="environmentService">An instance of <see cref="IEnvironmentService"/></param>
        /// <param name="fileSystem">An instance of <see cref="IFileSystem"/></param>
        public DataPersistenceService(
            IDataEncryptionService dataEncryptionService,
            IEnvironmentService environmentService,
            IFileSystem fileSystem)
        {
            this.dataEncryptionService = dataEncryptionService;
            this.environmentService = environmentService;
            this.fileSystem = fileSystem;

            this.routerSerialiser = new XmlSerializer(typeof(Router));
        }

        /// <summary>
        /// Serialises a <see cref="Router"/> object to XML, encrypts the raw bytes via DPAPI
        /// and dumps the contents to a file in the user's local application data folder
        /// </summary>
        /// <param name="router">The <see cref="Router"/> instance that is to be encrypted & persisted</param>
        /// <returns>An asynchronous <see cref="Task"/></returns>
        public async Task SerialiseRouterToEncryptedFileAsync(Router router)
        {
            string appPath = this.GetAppPath();
            string serialiseTarget = this.fileSystem.Path.Combine(appPath, RouterBinaryFileName);

            if (!this.fileSystem.Directory.Exists(appPath))
            {
                this.fileSystem.Directory.CreateDirectory(appPath);
            }

            MemoryStream memStream = new MemoryStream();
            this.routerSerialiser.Serialize(memStream, router);
            
            memStream.Position = 0;
            using StreamReader sr = new StreamReader(memStream);
            string serialisedRouter = await sr.ReadToEndAsync();

            byte[] encryptedRouter = this.dataEncryptionService.EncryptData(Encoding.UTF8.GetBytes(serialisedRouter));
            this.fileSystem.File.WriteAllBytes(serialiseTarget, encryptedRouter);
        }

        /// <summary>
        /// Loads raw encrypted bytes from a file in the user's local application data folder, decrypts them and
        /// and deserialises them into a <see cref="Router"/> instance
        /// </summary>
        /// <returns>A <see cref="Router"/> instance that was previously encrypted and serialised</returns>
        /// <exception cref="FileNotFoundException">If the router file does not exist on disk</exception>
        public Router? DeserialiseRouterFromEncryptedFile()
        {
            string appPath = this.GetAppPath();
            string deserialiseTarget = this.fileSystem.Path.Combine(appPath, RouterBinaryFileName);

            if (!this.fileSystem.File.Exists(deserialiseTarget))
            {
                throw new FileNotFoundException("Router binary file not found", deserialiseTarget);
            }

            byte[] encryptedRouter = this.fileSystem.File.ReadAllBytes(deserialiseTarget);
            byte[] decryptedRouter = this.dataEncryptionService.DecryptData(encryptedRouter);

            string decryptedRouterString = Encoding.UTF8.GetString(decryptedRouter);

            Router router;
            try
            {
                router = (Router)this.routerSerialiser.Deserialize(new StringReader(decryptedRouterString));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType()}: {ex.Message}");
                return null;
            }

            return router;
        }

        /// <summary>
        /// Gets a string containing the absolute path to NVRAMTuner's data folder within the user's
        /// AppData\Local area
        /// </summary>
        /// <returns>A string containing the absolute path to the app's folder</returns>
        private string GetAppPath()
        {
            string localAppData = this.environmentService.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return this.fileSystem.Path.Combine(localAppData, "NVRAMTuner");
        }
    }
}