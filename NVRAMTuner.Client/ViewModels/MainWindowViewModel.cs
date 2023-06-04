namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Messaging;
    using Messages;
    using Models.Enums;
    using System.Diagnostics;

    /// <summary>
    /// ViewModel for <see cref="Views.MainWindow"/>. As this is conceptually the
    /// 'highest' level VM, is is delegated the task of handling navigation. It keeps
    /// track of the currently selected ViewModel that is displayed inside 
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {
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
        /// <param name="homeViewModel">Instance of <see cref="HomeViewModel"/></param>
        /// <param name="routerSetupViewModel">Instance of <see cref="RouterSetupViewModel"/></param>
        public MainWindowViewModel(HomeViewModel homeViewModel, RouterSetupViewModel routerSetupViewModel)
        {
            this.CurrentViewModel = homeViewModel;
            this.routerSetupViewModel = routerSetupViewModel;

            WeakReferenceMessenger.Default.Register<NavigationRequestMessage>(this, this.NavigationRequestMessageHandler);
        }

        public ObservableObject CurrentViewModel
        {
            get => this.currentViewModel;
            set => this.SetProperty(ref this.currentViewModel, value);
        }

        private void NavigationRequestMessageHandler(object recipient, NavigationRequestMessage message)
        {
            switch (message.Value)
            {
                case NavigableViewModel.RouterSetupViewModel:
                    this.CurrentViewModel = this.routerSetupViewModel;
                    break;
                default:
                    Debug.WriteLine("Invalid enum value");
                    break;
            }
        }
    }
}