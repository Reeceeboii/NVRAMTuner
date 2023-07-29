namespace NVRAMTuner.Client.Views
{
    using MahApps.Metro.Controls;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Menu"/> class
        /// </summary>
        public Menu()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event handler for the 'About' option in the menu being clicked.
        /// The action here is to open the <see cref="AboutWindow"/> sub-window
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Instance of <see cref="RoutedEventArgs"/></param>
        private void AboutMenuItemOnClick(object sender, RoutedEventArgs e)
        {
            MetroWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
            aboutWindow.Focus();
        }
    }
}
