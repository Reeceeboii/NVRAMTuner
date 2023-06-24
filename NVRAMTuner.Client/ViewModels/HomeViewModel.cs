﻿#nullable enable

namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using CommunityToolkit.Mvvm.Messaging;
    using MahApps.Metro.Controls.Dialogs;
    using Messages;
    using Models;
    using Models.Enums;
    using Resources;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
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
        /// An instance of <see cref="IDataPersistenceService"/>
        /// </summary>
        private readonly IDataPersistenceService dataPersistenceService;

        /// <summary>
        /// An instance of <see cref="IProcessService"/>
        /// </summary>
        private readonly IProcessService processService;

        /// <summary>
        /// Instance of <see cref="IMessenger"/>
        /// </summary>
        private readonly IMessenger messenger;

        /// <summary>
        /// Backing field for <see cref="RoutersPresent"/>
        /// </summary>
        private bool routersPresent;

        /// <summary>
        /// Instance of <see cref="Router"/>
        /// </summary>
        private Router? activeRouter;

        /// <summary>
        /// Backing field for <see cref="AvailableRouters"/>
        /// </summary>
        private ObservableCollection<Router> availableRouters;

        /// <summary>
        /// Initialises a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        /// <param name="networkService">An instance of <see cref="INetworkService"/></param>
        /// <param name="dialogService">An instance of <see cref="IDialogService"/></param>
        /// <param name="dataPersistenceService">An instance of <see cref="IDataPersistenceService"/></param>
        /// <param name="processService">An instance of <see cref="IProcessService"/></param>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        public HomeViewModel(
            INetworkService networkService,
            IDialogService dialogService, 
            IDataPersistenceService dataPersistenceService,
            IProcessService processService,
            IMessenger messenger)
        {
            this.networkService = networkService;
            this.dialogService = dialogService;
            this.dataPersistenceService = dataPersistenceService;
            this.processService = processService;
            this.messenger = messenger;

            this.LoadRouterFromDiskCommand = new AsyncRelayCommand(this.LoadRouterFromDiskCommandHandlerAsync);

            // menu commands
            this.EnterSetupCommand = new RelayCommand(this.EnterSetupCommandHandler);
            this.ViewSourceMenuCommand = new RelayCommand(this.ViewSourceMenuCommandHandler);
            this.ReportBugMenuCommand = new RelayCommand(this.ReportBugMenuCommandHandler);
        }

        /// <summary>
        /// Gets the async command used to load a previously saved router config from disk
        /// </summary>
        public IAsyncRelayCommand LoadRouterFromDiskCommand { get; }

        /// <summary>
        /// Gets the command used to force entry to the router setup page
        /// </summary>
        public RelayCommand EnterSetupCommand { get; }

        /// <summary>
        /// Gets the command used to access the program's remote source repository
        /// on GitHub via the Menu
        /// </summary>
        public ICommand ViewSourceMenuCommand { get; }

        /// <summary>
        /// Gets the command used to access the issue page for this repository
        /// on GitHub
        /// </summary>
        public ICommand ReportBugMenuCommand { get; }

        /// <summary>
        /// Gets or sets a bool representing whether or not any saved routers are present
        /// </summary>
        public bool RoutersPresent
        {
            get => this.routersPresent;
            set => this.SetProperty(ref this.routersPresent, value);
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
        /// Method to handle the <see cref="LoadRouterFromDiskCommand"/> command
        /// </summary>
        /// <returns>An asynchronous <see cref="Task"/></returns>
        private async Task LoadRouterFromDiskCommandHandlerAsync()
        {
            List<Router> routers = this.dataPersistenceService.DeserialiseAllPresentRouters().ToList();

            if (!routers.Any())
            {
                this.RoutersPresent = false;

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
                    this.messenger.Send(new NavigationRequestMessage(NavigableViewModel.RouterSetupViewModel));
                }

                return;
            }

            this.AvailableRouters = new ObservableCollection<Router>(routers);
            this.RoutersPresent = true;
        }

        #region MenuCommandHandlers

        /// <summary>
        /// Method to handle <see cref="ViewSourceMenuCommand"/>.
        /// Opens the URL of the repository for this project in the user's default browser
        /// </summary>
        private void ViewSourceMenuCommandHandler()
        {
            this.processService.Start(Properties.Resources.RepositoryURL);
        }

        /// <summary>
        /// Method to handle <see cref="ReportBugMenuCommand"/>.
        /// Opens the URL of the GitGub issue page for this project in the user's default browser
        /// </summary>
        private void ReportBugMenuCommandHandler()
        {
            this.processService.Start(Properties.Resources.BugReportURL);
        }

        /// <summary>
        /// Method to handle <see cref="EnterSetupCommand"/>.
        /// Sends a <see cref="NavigationRequestMessage"/> requesting navigation to the
        /// <see cref="RouterSetupViewModel"/>
        /// </summary>
        private void EnterSetupCommandHandler()
        {
            this.messenger.Send(new NavigationRequestMessage(NavigableViewModel.RouterSetupViewModel));
        }

        #endregion
    }
}