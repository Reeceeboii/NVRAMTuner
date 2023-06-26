namespace NVRAMTuner.Client.Views
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Windows;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs : UserControl
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Logs"/> class
        /// </summary>
        public Logs()
        {
            this.InitializeComponent();
            this.DataContext = ((App)Application.Current).ServiceContainer.Services.GetService<LogsViewModel>();
        }

        /// <summary>
        /// Event for handling scrolling to the bottom of the <see cref="TextBox"/> that holds
        /// all of NVRAMTuner's logs
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">A <see cref="TextChangedEventArgs"/> instance</param>
        private void LogTextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.ScrollToEnd();
            }
        }
    }
}
