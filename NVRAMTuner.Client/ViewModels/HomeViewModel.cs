namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Events;
    using MahApps.Metro.Controls.Dialogs;
    using Messages;
    using Messages.Theme;
    using Messages.Variables.Staged;
    using Models;
    using Models.Enums;
    using Resources;
    using Services.Interfaces;
    using Services.Network.Interfaces;
    using Services.Wrappers.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

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
        /// An instance of <see cref="IDataPersistenceService"/>
        /// </summary>
        private readonly IDataPersistenceService dataPersistenceService;

        /// <summary>
        /// An instance of <see cref="IProcessService"/>
        /// </summary>
        private readonly IProcessService processService;

        /// <summary>
        /// An instance of <see cref="IVariableService"/>
        /// </summary>
        private readonly IVariableService variableService;

        /// <summary>
        /// Instance of <see cref="IMessengerService"/>
        /// </summary>
        private readonly IMessengerService messengerService;

        /// <summary>
        /// Backing field for <see cref="RoutersPresent"/>
        /// </summary>
        private bool routersPresent;

        /// <summary>
        /// Backing field for <see cref="IsLoading"/>
        /// </summary>
        private bool isLoading;

        /// <summary>
        /// Backing field for <see cref="IsConnected"/>
        /// </summary>
        private bool isConnected;
        
        /// <summary>
        /// Backing field for <see cref="AvailableRouters"/>
        /// </summary>
        private ObservableCollection<Router> availableRouters;

        /// <summary>
        /// Backing fiend for <see cref="TargetRouterForConnection"/>
        /// </summary>
        private Router targetRouterForConnection;

        /// <summary>
        /// Backing field for <see cref="CommandsRanAgainstTargetRouter"/>
        /// </summary>
        private int commandsRanAgainstTargetRouter;

        /// <summary>
        /// Backing field for <see cref="ActiveConnectionElapsedTime"/>
        /// </summary>
        private string activeConnectionElapsedTime;

        /// <summary>
        /// Initialises a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        /// <param name="networkService">An instance of <see cref="INetworkService"/></param>
        /// <param name="dialogService">An instance of <see cref="IDialogService"/></param>
        /// <param name="dataPersistenceService">An instance of <see cref="IDataPersistenceService"/></param>
        /// <param name="processService">An instance of <see cref="IProcessService"/></param>
        /// <param name="variableService">An instance of <see cref="IVariableService"/></param>
        /// <param name="messengerService">An instance of <see cref="IMessengerService"/></param>
        public HomeViewModel(
            INetworkService networkService,
            IDialogService dialogService, 
            IDataPersistenceService dataPersistenceService,
            IProcessService processService,
            IVariableService variableService,
            IMessengerService messengerService)
        {
            this.networkService = networkService;
            this.dialogService = dialogService;
            this.dataPersistenceService = dataPersistenceService;
            this.processService = processService;
            this.variableService = variableService;
            this.messengerService = messengerService;

            this.networkService.CommandRan += this.NetworkServiceOnCommandRan;
            this.networkService.ConnectionTimerSecondTick += this.NetworkServiceOnConnectionTimerSecondTick;

            this.availableRouters = new ObservableCollection<Router>();

            this.ConnectToTargetRouterCommand = new AsyncRelayCommand(this.ConnectToTargetRouterCommandHandler);
            this.DisconnectFromTargetRouterCommand =
                new AsyncRelayCommand(this.DisconnectFromTargetRouterCommandHandler, () => this.IsConnected);

            // menu commands
            this.EnterSetupCommand = new AsyncRelayCommand(this.EnterSetupCommandHandler);
            this.ViewSourceMenuCommand = new RelayCommand(this.ViewSourceMenuCommandHandler);
            this.ReportBugMenuCommand = new RelayCommand(this.ReportBugMenuCommandHandler);
            this.ChangeThemeCommand = new RelayCommand<ApplicationTheme>(this.ChangeThemeCommandHandler);
            this.OpenSettingsFlyoutCommand = new RelayCommand(this.OpenSettingsFlyoutCommandHandler);

            this.LoadRouterFromDisk();
        }

        /// <summary>
        /// Gets the async command used to connect to the <see cref="TargetRouterForConnection"/>
        /// </summary>
        public IAsyncRelayCommand ConnectToTargetRouterCommand { get; }

        /// <summary>
        /// Gets the async command used to disconnect from the <see cref="TargetRouterForConnection"/>
        /// </summary>
        public IAsyncRelayCommand DisconnectFromTargetRouterCommand { get; }

        /// <summary>
        /// Gets the command used to force entry to the router setup page
        /// </summary>
        public IAsyncRelayCommand EnterSetupCommand { get; }

        /// <summary>
        /// Gets the command used to access the program's remote source repository
        /// on GitHub via the Menu
        /// </summary>
        public IRelayCommand ViewSourceMenuCommand { get; }

        /// <summary>
        /// Gets the command used to access the issue page for this repository
        /// on GitHub
        /// </summary>
        public IRelayCommand ReportBugMenuCommand { get; }

        /// <summary>
        /// Gets the command used to change the theme via the menu
        /// </summary>
        public IRelayCommand<ApplicationTheme> ChangeThemeCommand { get; }

        /// <summary>
        /// Gets the command used to indicate the user wishes to open the settings flyout
        /// </summary>
        public IRelayCommand OpenSettingsFlyoutCommand { get; }

        /// <summary>
        /// Gets or sets a bool representing whether or not any saved routers are present
        /// </summary>
        public bool RoutersPresent
        {
            get => this.routersPresent;
            set => this.SetProperty(ref this.routersPresent, value);
        }

        /// <summary>
        /// Gets or sets a bool representing whether NVRAMTuner is loading (e.g. waiting on network etc...)
        /// </summary>
        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        /// <summary>
        /// Gets or sets a bool representing whether NVRAMTuner is currently connected to a router
        /// </summary>
        public bool IsConnected
        {
            get => this.isConnected;
            set => this.SetProperty(ref this.isConnected, value);
        }

        /// <summary>
        /// Gets or sets a <see cref="Router"/> that is currently the user's selected target for a connection
        /// </summary>
        public Router TargetRouterForConnection
        {
            get => this.targetRouterForConnection;
            set => this.SetProperty(ref this.targetRouterForConnection, value);
        }

        /// <summary>
        /// Gets or sets an <see cref="ObservableCollection{T}"/> of all the available
        /// <see cref="Router"/> instances that are available for the user to connect to
        /// </summary>
        public ObservableCollection<Router> AvailableRouters
        {
            get => this.availableRouters;
            set => this.SetProperty(ref this.availableRouters, value);
        }

        /// <summary>
        /// Gets or sets an integer representing how many commands have been executed against the
        /// <see cref="TargetRouterForConnection"/>
        /// </summary>
        public int CommandsRanAgainstTargetRouter
        {
            get => this.commandsRanAgainstTargetRouter;
            private set => this.SetProperty(ref this.commandsRanAgainstTargetRouter, value);
        }

        /// <summary>
        /// Gets or privately sets the string representation of the amount of time that the current
        /// router connection has been elapsed for
        /// </summary>
        public string ActiveConnectionElapsedTime
        {
            get => this.activeConnectionElapsedTime;
            private set => this.SetProperty(ref this.activeConnectionElapsedTime, value);
        }

        /// <summary>
        /// Gets a string denoting the current status of NVRAMTuner - to be displayed in the status bar
        /// </summary>
        public string NvramTunerStatus =>
            this.IsConnected
                ? $"Connected to '{this.targetRouterForConnection.RouterNickname}'"
                : "Disconnected";

        /// <summary>
        /// Handles the <see cref="ConnectToTargetRouterCommand"/>
        /// </summary>
        /// <returns>An asynchronous <see cref="Task"/></returns>
        private async Task ConnectToTargetRouterCommandHandler()
        {
            this.IsLoading = true;

            try
            {
                await this.networkService.ConnectToRouterAsync(this.targetRouterForConnection, useTempClient: false);
            }
            catch (Exception ex)
            {
                await this.dialogService.ShowMessageAsync(
                    this,
                    $"Connection to {this.targetRouterForConnection.RouterNickname} failed",
                    $"An attempt was made to connect to your router at {this.targetRouterForConnection.RouterIpv4Address}. " +
                    $"However, an error occurred: \n\n {ex.Message}");
            }
            finally
            {
                this.IsLoading = false;
            }

            if (this.networkService.IsConnected)
            {
                this.IsLoading = true;
                this.IsConnected = true;

                this.OnPropertyChanged(nameof(this.NvramTunerStatus));
                this.DisconnectFromTargetRouterCommand.NotifyCanExecuteChanged();
                this.OnPropertyChanged(nameof(this.DisconnectFromTargetRouterCommand));

                await this.variableService.GetNvramVariablesAsync();
                this.IsLoading = false;
            }
        }

        /// <summary>
        /// Method to load all routers from disk
        /// </summary>
        private void LoadRouterFromDisk()
        {
            this.IsLoading = true;

            List<Router> routers = this.dataPersistenceService.DeserialiseAllPresentRouters().ToList();

            if (!routers.Any())
            {
                this.RoutersPresent = false;

                Task.Run(async () =>
                {
                    MessageDialogResult setupRes = await this.dialogService.ShowMessageAsync(
                        this,
                        "No saved routers present",
                        ViewModelStrings.NoSavedRoutersFoundDialogMessage,
                        MessageDialogStyle.AffirmativeAndNegative,
                        new MetroDialogSettings
                        {
                            AffirmativeButtonText = "Yes, enter setup now",
                            NegativeButtonText = "No, complete setup later",
                            DefaultButtonFocus = MessageDialogResult.Affirmative,
                        });

                    if (setupRes == MessageDialogResult.Affirmative)
                    {
                        this.messengerService.Send(new NavigationRequestMessage(NavigableViewModel.RouterSetupViewModel));
                    }
                });

                this.IsLoading = false;
                return;
            }
   
            this.AvailableRouters = new ObservableCollection<Router>(routers);
            this.RoutersPresent = true;
            this.TargetRouterForConnection = routers[0];

            this.IsLoading = false;
        }

        /// <summary>
        /// Method to handle the <see cref="DisconnectFromTargetRouterCommand"/>.
        /// Disconnects from the <see cref="TargetRouterForConnection"/> if a connection is established via
        /// the <see cref="INetworkService"/>
        /// </summary>
        /// <returns>An asynchronous <see cref="Task"/></returns>
        private async Task DisconnectFromTargetRouterCommandHandler()
        {
            this.IsLoading = true;

            RequestNumOfStagedVariablesMessage reqStaged = this.messengerService.Send<RequestNumOfStagedVariablesMessage>();
            if (reqStaged.Response >= 1)
            {
                MessageDialogResult result = await this.dialogService.ShowMessageAsync(
                    this,
                    "Abandon staged changes?",
                    ViewModelStrings.DisconnectWhileVariablesAreStagedDialogMessage,
                    MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings
                    {
                        AffirmativeButtonText = "Disconnect",
                        NegativeButtonText = "Cancel",
                        DefaultButtonFocus = MessageDialogResult.Affirmative
                    });

                if (result == MessageDialogResult.Negative)
                {
                    this.IsLoading = false;
                    return;
                }
            }

            if (this.networkService.IsConnected)
            {
                await this.networkService.DisconnectFromRouter();
                this.IsConnected = false;

                this.DisconnectFromTargetRouterCommand.NotifyCanExecuteChanged();
                this.OnPropertyChanged(nameof(this.NvramTunerStatus));
                this.messengerService.Send(new RouterDisconnectMessage(this.TargetRouterForConnection));
                this.messengerService.Send(new ClearStagedVariablesMessage());
            }

            this.CommandsRanAgainstTargetRouter = 0;
            this.IsLoading = false;
        }

        /// <summary>
        /// Method to handle the <see cref="ViewSourceMenuCommand"/>.
        /// Opens the URL of the repository for this project in the user's default browser
        /// </summary>
        private void ViewSourceMenuCommandHandler()
        {
            this.processService.Start(Properties.Resources.RepositoryURL);
        }

        /// <summary>
        /// Method to handle the <see cref="ReportBugMenuCommand"/>.
        /// Opens the URL of the GitGub issue page for this project in the user's default browser
        /// </summary>
        private void ReportBugMenuCommandHandler()
        {
            this.processService.Start(Properties.Resources.BugReportURL);
        }

        /// <summary>
        /// Method to handle the <see cref="EnterSetupCommand"/>.
        /// Sends a <see cref="NavigationRequestMessage"/> requesting navigation to the
        /// <see cref="RouterSetupViewModel"/>
        /// </summary>
        /// <returns>The asynchronous <see cref="Task"/></returns>
        private async Task EnterSetupCommandHandler()
        {
            string message = this.networkService.IsConnected
                ? "To setup a router, any currently established connections will be dropped and you will then be " +
                  "redirected to the router setup page"
                : "You will be redirected to the router setup page";

            MessageDialogResult res = await this.dialogService.ShowMessageAsync(
                this,
                "Setup a new router",
                message,
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Enter setup",
                    NegativeButtonText = "Close"
                });

            if (res == MessageDialogResult.Affirmative)
            {
                this.IsLoading = true;

                if (this.IsConnected)
                {
                    await this.DisconnectFromTargetRouterCommand.ExecuteAsync(null);
                }

                this.IsLoading = false;

                this.messengerService.Send(new NavigationRequestMessage(NavigableViewModel.RouterSetupViewModel));
            }
        }

        /// <summary>
        /// Method to handle the <see cref="ChangeThemeCommand"/>
        /// </summary>
        /// <param name="theme">Member of the <see cref="ApplicationTheme"/> enumeration</param>
        private void ChangeThemeCommandHandler(ApplicationTheme theme)
        {
            this.messengerService.Send(new ThemeChangeMessage(theme));
        }

        /// <summary>
        /// Method to handle the <see cref="OpenSettingsFlyoutCommand"/>
        /// </summary>
        private void OpenSettingsFlyoutCommandHandler()
        {
            this.messengerService.Send(new OpenSettingsFlyoutMessage());
        }

        /// <summary>
        /// Handles receiving <see cref="INetworkService.CommandRan"/> events
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void NetworkServiceOnCommandRan(object sender, EventArgs e)
        {
            this.CommandsRanAgainstTargetRouter++;
        }

        /// <summary>
        /// Handles receiving <see cref="INetworkService.ConnectionTimerSecondTick"/> events
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments (<see cref="ActiveConnectionTimerTickArgs"/>)</param>
        private void NetworkServiceOnConnectionTimerSecondTick(object sender, ActiveConnectionTimerTickArgs e)
        {
            this.ActiveConnectionElapsedTime = e.ElapsedPretty;
        }
    }
}