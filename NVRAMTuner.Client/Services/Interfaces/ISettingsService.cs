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
    }
}