namespace NVRAMTuner.Client.Views.Flyouts
{
    using MahApps.Metro.Controls;
    using Microsoft.Extensions.DependencyInjection;
    using System.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for SettingsFlyout.xaml
    /// </summary>
    public partial class SettingsFlyout : Flyout
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="SettingsFlyout"/> class
        /// </summary>
        public SettingsFlyout()
        {
            this.InitializeComponent();
            this.DataContext = this.DataContext = ((App)Application.Current).ServiceContainer.Services.GetService<SettingsFlyoutViewModel>();
        }
    }
}
