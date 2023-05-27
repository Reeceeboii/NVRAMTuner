namespace NVRAMTuner.Client.Models
{
    /// <summary>
    /// Model representing a username and password combination that a
    /// user wishes to use when authenticating with the remote server
    /// </summary>
    public class SshPasswordCredentialPair
    {
        /// <summary>
        /// Gets or sets the user's username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's password
        /// </summary>
        public string Password { get; set; }
    }
}