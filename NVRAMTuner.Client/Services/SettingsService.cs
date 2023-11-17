namespace NVRAMTuner.Client.Services
{
    using Interfaces;
    using Messages.Settings;
    using Models.Enums;
    using Properties;
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using Wrappers.Interfaces;

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
        /// Instance of <see cref="IMessengerService"/>
        /// </summary>
        private readonly IMessengerService messengerService;

        /// <summary>
        /// Initialises a new instance of the <see cref="SettingsService"/> class
        /// </summary>
        /// <param name="messengerService">An instance of <see cref="IMessengerService"/></param>
        public SettingsService(IMessengerService messengerService)
        {
            this.messengerService = messengerService;
            this.settings = Settings.Default;
            this.settings.PropertyChanged += this.SettingsOnPropertyChanged;
        }

        /// <summary>
        /// An event fired whenever settings are saved to disk
        /// </summary>
        public event EventHandler SettingsChangedEvent;

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
                this.messengerService.Send(new ThemeChangeMessage(value));
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
        /// Gets or sets a value representing whether or not the pre-commit warning message
        /// should be displayed to the user
        /// </summary>
        public bool DisplayPreCommitWarning
        {
            get => this.settings.DisplayPreCommitWarning;
            set
            {
                if (value != this.settings.DisplayPreCommitWarning)
                {
                    this.settings.DisplayPreCommitWarning = value;
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
            this.SettingsChangedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}