namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Messaging;
    using Messages;
    using Models.Enums;
    using System.ComponentModel;

    /// <summary>
    /// ViewModel for <see cref="Views.MainWindow"/>. As this is conceptually the
    /// 'highest' level VM, is is delegated the task of handling navigation. It keeps
    /// track of the currently selected ViewModel that is displayed inside 
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Instance of <see cref="IMessenger"/>
        /// </summary>
        private readonly IMessenger messenger;

        /// <summary>
        /// The currently visible ViewModel. This is changed in response to navigation events.
        /// </summary>
        private ObservableObject currentViewModel;

        /// <summary>
        /// Instance of <see cref="HomeViewModel"/>
        /// </summary>
        private readonly HomeViewModel homeViewModel;

        /// <summary>
        /// Instance of <see cref="RouterSetupViewModel"/>
        /// </summary>
        private readonly RouterSetupViewModel routerSetupViewModel;

        /// <summary>
        /// Initialises a new instance of the <see cref="MainWindowViewModel"/> class
        /// </summary>
        /// <param name="messenger">Instance of <see cref="IMessenger"/></param>
        /// <param name="homeViewModel">Instance of <see cref="HomeViewModel"/></param>
        /// <param name="routerSetupViewModel">Instance of <see cref="RouterSetupViewModel"/></param>
        public MainWindowViewModel(
            IMessenger messenger,
            HomeViewModel homeViewModel,
            RouterSetupViewModel routerSetupViewModel)
        {
            this.homeViewModel = homeViewModel;
            this.routerSetupViewModel = routerSetupViewModel;
            this.CurrentViewModel = this.homeViewModel;

            messenger.Register<NavigationRequestMessage>(this, this.NavigationRequestMessageHandler);
        }

        /// <summary>
        /// Gets or sets the current view model that determines what major view appears
        /// in the main window. For a full list of what ViewModels are able to be navigated
        /// to, see the <see cref="NavigableViewModel"/> enumeration
        /// </summary>
        public ObservableObject CurrentViewModel
        {
            get => this.currentViewModel;
            set => this.SetProperty(ref this.currentViewModel, value);
        }

        /// <summary>
        /// Method for handling receiving the <see cref="NavigationRequestMessage"/>
        /// </summary>
        /// <param name="recipient">The recipient of the message</param>
        /// <param name="message">An instance of <see cref="NavigationRequestMessage"/></param>
        private void NavigationRequestMessageHandler(object recipient, NavigationRequestMessage message)
        {
            switch (message.Value)
            {
                case NavigableViewModel.RouterSetupViewModel:
                    this.CurrentViewModel = this.routerSetupViewModel;
                    break;
                case NavigableViewModel.HomeViewModel:
                    this.CurrentViewModel = this.homeViewModel;
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(message.Value));
            }
        }
    }
}