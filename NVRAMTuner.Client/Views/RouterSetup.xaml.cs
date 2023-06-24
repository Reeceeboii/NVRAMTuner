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

        /// <summary>
        /// Meh... I don't like code behind things like this. Maybe I'm being too rigid with the whole MVVM thing.
        ///
        /// Whenever the password changes, update the property that should be bound, but that cannot be bound
        /// due to the internal implementation of the <see cref="PasswordBox"/> control
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The routed event args</param>
        private void PasswordBoxOnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((RouterSetupViewModel)this.DataContext).SshPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
