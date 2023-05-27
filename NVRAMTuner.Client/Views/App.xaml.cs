namespace NVRAMTuner.Client.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="App"/> class
        /// </summary>
        public App()
        {
            this.ServiceContainer = new ServiceContainer();
        }

        /// <summary>
        /// Gets an instance of <see cref="Client.ServiceContainer"/>
        /// </summary>
        public ServiceContainer ServiceContainer { get; }
    }
}
