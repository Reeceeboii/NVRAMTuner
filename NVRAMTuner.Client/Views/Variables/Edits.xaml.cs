namespace NVRAMTuner.Client.Views.Variables
{
    using Microsoft.Extensions.DependencyInjection;
    using NVRAMTuner.Client.ViewModels.Variables;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for Edits.xaml
    /// </summary>
    public partial class Edits : UserControl
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Edits"/> class
        /// </summary>
        public Edits()
        {
            this.InitializeComponent();
            this.DataContext = ((App)Application.Current).ServiceContainer.Services.GetService<EditsViewModel>();
        }
    }
}
