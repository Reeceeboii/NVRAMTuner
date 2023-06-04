namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using CommunityToolkit.Mvvm.Messaging;
    using Messages;
    using Models.Enums;
    using Services.Interfaces;
    using System.Diagnostics;
    using System.Windows.Input;

    /// <summary>
    /// ViewModel for <see cref="Views.Home"/>
    /// </summary>
    public class HomeViewModel : ObservableObject
    {
        /// <summary>
        /// An instance of <see cref="INetworkService"/>
        /// </summary>
        private readonly INetworkService networkService;

        /// <summary>
        /// An instance of <see cref="IDialogService"/>
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// Initialises a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        /// <param name="networkService">An instance of <see cref="INetworkService"/></param>
        /// <param name="dialogService">An instance of <see cref="IDialogService"/></param>
        public HomeViewModel(INetworkService networkService, IDialogService dialogService)
        {
            this.networkService = networkService;
            this.dialogService = dialogService;

            this.InitiateRouterSetupCommand = new RelayCommand(this.InitiateRouterSetupCommandHandler);
        }

        /// <summary>
        /// Gets the command used to initiate the process of setting up a connection to a new router
        /// </summary>
        public ICommand InitiateRouterSetupCommand { get; }

        private void InitiateRouterSetupCommandHandler()
        {
            Debug.WriteLine("Moving to router setup...");
            WeakReferenceMessenger.Default.Send(new NavigationRequestMessage(NavigableViewModel.RouterSetupViewModel));
        }
    }
}