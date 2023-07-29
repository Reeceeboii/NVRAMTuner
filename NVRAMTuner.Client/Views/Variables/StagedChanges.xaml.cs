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
            this.InitializeComponent();
            this.DataContext = ((App)Application.Current).ServiceContainer.Services.GetService<StagedChangesViewModel>();
        }

        /// <summary>
        /// MouseDown event handler for handling clicks on the staged variables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StagedVariableOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // ignore everything but double clicks
            if (e.ClickCount != 2)
            {
                return;
            }

            VariableDiffWindow diffWindow = new VariableDiffWindow
            {
                DataContext = ((App)Application.Current).ServiceContainer.Services.GetService<VariableDiffWindowViewModel>()
            };

            diffWindow.ShowDialog();
            diffWindow.Focus();
        }
    }
}
