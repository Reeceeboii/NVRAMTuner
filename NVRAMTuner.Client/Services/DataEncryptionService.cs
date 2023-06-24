namespace NVRAMTuner.Client.Services
{
    using Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;

    /// <summary>
    /// Provides the ability to encrypt/decrypt data using the Windows Data Protection API (DPAPI)
    /// as well as combine this functionality with serialisation to persist sensitive data to disk
    /// safely (or at least in safer manner than using plaintext)
    /// </summary>
    public class DataEncryptionService : IDataEncryptionService
    {
        /// <summary>
        /// A static GUID value used to help in the derivation of entropy data
        /// </summary>
        private readonly string entropyGuid;

        /// <summary>
        /// A byte array of entropy to use in encryption
        /// </summary>
        private readonly byte[] entropy;

        /// <summary>
        /// Initialises a new instance of the <see cref="DataEncryptionService"/> class
        /// </summary>
        public DataEncryptionService(IWindowsSecurityService windowsSecurityService)
        {
            this.entropyGuid = "75207C65-2A79-4800-998E-BA7F57C7CC37";
            this.entropy = this.DeriveEntropy(windowsSecurityService.RetrieveUserSid());
        }

        /// <summary>
        /// Derive some entropy from the users <see cref="SecurityIdentifier"/> and a GUID
        /// combined together
        /// </summary>
        /// <returns></returns>
        private byte[] DeriveEntropy(SecurityIdentifier sid)
        {
            List<byte> entropyConstruction = Encoding.UTF8.GetBytes(sid.Value).ToList();
            entropyConstruction.AddRange(Encoding.ASCII.GetBytes(this.entropyGuid));

            // truncate the end of the list so its length is the highest possible multiple of 16
            int highestMultiple = (entropyConstruction.Count / 16) * 16;
            if (entropyConstruction.Count != highestMultiple)
            {
                entropyConstruction.RemoveRange(highestMultiple, entropyConstruction.Count - highestMultiple);
            }
            return entropyConstruction.ToArray();
        }

        /// <summary>
        /// Encrypts data in the form of a byte array
        /// </summary>
        /// <param name="unencryptedData">The unencrypted data that is to be encrypted</param>
        /// <param name="scope">A member of the <see cref="DataProtectionScope"/> enumeration</param>
        /// <returns><paramref name="unencryptedData"/>, in its encrypted form</returns>
        public byte[] EncryptData(byte[] unencryptedData, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            return ProtectedData.Protect(unencryptedData, this.entropy, scope);
        }

        /// <summary>
        /// Decrypts data in the form of a byte array
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public byte[] DecryptData(byte[] encryptedData, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            return ProtectedData.Unprotect(encryptedData, this.entropy, scope);
        }
    }
}