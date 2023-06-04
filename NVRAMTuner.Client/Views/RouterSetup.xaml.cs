namespace NVRAMTuner.Client.Views
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Windows;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for RouterSetup.xaml
    /// </summary>
    public partial class RouterSetup : UserControl
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="RouterSetup"/> class
        /// </summary>
        public RouterSetup()
        {
            this.InitializeComponent();
            this.DataContext = ((App)Application.Current).ServiceContainer.Services.GetService<RouterSetupViewModel>();
        }
    }
}
