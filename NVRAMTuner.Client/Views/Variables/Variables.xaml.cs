namespace NVRAMTuner.Client.Views.Variables
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Windows;
    using System.Windows.Controls;
    using ViewModels.Variables;

    /// <summary>
    /// Interaction logic for Variables.xaml
    /// </summary>
    public partial class Variables : UserControl
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Variables"/> class
        /// </summary>
        public Variables()
        {
            this.InitializeComponent();
            this.DataContext = ((App)Application.Current).ServiceContainer.Services.GetService<VariablesViewModel>();
        }
    }
}
