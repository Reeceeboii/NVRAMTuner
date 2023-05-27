namespace NVRAMTuner.Client.Views
{
    using MahApps.Metro.Controls;
    using Microsoft.Extensions.DependencyInjection;
    using System.ComponentModel;
    using System.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : MetroWindow
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AboutWindow"/> class
        /// </summary>
        public AboutWindow()
        {
            this.InitializeComponent();
            this.DataContext = ((App)Application.Current).ServiceContainer.Services.GetService<AboutWindowViewModel>();
        }
    }
}
