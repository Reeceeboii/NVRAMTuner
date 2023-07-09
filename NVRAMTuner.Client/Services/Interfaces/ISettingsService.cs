namespace NVRAMTuner.Client.Services.Interfaces
{
    using Models.Enums;

    /// <summary>
    /// Interface for a service that handles storing and retrieving application settings
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets or sets the current <see cref="Models.Enums.ApplicationTheme"/> stored in settings
        /// </summary>
        ApplicationTheme ApplicationTheme { get; set; }

        /// <summary>
        /// Gets or sets the interval in minutes used to send keep alive messages to the remote SSH server
        /// </summary>
        int SshKeepAliveIntervalMinutes { get; set; }
    }
}