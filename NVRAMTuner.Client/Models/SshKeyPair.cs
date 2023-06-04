namespace NVRAMTuner.Client.Models
{
    /// <summary>
    /// Model representing a pair (private and public) SSH keys
    /// </summary>
    public class SshKeyPair
    {
        /// <summary>
        /// Gets or sets a path pointing to the user's public key
        /// </summary>
        public string PubKeyPath { get; set; }

        /// <summary>
        /// Gets or sets a path pointing to the user's private key
        /// </summary>
        public string PrivKeyPath { get; set;}
    }
}