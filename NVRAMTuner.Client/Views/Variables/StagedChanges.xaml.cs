namespace NVRAMTuner.Client.Views.Variables
{
    using Microsoft.Extensions.DependencyInjection;
    using NVRAMTuner.Client.ViewModels.Variables;
    using System.Windows;
    using System.Windows.Controls;

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
    }
}
