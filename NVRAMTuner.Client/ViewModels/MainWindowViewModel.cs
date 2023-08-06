namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using ControlzEx.Theming;
    using Messages;
    using Messages.Settings;
    using Models.Enums;
    using Services.Interfaces;
    using Services.Wrappers.Interfaces;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;
    using Utils;

    /// <summary>
    /// ViewModel for <see cref="Views.MainWindow"/>. As this is conceptually the
    /// 'highest' level VM, is is delegated the task of handling navigation. It keeps
    /// track of the currently selected ViewModel that is displayed inside the main window.
    ///
    /// Also, any high level errors send via <see cref="DialogErrorMessage"/>s are sent here.
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Instance of <see cref="IMessengerService"/>
        /// </summary>
        private readonly IMessengerService messengerService;

        /// <summary>
        /// Instance of <see cref="IDialogService"/>
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// Instance of <see cref="ISettingsService"/>
        /// </summary>
        private readonly ISettingsService settingsService;

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
        /// The current application theme. Member of the <see cref="ApplicationThemes"/>
        /// enumeration
        /// </summary>
        private ApplicationTheme currentApplicationTheme;

        /// <summary>
        /// Initialises a new instance of the <see cref="MainWindowViewModel"/> class
        /// </summary>
        /// <param name="messengerService">Instance of <see cref="IMessengerService"/></param>
        /// <param name="dialogService">Instance of <see cref="IDialogService"/></param>
        /// <param name="settingsService">Instance of <see cref="ISettingsService"/></param>
        /// <param name="homeViewModel">Instance of <see cref="HomeViewModel"/></param>
        /// <param name="routerSetupViewModel">Instance of <see cref="RouterSetupViewModel"/></param>
        public MainWindowViewModel(
            IMessengerService messengerService,
            IDialogService dialogService,
            ISettingsService settingsService,
            HomeViewModel homeViewModel,
            RouterSetupViewModel routerSetupViewModel)
        {
            this.messengerService = messengerService;
            this.dialogService = dialogService;
            this.settingsService = settingsService;

            this.homeViewModel = homeViewModel;
            this.routerSetupViewModel = routerSetupViewModel;
            this.CurrentViewModel = this.homeViewModel;

            // register messages
            this.messengerService.Register<MainWindowViewModel, NavigationRequestMessage>(this,
                (recipient, message) => recipient.Receive(message));

            this.messengerService.Register<MainWindowViewModel, DialogErrorMessage>(this,
                (recipient, message) => recipient.Receive(message));

            this.messengerService.Register<MainWindowViewModel, ThemeChangeMessage>(this,
                (recipient, message) => recipient.Receive(message));

            this.messengerService.Register<MainWindowViewModel, ThemeRequestMessage>(this,
                (recipient, message) => recipient.Receive(message));

            // sync the application's theme to the host OS by default
            this.currentApplicationTheme = this.settingsService.ApplicationTheme;
            this.SetAndSyncAppTheme();
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
        /// Recipient method for <see cref="DialogErrorMessage"/> instances.
        /// </summary>
        /// <param name="message">The <see cref="DialogErrorMessage"/> instance</param>
        public void Receive(DialogErrorMessage message)
        {
            Task.Run(async () =>
            {
                await this.dialogService.ShowMessageAsync(
                    this,
                    "Error",
                    message.Value.Message);
            });
        }

        /// <summary>
        /// Recipient method for <see cref="NavigationRequestMessage"/> instances
        /// </summary>
        /// <param name="message">The <see cref="NavigationRequestMessage"/> instance</param>
        /// <exception cref="InvalidEnumArgumentException">If the message's value is not a member of
        /// the <see cref="NavigableViewModel"/> enumeration</exception>
        public void Receive(NavigationRequestMessage message)
        {
            this.CurrentViewModel = message.Value switch
            {
                NavigableViewModel.RouterSetupViewModel => this.routerSetupViewModel,
                NavigableViewModel.HomeViewModel => this.homeViewModel,
                _ => throw new InvalidEnumArgumentException(nameof(message.Value))
            };
        }

        /// <summary>
        /// Recipient method for <see cref="ThemeChangeMessage"/> instances.
        /// Updates <see cref="currentApplicationTheme"/> and syncs the application
        /// </summary>
        /// <param name="message">The <see cref="ThemeChangeMessage"/> instance</param>
        public void Receive(ThemeChangeMessage message)
        {
            if (message.Value == this.currentApplicationTheme)
            {
                return;
            }

            this.messengerService.Send(new LogMessage($"Theme changed to: {message.Value}"));
         
            this.currentApplicationTheme = message.Value;
            this.SetAndSyncAppTheme();
        }

        /// <summary>
        /// Recipient method for <see cref="ThemeChangeMessage"/> instances.
        /// Updates <see cref="currentApplicationTheme"/> and syncs the application
        /// </summary>
        /// <param name="message">The <see cref="ThemeChangeMessage"/> instance</param>
        public void Receive(ThemeRequestMessage message)
        {
            message.Reply(this.currentApplicationTheme);
        }

        /// <summary>
        /// Sets the application theme based on the value of <see cref="currentApplicationTheme"/>
        /// </summary>
        private void SetAndSyncAppTheme()
        {
            ThemeManager.Current.ChangeTheme(
                Application.Current, 
                ApplicationThemes.ThemeToString(this.currentApplicationTheme));
            ThemeManager.Current.SyncTheme();
            this.settingsService.ApplicationTheme = this.currentApplicationTheme;
        }
    }
}