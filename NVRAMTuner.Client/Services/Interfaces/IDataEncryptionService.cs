namespace NVRAMTuner.Client.Services.Interfaces
{
    using System.Security.Cryptography;

    /// <summary>
    /// Interface for a service that handles encryption via the Windows Data Protection API (DPAPI)
    /// </summary>
    public interface IDataEncryptionService
    {
        /// <summary>
        /// Encrypts data in the form of a byte array
        /// </summary>
        /// <param name="unencryptedData">The unencrypted data that is to be encrypted</param>
        /// <param name="scope">A member of the <see cref="DataProtectionScope"/> enumeration</param>
        /// <returns><paramref name="unencryptedData"/>, in its encrypted form</returns>
        byte[] EncryptData(byte[] unencryptedData, DataProtectionScope scope = DataProtectionScope.CurrentUser);

        /// <summary>
        /// Decrypts data in the form of a byte array
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        byte[] DecryptData(byte[] encryptedData, DataProtectionScope scope = DataProtectionScope.CurrentUser);
    }
}