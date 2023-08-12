namespace NVRAMTuner.Client.Views.Variables
{
    using Microsoft.Extensions.DependencyInjection;
    using NVRAMTuner.Client.ViewModels.Variables;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for StagedChanges.xaml
    /// </summary>
    public partial class StagedChanges : UserControl
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="StagedChanges"/> class
        /// </summary>
        public StagedChanges()
        {
            // TODO - push this diff stuff into viewmodels/services so it can be tested
            this.InitializeComponent();
            this.DataContext = ((App)Application.Current).ServiceContainer.Services.GetService<StagedChangesViewModel>();
        }

        /// <summary>
        /// Mouse double click event for staged variables, opens the diff window
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">The event args</param>
        private void StagedVariableControlOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenDiffWindow();
        }

        /// <summary>
        /// Mouse click event for the 'view diff' option in the staged variable's context menu
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">The event args</param>
        private void StagedVariableCtxMenuOnClick(object sender, RoutedEventArgs e)
        {
            OpenDiffWindow();
        }

        /// <summary>
        /// Opens the diff window
        /// </summary>
        private static void OpenDiffWindow()
        {
            VariableDiffWindow diffWindow = new VariableDiffWindow
            {
                DataContext = ((App)Application.Current).ServiceContainer.Services.GetService<VariableDiffWindowViewModel>(),
                Owner = ((App)Application.Current).MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            diffWindow.ShowDialog();
            diffWindow.Focus();
        }
    }
}
