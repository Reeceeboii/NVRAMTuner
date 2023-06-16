namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using CommunityToolkit.Mvvm.Messaging;
    using Messages;
    using Models.Enums;
    using Services.Interfaces;
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
        /// Instance of <see cref="IMessenger"/>
        /// </summary>
        private readonly IMessenger messenger;

        /// <summary>
        /// Initialises a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        /// <param name="networkService">An instance of <see cref="INetworkService"/></param>
        /// <param name="dialogService">An instance of <see cref="IDialogService"/></param>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        public HomeViewModel(INetworkService networkService, IDialogService dialogService, IMessenger messenger)
        {
            this.networkService = networkService;
            this.dialogService = dialogService;
            this.messenger = messenger;

            this.InitiateRouterSetupCommand = new RelayCommand(this.InitiateRouterSetupCommandHandler);
        }

        /// <summary>
        /// Gets the command used to initiate the process of setting up a connection to a new router
        /// </summary>
        public ICommand InitiateRouterSetupCommand { get; }

        /// <summary>
        /// Method to handle the <see cref="InitiateRouterSetupCommand"/>
        /// </summary>
        private void InitiateRouterSetupCommandHandler()
        {
            this.messenger.Send(new NavigationRequestMessage(NavigableViewModel.RouterSetupViewModel));
        }
    }
}