#nullable enable

namespace NVRAMTuner.Client.Services
{
    using CommunityToolkit.Mvvm.Messaging;
    using Interfaces;
    using Messages;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
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
        /// Instance of <see cref="IMessenger"/>
        /// </summary>
        private readonly IMessenger messenger;

        /// <summary>
        /// File extension to be used by NVRAMTuner for any binary <see cref="Router"/> files it creates.
        /// Short for: (N)VRAM(T)uner(B)inary(R)outer
        /// </summary>
        private const string BinaryRouterFileExtension = ".ntbr";

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
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        public DataPersistenceService(
            IDataEncryptionService dataEncryptionService,
            IEnvironmentService environmentService,
            IFileSystem fileSystem,
            IMessenger messenger)
        {
            this.dataEncryptionService = dataEncryptionService;
            this.environmentService = environmentService;
            this.fileSystem = fileSystem;
            this.messenger = messenger;

            this.routerSerialiser = new XmlSerializer(typeof(Router));
        }

        /// <summary>
        /// Serialises a <see cref="Router"/> object to XML, encrypts the raw bytes via DPAPI
        /// and dumps the contents to a file in the user's local application data folder
        /// </summary>
        /// <param name="router">The <see cref="Router"/> instance that is to be encrypted and persisted</param>
        /// <returns>An asynchronous <see cref="Task"/></returns>
        public async Task SerialiseRouterToEncryptedFileAsync(Router router)
        {
            string appPath = this.GetAppPath();
            string serialiseTarget = this.fileSystem.Path.Combine(
                appPath, 
                $"{router.RouterUid}{BinaryRouterFileExtension}");

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

            this.messenger.Send(new LogMessage($"Router \"{router.RouterNickname}\" has been saved"));
        }

        /// <summary>
        /// Returns all routers present in the NVRAMTuner local AppData folder that are
        /// able to be successfully deserialised 
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Router"/> instances</returns>
        public IEnumerable<Router> DeserialiseAllPresentRouters()
        {
            string appPath = this.GetAppPath();

            if (!this.fileSystem.Directory.Exists(appPath))
            {
                Debug.WriteLine("NVRAMTuner local AppData folder does not exist");
                return new List<Router>();
            }

            string[] encryptedRouterFiles = this.fileSystem.Directory.GetFiles(appPath)
                .Where(f => f.EndsWith(BinaryRouterFileExtension))
                .ToArray();

            List<Router> routers = new List<Router>();

            foreach (string file in encryptedRouterFiles)
            {
                byte[] fileBytes = this.fileSystem.File.ReadAllBytes(file);
                byte[] decryptedBytes = this.dataEncryptionService.DecryptData(fileBytes);
                string routerString = Encoding.UTF8.GetString(decryptedBytes);

                using StringReader sr = new StringReader(routerString);

                Router router;
                try
                {
                    router = (Router)this.routerSerialiser.Deserialize(sr);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.GetType()}: {ex.Message}");
                    break;
                }

                routers.Add(router);
            }

            this.messenger.Send(new LogMessage($"{routers.Count} previously saved routers are valid and have been loaded"));

            return routers;
        }

        /// <summary>
        /// Writes plain text to a file
        /// </summary>
        /// <param name="absoluteFilePath">The absolute path to the file that will be written to</param>
        /// <param name="text">The text data to write to the file</param>
        public void WriteTextToFile(string absoluteFilePath, string text)
        {
            this.fileSystem.File.WriteAllText(absoluteFilePath, text);
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