namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Services.Wrappers.Interfaces;
    using System;
    using System.Windows.Input;

    /// <summary>
    /// NavigableViewModel for the about window
    /// </summary>
    public class AboutWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Instance of <see cref="IProcessService"/>
        /// </summary>
        private readonly IProcessService processService;

        /// <summary>
        /// Initialises a new instance of the <see cref="AboutWindowViewModel"/> class
        /// </summary>
        /// <param name="processService">Instance of <see cref="IProcessService"/></param>
        public AboutWindowViewModel(IProcessService processService)
        {
            this.processService = processService;
            this.OpenLicenseInBrowserCommand = new RelayCommand(this.OpenLicenseInBrowserCommandHandler);
        }

        /// <summary>
        /// Gets the command to open the license in the user's default browser
        /// </summary>
        public ICommand OpenLicenseInBrowserCommand { get; }

        /// <summary>
        /// Gets the license for the software
        /// </summary>
        public string License => Properties.Resources.GPLv3;

        /// <summary>
        /// Gets the copyright for the software
        /// </summary>
        public string Copyright => $"Copyright © Reece Mercer {DateTime.Now.Year}";

        /// <summary>
        /// Method to handle <see cref="OpenLicenseInBrowserCommand"/>.
        /// Opens the license for this software in the user's default browser
        /// </summary>
        private void OpenLicenseInBrowserCommandHandler()
        {
            this.processService.Start(Properties.Resources.LicenseURL);
        }
    }
}