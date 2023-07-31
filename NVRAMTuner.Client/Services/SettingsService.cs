namespace NVRAMTuner.Client.Services
{
    using CommunityToolkit.Mvvm.Messaging;
    using Messages;
    using Messages.Theme;
    using Models.Enums;
    using Properties;
    using Interfaces;
    using System.ComponentModel;
    using System.Configuration;

    /// <summary>
    /// Service to handle retrieving and storing settings
    /// </summary>
    public class SettingsService : ISettingsService
    {
        /// <summary>
        /// Instance of <see cref="Settings"/>
        /// </summary>
        private readonly Settings settings;

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

            this.settings = Settings.Default;
            this.settings.PropertyChanged += this.SettingsOnPropertyChanged;
        }

        /// <summary>
        /// Gets or sets the current <see cref="ApplicationTheme"/> stored in settings
        /// </summary>
        public ApplicationTheme ApplicationTheme
        {
            get => this.settings.AppTheme;
            set
            {
                if (value == this.ApplicationTheme)
                {
                    return;
                }

                this.settings.AppTheme = value;
                this.messenger.Send(new ThemeChangeMessage(value));
            }
        }

        /// <summary>
        /// Gets or sets the interval in minutes used to send keep alive messages to the remote SSH server
        /// </summary>
        public int SshKeepAliveIntervalMinutes
        {
            get => this.settings.SshKeepAliveIntervalMinutes;
            set
            {
                if (value != this.settings.SshKeepAliveIntervalMinutes && value >= 1)
                {
                    this.settings.SshKeepAliveIntervalMinutes = value;
                }
            }
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
            this.messenger.Send(new LogMessage("Settings have been saved"));
        }
    }
}