﻿namespace NVRAMTuner.Client.Services.Interfaces
{
    using MahApps.Metro.Controls.Dialogs;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface for a service that handles dialogs
    /// </summary>
    public interface IDialogService
    {
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
        Task<MessageDialogResult> ShowMessageAsync(
            object ctx,
            string title,
            string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative,
            MetroDialogSettings settings = null);

        /// <summary>
        /// Displays a folder selection dialog, and if a folder is selected, returns the path to that folder.
        /// If no path is selected, or the window is closed, then an empty string is returned. This difference
        /// needs to be checked by callers.
        /// </summary>
        /// <param name="description">A description of the dialog</param>
        /// <param name="multiSelect">Whether or not multiple folders should be allowed to be selected</param>
        /// <returns>The selected folder path(s), or an empty string</returns>
        string ShowFolderBrowserDialog(string description, bool multiSelect);

        /// <summary>
        /// Displays a "Save as" dialog, and if a path is selected, returns the path to that file.
        /// If no path is selected, or the window is closed, then an empty string is returned. This
        /// difference needs to be checked by callers.
        /// </summary>
        /// <param name="filter">The file filter to apply to the dialog</param>
        /// <param name="fileName">The default filename to apply to the dialog</param>
        /// <returns>The selected file path, or an empty string</returns>
        string ShowSaveAsDialog(string filter, string fileName);
    }
}