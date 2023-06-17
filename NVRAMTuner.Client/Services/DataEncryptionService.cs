namespace NVRAMTuner.Client.Services
{
    using Interfaces;
    using System.Security.Cryptography;

    /// <summary>
    /// Provides the ability to encrypt/decrypt data using the Windows Data Protection API (DPAPI)
    /// as well as combine this functionality with serialisation to persist sensitive data to disk
    /// safely (or at least in safer manner than using plaintext)
    /// </summary>
    public class DataEncryptionService : IDataEncryptionService
    {
        /// <summary>
        /// Random entropy to be used in the encryption process
        /// </summary>
        private readonly byte[] entropy;

        /// <summary>
        /// Initialises a new instance of the <see cref="DataEncryptionService"/> class
        /// </summary>
        public DataEncryptionService()
        {
            this.entropy = new byte[256];
            new RNGCryptoServiceProvider().GetBytes(this.entropy);
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