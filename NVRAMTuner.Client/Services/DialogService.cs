#nullable enable

namespace NVRAMTuner.Client.Services
{
    using Interfaces;
    using MahApps.Metro.Controls.Dialogs;
    using Ookii.Dialogs.Wpf;
    using System.Threading.Tasks;

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
        /// Initialises a new instance of the <see cref="DialogService"/> class
        /// </summary>
        /// <param name="dialogCoordinator">Instance of <see cref="IDialogCoordinator"/></param>
        public DialogService(IDialogCoordinator dialogCoordinator)
        {
            this.dialogCoordinator = dialogCoordinator;
        }

        #region MetroDialogs

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

        #endregion

        /// <summary>
        /// Displays a folder selection dialog, and if a folder is selected, returns the path to that folder.
        /// If no path is selected, or the window is closed, then an empty string is returned. This difference
        /// needs to be checked by callers.
        /// </summary>
        /// <param name="description">A description of the dialog</param>
        /// <param name="multiSelect">Whether or not multiple folders should be allowed to be selected</param>
        /// <returns></returns>
        public string ShowFolderBrowserDialog (string description, bool multiSelect)
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
    }
}