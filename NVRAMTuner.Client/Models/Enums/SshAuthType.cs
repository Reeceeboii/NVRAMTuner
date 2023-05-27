namespace NVRAMTuner.Client.Models.Enums
{
    /// <summary>
    /// Enum representing the currently supported methods of SSH authentication
    /// </summary>
    public enum SshAuthType
    {
        /// <summary>
        /// User wants to authenticate to the remote server with password authentication
        /// </summary>
        PasswordBasedAuth,

        /// <summary>
        /// User wants to authenticate to the remote server with a key
        /// </summary>
        PubKeyBasedAuth
    }
}