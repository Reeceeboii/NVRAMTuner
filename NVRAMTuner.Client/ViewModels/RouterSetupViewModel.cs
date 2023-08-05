#nullable enable

namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using MahApps.Metro.Controls.Dialogs;
    using Messages;
    using Models;
    using Models.Enums;
    using Resources;
    using Services;
    using Services.Interfaces;
    using Services.Wrappers.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Validators;

    /// <summary>
    /// ViewModel for the RouterSetup view
    /// </summary>
    public class RouterSetupViewModel : ObservableValidator
    {
        /// <summary>
        /// Instance of <see cref="INetworkService"/>
        /// </summary>
        private readonly INetworkService networkService;

        /// <summary>
        /// Instance of <see cref="IDialogService"/>
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// Instance of <see cref="IDataPersistenceService"/>
        /// </summary>
        private readonly IDataPersistenceService dataPersistenceService;

        /// <summary>
        /// Instance of <see cref="IMessengerService"/>
        /// </summary>
        private readonly IMessengerService messengerService;

        /// <summary>
        /// Backing field for <see cref="RouterIpv4Address"/>
        /// </summary>
        private string routerIpv4Address;

        /// <summary>
        /// Backing field for <see cref="SshPort"/>
        /// </summary>
        private string sshPort;

        /// <summary>
        /// Backing field for <see cref="SshKeyFolder"/>
        /// </summary>
        private string sshKeyFolder;

        /// <summary>
        /// Backing field for <see cref="UserIsUsingSshKeys"/>
        /// </summary>
        private bool userIsUsingSshKeys;

        /// <summary>
        /// Backing field for <see cref="SshUsername"/>
        /// </summary>
        private string sshUsername;

        /// <summary>
        /// Backing field for <see cref="SshPassword"/>
        /// </summary>
        private string sshPassword;

        /// <summary>
        /// Backing field for <see cref="RouterNickname"/>
        /// </summary>
        private string routerNickname;

        /// <summary>
        /// Backing field for <see cref="FormValidationStatus"/>
        /// </summary>
        private GenericStatus formValidationStatus;

        /// <summary>
        /// Backing field for <see cref="SpecificRouterVerificationError"/>
        /// </summary>
        private string specificRouterVerificationError;

        /// <summary>
        /// Backing field for <see cref="Loading"/>
        /// </summary>
        private bool loading;

        /// <summary>
        /// Initialises a new instance of the <see cref="RouterSetupViewModel"/> class
        /// </summary>
        /// <param name="networkService">An instance of <see cref="INetworkService"/></param>
        /// <param name="dialogService">An instance of <see cref="IDialogService"/></param>
        /// <param name="dataPersistenceService">An instance of <see cref="IDataPersistenceService"/></param>
        /// <param name="messengerService">An instance of <see cref="IMessengerService"/></param>
        public RouterSetupViewModel(
            INetworkService networkService,
            IDialogService dialogService,
            IDataPersistenceService dataPersistenceService,
            IMessengerService messengerService)
        {
            this.networkService = networkService;
            this.dialogService = dialogService;
            this.dataPersistenceService = dataPersistenceService;
            this.messengerService = messengerService;

            this.routerIpv4Address = string.Empty;
            this.sshPort = string.Empty;
            this.sshKeyFolder = string.Empty;
            this.userIsUsingSshKeys = false;
            this.sshUsername = string.Empty;
            this.sshPassword = string.Empty;
            this.routerNickname = string.Empty;

            this.formValidationStatus = GenericStatus.Warning;
            this.specificRouterVerificationError = string.Empty;

            this.loading = false;

            this.SshPort = NetworkService.DefaultSshPort;

            // set the default nickname
            List<Router> routers = this.dataPersistenceService.DeserialiseAllPresentRouters().ToList();
            this.DefaultRouterNickname = $"Router-{routers.Count + 1}";
            this.OnPropertyChanged(nameof(this.DefaultRouterNickname));

            // set up all commands
            this.BrowseForSshKeysCommand = new RelayCommand(this.BrowseForSshKeysCommandHandler);
            this.ScanForSshKeysCommand = new RelayCommand(this.ScanForSshKeysCommandHandler);
            this.VerifyRouterDetailsCommandAsync = new AsyncRelayCommand(this.VerifyRouterDetailsCommandHandlerAsync);
            this.ExitSetupCommandAsync = new AsyncRelayCommand(this.ExitSetupCommandHandlerAsync);

            this.messengerService.Send(new LogMessage("Entered router setup process"));
        }

        /// <summary>
        /// A <see cref="CustomValidationAttribute"/> validator for validating that a provided
        /// absolute folder path contains a pair of SSH keys. Being a <see cref="CustomValidationAttribute"/>
        /// allows this validator to resolve dependency injected services via the ViewModel instance's
        /// fields, allowing for non-static validation contexts.
        /// </summary>
        /// <param name="folder">The folder to scan for SSH keys</param>
        /// <param name="ctx">A <see cref="ValidationContext"/> instance</param>
        /// <returns>A <see cref="ValidationResult"/> instance</returns>
        public static ValidationResult ValidateSshKeysFolder(string folder, ValidationContext ctx)
        {
            INetworkService networkService = ((RouterSetupViewModel)ctx.ObjectInstance).networkService;

            return networkService.FolderContainsSshKeys(folder)
                ? ValidationResult.Success
                : new ValidationResult("Folder does not contain a pair of SSH keys");
        }

        /// <summary>
        /// Gets the command used when the user issues the action to manually
        /// browse their system for a folder containing a pair of SSH keys
        /// </summary>
        public ICommand BrowseForSshKeysCommand { get; }

        /// <summary>
        /// Gets the command used when the user issues the action to let NVRAMTuner
        /// automatically scan their system for a folder containing a pair of SSH keys
        /// </summary>
        public ICommand ScanForSshKeysCommand { get; }

        /// <summary>
        /// Gets the async command used when the user wants to verify the information they have provided to
        /// the NVRAMTuner setup page by attempting a connection to the router
        /// </summary>
        public IAsyncRelayCommand VerifyRouterDetailsCommandAsync { get; }

        /// <summary>
        /// Gets the async command used when the user wants to exit the setup process
        /// </summary>
        public IAsyncRelayCommand ExitSetupCommandAsync { get; }

        /// <summary>
        /// Gets or sets the IPv4 address of the router
        /// </summary>
        [Required(ErrorMessage = "You need to provide an IP address")]
        [ValidIpv4Address]
        public string RouterIpv4Address
        {
            get => this.routerIpv4Address;
            set
            {
                this.SetProperty(ref this.routerIpv4Address, value, true);
                this.FormValidationStatus = GenericStatus.Warning;
            }
        }

        /// <summary>
        /// Gets or sets the port on the router used to expose its SSH server
        /// </summary>
        [Required(ErrorMessage = "You need to provide a port")]
        [ValidNetworkPort]
        public string SshPort
        {
            get => this.sshPort;
            set
            {
                this.SetProperty(ref this.sshPort, value, true);
                this.FormValidationStatus = GenericStatus.Warning;
            }
        }

        /// <summary>
        /// Gets or sets the full path to a folder containing a private and public SSH key pair on the user's
        /// system
        /// </summary>
        [Required(ErrorMessage = "You must provide a folder containing your SSH keys")]
        [CustomValidation(typeof(RouterSetupViewModel), nameof(ValidateSshKeysFolder))]
        public string SshKeyFolder
        {
            get => this.sshKeyFolder;
            set
            {
                this.SetProperty(ref this.sshKeyFolder, value, true);
                this.FormValidationStatus = GenericStatus.Warning;
            }
        }

        /// <summary>
        /// Gets or sets a bool representing whether or not the user is using SSH keys to authenticate
        /// with their router
        /// </summary>
        public bool UserIsUsingSshKeys
        {
            get => this.userIsUsingSshKeys;
            set
            {
                this.SetProperty(ref this.userIsUsingSshKeys, value, true);
                this.FormValidationStatus = GenericStatus.Warning;
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="GenericStatus"/> enumeration to denote the status
        /// of the form's validation against the user's router
        /// </summary>
        public GenericStatus FormValidationStatus
        {
            get => this.formValidationStatus;
            set => this.SetProperty(ref this.formValidationStatus, value);
        }

        /// <summary>
        /// Gets or sets the username that will be used to authenticate with the router's SSH server 
        /// </summary>
        [Required(ErrorMessage = "You need to provide an SSH username")]
        [MaxLength(32, ErrorMessage = "Username cannot be longer than 32 characters")]
        public string SshUsername
        {
            get => this.sshUsername;
            set
            {
                this.SetProperty(ref this.sshUsername, value, true);
                this.FormValidationStatus = GenericStatus.Warning;
            }
        }

        /// <summary>
        /// Gets or sets the password that will be used to authenticate with the router's SSH server
        /// </summary>
        [Required(ErrorMessage = "You need to provide an SSH password")]
        public string SshPassword
        {
            get => this.sshPassword;
            set
            {
                this.SetProperty(ref this.sshPassword, value, true);
                Debug.WriteLine($"password: '{this.sshPassword}'");
                this.FormValidationStatus = GenericStatus.Warning;
            }
        }

        /// <summary>
        /// Gets or sets the nickname that will be given to this router. The role of this nickname
        /// is simply to be a more user-friendly name that can be used for quick recognition when more
        /// than 1 router has been saved
        /// </summary>
        [MaxLength(25, ErrorMessage = "Nickname has a max character limit of 25")]
        public string RouterNickname
        {
            get => this.routerNickname;
            set => this.SetProperty(ref this.routerNickname, value, true);
        }

        /// <summary>
        /// Gets the default router nickname that will be given to this router. This value is set if and
        /// when the user enters a valid Ipv4 address.
        /// </summary>
        public string DefaultRouterNickname { get; }

        /// <summary>
        /// Gets or sets a bool representing whether or not any loading is taking place in the background
        /// which should be interpreted as a sign to disable certain UI elements or block certain actions
        /// </summary>
        public bool Loading
        {
            get => this.loading;
            set => this.SetProperty(ref this.loading, value);
        }

        /// <summary>
        /// Gets or sets a string that provides feedback to the user on why their
        /// </summary>
        public string SpecificRouterVerificationError
        {
            get => this.specificRouterVerificationError;
            set => this.SetProperty(ref this.specificRouterVerificationError, value);
        }

        /// <summary>
        /// Constructs a new <see cref="Router"/> model instance from the current state of the setup form
        /// (only if there are no errors). This method also validates all fields in the form
        /// </summary>
        /// <returns>A new <see cref="Router"/> instance, or null if there are any errors in the form</returns>
        private Router? ConstructRouterFromForm()
        {
            if (!this.HasErrorsFiltered())
            {
                string nickname = string.IsNullOrWhiteSpace(this.RouterNickname) 
                    ? this.DefaultRouterNickname 
                    : this.RouterNickname;

                return new Router
                {
                    RouterIpv4Address = this.RouterIpv4Address,
                    RouterNickname = nickname,
                    RouterUid = Guid.NewGuid(),
                    SshPort = int.Parse(this.SshPort),
                    AuthType = this.UserIsUsingSshKeys ? SshAuthType.PubKeyBasedAuth : SshAuthType.PasswordBasedAuth,
                    SshUsername = this.SshUsername,
                    SshPassword = this.UserIsUsingSshKeys ? null : this.SshPassword,
                    SskKeyDir = this.UserIsUsingSshKeys ? this.SshKeyFolder : null
                };
            }

            this.ApplySpecificErrorPointingToFormErrors();
            return null;
        }

        /// <summary>
        /// Method for handling the <see cref="BrowseForSshKeysCommand"/>
        /// </summary>
        private void BrowseForSshKeysCommandHandler()
        {
            string path = this.dialogService.ShowFolderBrowserDialog("Select folder containing your SSH key pair", false);

            if (path == string.Empty)
            {
                return;
            }

            Task.Run(async () =>
            {
                // TODO: is there a way to avoid calling this method twice? - maybe move this dialog?
                if (this.networkService.FolderContainsSshKeys(path))
                {
                    await this.dialogService.ShowMessageAsync(
                        this,
                        "SSH keys located",
                        $"A pair of SSH keys have been located inside '{path}'!");
                }
            });

            this.SshKeyFolder = path;
        }

        /// <summary>
        /// Method for handling the <see cref="ScanForSshKeysCommand"/>
        /// </summary>
        private void ScanForSshKeysCommandHandler()
        {
            string? path = this.networkService.ScanLocalSystemForSshKeys();

            if (path == null)
            {
                Task.Run(async () =>
                {
                    await this.dialogService.ShowMessageAsync(
                        this,
                        "No keys found",
                        "The scan didn't turn up any directories. Please enter the path manually");
                });

                return;
            }

            if (path == this.SshKeyFolder)
            {
                return;
            }

            Task.Run(async () =>
            {
                await this.dialogService.ShowMessageAsync(
                    this,
                    "Keys found!",
                    $"SSH keys found in '{path}'. It has been auto filled");
            });

            this.SshKeyFolder = path;
        }

        /// <summary>
        /// Asynchronous handler for the <see cref="VerifyRouterDetailsCommandAsync"/> command
        /// </summary>
        /// <returns>An asynchronous <see cref="Task"/></returns>
        private async Task VerifyRouterDetailsCommandHandlerAsync()
        {
            this.Loading = true;

            Router? router = this.ConstructRouterFromForm();

            if (router != null)
            {
                try
                {
                    SshConnectionInfo connectionInfo = await this.networkService.ConnectToRouterAsync(router);

                    if (connectionInfo.ConnectionSuccessful)
                    {
                        this.FormValidationStatus = GenericStatus.Success;

                        MessageDialogResult verifySuccessChoice = await this.dialogService.ShowMessageAsync(
                            this,
                            "Connection success!",
                            $"NVRAMTuner is able to connect to {connectionInfo.HostName} ({connectionInfo.OperatingSystem}).\n" +
                            "Do you want to save this router and complete the setup process, or go back to alter any " +
                            "details?",
                            MessageDialogStyle.AffirmativeAndNegative,
                            new MetroDialogSettings()
                            {
                                AffirmativeButtonText = "Complete setup",
                                NegativeButtonText = "Go back",
                                DefaultButtonFocus = MessageDialogResult.Affirmative
                            });

                        if (verifySuccessChoice == MessageDialogResult.Affirmative)
                        {
                            await this.dataPersistenceService.SerialiseRouterToEncryptedFileAsync(router);
                            this.messengerService.Send(
                                new NavigationRequestMessage(NavigableViewModel.HomeViewModel));
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                    this.FormValidationStatus = GenericStatus.Failure;
                    this.SpecificRouterVerificationError = e.Message;

                    await this.dialogService.ShowMessageAsync(
                        this,
                        "Connection failure",
                        $"An attempt was made to connect to your router at {router.RouterIpv4Address}, " +
                        $"but an error occurred:\n\n{e.Message}");
                    
                }
            }

            this.Loading = false;
        }

        /// <summary>
        /// Method for handling the <see cref="ExitSetupCommandAsync"/> command
        /// </summary>
        private async Task ExitSetupCommandHandlerAsync()
        {
            MessageDialogResult userExitChoice = await this.dialogService.ShowMessageAsync(
                this,
                "Exit?",
                "Are you sure you wish to exit? This will return you to the home page and all current form progress will be lost",
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = "Yes, exit setup",
                    DefaultButtonFocus = MessageDialogResult.Affirmative,
                    NegativeButtonText = "No, stay on setup page"
                });

            if (userExitChoice == MessageDialogResult.Affirmative)
            {
                this.messengerService.Send(new LogMessage("Exited router setup process"));
                this.messengerService.Send(new NavigationRequestMessage(NavigableViewModel.HomeViewModel));
            }
        }

        /// <summary>
        /// Sets an overall form error message directing the user to the values they have entered
        /// into the actual form itself, as they are still throwing validation errors.
        /// </summary>
        private void ApplySpecificErrorPointingToFormErrors()
        {
            this.FormValidationStatus = GenericStatus.Failure;
            this.SpecificRouterVerificationError = ViewModelStrings.RouterSetupOverallFormErrorsMessage;
        }

        /// <summary>
        /// Sets an overall form error message to the user that can have its content dynamically set
        /// </summary>
        /// <param name="message">The specific error message to display to the user</param>
        private void ApplySpecificErrorWithMessage(string message)
        {
            this.FormValidationStatus = GenericStatus.Failure;
            this.SpecificRouterVerificationError = message;
        }

        /// <summary>
        /// Analogous to the <see cref="ObservableValidator.HasErrors"/> property, except applies the custom filtering
        /// of the <see cref="GetFilteredErrors"/> method
        /// </summary>
        /// <returns>A bool representing whether or not the form has any errors. This bool takes into account
        /// the filtering applied to the underlying <see cref="ObservableValidator"/>s list of errors carried out
        /// by the <see cref="GetFilteredErrors"/> method</returns>
        private bool HasErrorsFiltered()
        {
            return this.GetFilteredErrors().Any();
        }

        /// <summary>
        /// Analogous to the <see cref="ObservableValidator.GetErrors"/> method, except with some custom filtering on
        /// top of the returned <see cref="IEnumerable{T}"/> object. In this case, we want to filter out some specific
        /// errors based on the state of the <see cref="UserIsUsingSshKeys"/> property. As either the SSH password or
        /// the SSH key folder text box will be hidden based on the state of <see cref="UserIsUsingSshKeys"/>, we
        /// want to dynamically filter out errors attached to the bound properties, as if the box for either one
        /// is hidden from the user, its value will not be used and is therefore immaterial to the actual
        /// error state of the ViewModel
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ValidationResult"/> instances</returns>
        private IEnumerable<ValidationResult> GetFilteredErrors()
        {
            this.ValidateAllProperties();

            return this.UserIsUsingSshKeys
                ? this.GetErrors().Where(vr => vr.MemberNames.Any(mn => !mn.Equals(nameof(this.SshPassword))))
                : this.GetErrors().Where(vr => vr.MemberNames.Any(mn => !mn.Equals(nameof(this.SshKeyFolder))));
        }
    }
}