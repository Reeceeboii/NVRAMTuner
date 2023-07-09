namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using CommunityToolkit.Mvvm.Messaging;
    using Messages;
    using Models.Enums;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Linq;

    /// <summary>
    /// ViewModel for the SettingsFlyout view
    /// </summary>
    public class SettingsFlyoutViewModel : ObservableRecipient, IRecipient<OpenSettingsFlyoutMessage>
    {
        /// <summary>
        /// An instance of <see cref="ISettingsService"/>
        /// </summary>
        private readonly ISettingsService settingsService;

        /// <summary>
        /// An instance of <see cref="IProcessService"/>
        /// </summary>
        private readonly IProcessService processService;

        /// <summary>
        /// An instance of <see cref="IEnvironmentService"/>
        /// </summary>
        private readonly IEnvironmentService environmentService;

        /// <summary>
        /// An instance of <see cref="IFileSystem"/>
        /// </summary>
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Backing field for <see cref="IsOpen"/>
        /// </summary>
        private bool isOpen;

        /// <summary>
        /// Backing field for <see cref="AvailableApplicationThemes"/>
        /// </summary>
        private List<ApplicationTheme> availableApplicationThemes;

        /// <summary>
        /// Initialises a new instance of the <see cref="SettingsFlyoutViewModel"/> class
        /// </summary>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        /// <param name="settingsService">An instance of <see cref="ISettingsService"/></param>
        /// <param name="processService">An instance of <see cref="IProcessService"/></param>
        /// <param name="environmentService">An instance of <see cref="IEnvironmentService"/></param>
        /// <param name="fileSystem">An instance of <see cref="IFileSystem"/></param>
        public SettingsFlyoutViewModel(
            IMessenger messenger, 
            ISettingsService settingsService,
            IProcessService processService,
            IEnvironmentService environmentService,
            IFileSystem fileSystem) : base(messenger)
        {
            this.settingsService = settingsService;
            this.processService = processService;
            this.environmentService = environmentService;
            this.fileSystem = fileSystem;

            this.IsActive = true;

            this.OpenSettingsFolderCommand = new RelayCommand(this.OpenSettingsFolderCommandHandler);

            this.AvailableApplicationThemes = Enum.GetValues(typeof(ApplicationTheme)).Cast<ApplicationTheme>().ToList();
            this.ReadAndSynchroniseCurrentSettings();
        }

        /// <summary>
        /// Gets the command used to open the local settings directory inside File Explorer
        /// </summary>
        public IRelayCommand OpenSettingsFolderCommand { get; }

        /// <summary>
        /// Gets or sets a bool representing whether or not the flyout is open
        /// </summary>
        public bool IsOpen
        {
            get => this.isOpen;
            set => this.SetProperty(ref this.isOpen, value);
        }

        /// <summary>
        /// Gets or sets a list of available application themes to choose from
        /// </summary>
        public List<ApplicationTheme> AvailableApplicationThemes
        {
            get => this.availableApplicationThemes;
            set => this.SetProperty(ref this.availableApplicationThemes, value);
        }

        /// <summary>
        /// Gets or sets the current application theme
        /// </summary>
        public ApplicationTheme ApplicationTheme
        {
            get => this.settingsService.ApplicationTheme;
            set
            {
                this.settingsService.ApplicationTheme = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current interval used for SSH keep alive messages
        /// </summary>
        public int SshKeepAliveIntervalMinutes
        {
            get => this.settingsService.SshKeepAliveIntervalMinutes;
            set
            {
                this.settingsService.SshKeepAliveIntervalMinutes = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Recipient method for <see cref="OpenSettingsFlyoutMessage"/> messages
        /// </summary>
        /// <param name="message">An instance of <see cref="OpenSettingsFlyoutMessage"/>"/></param>
        public void Receive(OpenSettingsFlyoutMessage message)
        {
            this.IsOpen = true;
        }

        /// <summary>
        /// Reads in all settings from <see cref="ISettingsService"/> and synchronises the ViewModel
        /// </summary>
        private void ReadAndSynchroniseCurrentSettings()
        {
            this.ApplicationTheme = this.settingsService.ApplicationTheme;
            this.SshKeepAliveIntervalMinutes = this.settingsService.SshKeepAliveIntervalMinutes;
        }

        /// <summary>
        /// Method to handle <see cref="OpenSettingsFolderCommand"/>
        /// </summary>
        private void OpenSettingsFolderCommandHandler()
        {
            string localAppData = this.environmentService.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            this.processService.Start("explorer.exe", this.fileSystem.Path.Combine(localAppData, "NVRAMTuner"));
        }
    }
}
