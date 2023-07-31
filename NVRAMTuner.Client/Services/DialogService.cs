namespace NVRAMTuner.Client.Services
{
    using Interfaces;
    using MahApps.Metro.Controls.Dialogs;
    using Ookii.Dialogs.Wpf;
    using System;
    using System.Threading.Tasks;
    using Wrappers.Interfaces;

    /// <summary>
    /// Service class for handling any dialogs that are displayed to the user
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Instance of <see cref="IDialogCoordinator"/>
        /// </summary>
        private readonly IDialogCoordinator dialogCoordinator;

        /// <summary>
        /// Instance of <see cref="IEnvironmentService"/>
        /// </summary>
        private readonly IEnvironmentService environmentService;

        /// <summary>
        /// Initialises a new instance of the <see cref="DialogService"/> class
        /// </summary>
        /// <param name="dialogCoordinator">Instance of <see cref="IDialogCoordinator"/></param>
        /// <param name="environmentService">Instance of <see cref="IEnvironmentService"/></param>
        public DialogService(IDialogCoordinator dialogCoordinator, IEnvironmentService environmentService)
        {
            this.dialogCoordinator = dialogCoordinator;
            this.environmentService = environmentService;
        }

        /// <summary>
        /// Creates a message dialog inside the current window. Accepts a context parameter, which is typically
        /// the ViewModel belonging to the view to which the dialog should be attached
        /// </summary>
        /// <param name="ctx">The context (as mentioned, usually a ViewModel class)</param>
        /// <param name="title">The title of the dialog</param>
        /// <param name="message">The content of the dialog</param>
        /// <param name="style">The style of the dialog. Defaults to <see cref="MessageDialogStyle.Affirmative"/></param>
        /// <param name="settings">Settings of the dialog. Defaults to null</param>
        /// <returns>An asynchronous <see cref="Task"/> wrapping a <see cref="MessageDialogResult"/> instance</returns>
        public async Task<MessageDialogResult> ShowMessageAsync(
            object ctx,
            string title,
            string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative,
            MetroDialogSettings? settings = null)
        {
            return await this.dialogCoordinator.ShowMessageAsync(ctx, title, message, style, settings);
        }

        /// <summary>
        /// Displays a folder selection dialog, and if a folder is selected, returns the path to that folder.
        /// If no path is selected, or the window is closed, then an empty string is returned. This difference
        /// needs to be checked by callers.
        /// </summary>
        /// <param name="description">A description of the dialog</param>
        /// <param name="multiSelect">Whether or not multiple folders should be allowed to be selected</param>
        /// <returns>The selected folder path(s), or an empty string</returns>
        public string ShowFolderBrowserDialog(string description, bool multiSelect)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog
            {
                Description = description, 
                Multiselect = multiSelect, 
                UseDescriptionForTitle = true
            };

            return dialog.ShowDialog() == true 
                ? dialog.SelectedPath 
                : string.Empty;
        }

        /// <summary>
        /// Displays a "Save as" dialog, and if a path is selected, returns the path to that file.
        /// If no path is selected, or the window is closed, then an empty string is returned. This
        /// difference needs to be checked by callers.
        /// </summary>
        /// <param name="filter">The file filter to apply to the dialog</param>
        /// <param name="fileName">The default filename to apply to the dialog</param>
        /// <returns>The selected file path, or an empty string</returns>
        public string ShowSaveAsDialog(string filter, string fileName)
        {
            VistaSaveFileDialog dialog = new VistaSaveFileDialog
            {
                Filter = filter,
                InitialDirectory = this.environmentService.GetFolderPath(Environment.SpecialFolder.UserProfile),
                FileName = fileName,
                Title = @"Save as",
                ValidateNames = true
            };

            return dialog.ShowDialog() == true
                ? dialog.FileName
                : string.Empty;
        }
    }
}