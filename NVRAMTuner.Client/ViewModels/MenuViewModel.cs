namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Services.Interfaces;
    using System.IO.Abstractions;
    using System.Windows.Input;

    /// <summary>
    /// NavigableViewModel for the <see cref="Views.Menu"/> view
    /// </summary>
    public class MenuViewModel : ObservableObject
    {
        /// <summary>
        /// Instance of <see cref="IProcessService"/>
        /// </summary>
        private readonly IProcessService processService;

        /// <summary>
        /// Instance of <see cref="IFileSystem"/>
        /// </summary>
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Initialises a new instance of the <see cref="MenuViewModel"/> class
        /// </summary>
        /// <param name="processService">Instance of <see cref="IProcessService"/></param>
        /// <param name="fileSystem">Instance of <see cref="IFileSystem"/></param>
        public MenuViewModel(IProcessService processService, IFileSystem fileSystem)
        {
            this.processService = processService;
            this.fileSystem = fileSystem;

            this.ViewSourceCommand = new RelayCommand(this.ViewSourceCommandHandler);
            this.ReportBugCommand = new RelayCommand(this.ReportBugCommandHandler);
        }

        /// <summary>
        /// Gets the command used to access the program's remote source repository
        /// on GitHub via the Menu
        /// </summary>
        public ICommand ViewSourceCommand { get; }

        /// <summary>
        /// Gets the command used to access the issue page for this repository
        /// on GitHub
        /// </summary>
        public ICommand ReportBugCommand { get; }

        /// <summary>
        /// Method to handle <see cref="ViewSourceCommand"/>.
        /// Opens the URL of the repository for this project in the user's default browser
        /// </summary>
        private void ViewSourceCommandHandler()
        {
            this.processService.Start(Properties.Resources.RepositoryURL);
        }

        /// <summary>
        /// Method to handle <see cref="ReportBugCommand"/>.
        /// Opens the URL of the GitGub issue page for this project in the user's default
        /// browser
        /// </summary>
        private void ReportBugCommandHandler()
        {
            this.processService.Start(Properties.Resources.BugReportURL);
        }
    }
}