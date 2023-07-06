namespace NVRAMTuner.Client.Services
{
    using CommunityToolkit.Mvvm.Messaging;
    using Interfaces;
    using Messages;
    using Models;
    using Models.Enums;
    using Properties;
    using System.ComponentModel;
    using System.Configuration;

    /// <summary>
    /// Service to handle retrieving and storing settings
    /// </summary>
    public class SettingsService : ISettingsService
    {
        /// <summary>
        /// Instance of <see cref="ApplicationSettings"/>
        /// </summary>
        private readonly ApplicationSettings settings;

        /// <summary>
        /// Instance of <see cref="IMessenger"/>
        /// </summary>
        private readonly IMessenger messenger;

        /// <summary>
        /// Initialises a new instance of the <see cref="SettingsService"/> class
        /// </summary>
        public SettingsService(IMessenger messenger)
        {
            this.messenger = messenger;

            this.settings = ApplicationSettings.Default;
            this.settings.PropertyChanged += this.SettingsOnPropertyChanged;
        }

        /// <summary>
        /// Gets or sets the current <see cref="ApplicationTheme"/> stored in settings
        /// </summary>
        public ApplicationTheme ApplicationTheme
        {
            get => this.settings.AppTheme;
            set => this.settings.AppTheme = value;
        }

        /// <summary>
        /// Method handler for the <see cref="ApplicationSettingsBase.PropertyChanged"/> event.
        /// This is subscribed to such that the settings file can be saved whenever a setting value
        /// is altered
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Instance of <see cref="PropertyChangedEventArgs"/></param>
        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.settings.Save();

            this.messenger.Send(new LogMessage(new LogEntry
            {
                LogMessage = "Settings have been saved"
            }));
        }
    }
}