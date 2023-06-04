namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Services;
    using Services.Interfaces;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Validators;

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
        /// The Ipv4 address of the router being targeted in this setup
        /// </summary>
        private string routerIpv4Address;

        /// <summary>
        /// The network port the target router uses to expose its SSH server
        /// </summary>
        private string sshPort;

        /// <summary>
        /// The full path to a folder containing a private and public SSH key pair on the user's
        /// system
        /// </summary>
        private string sshKeyFolder;

        /// <summary>
        /// A bool representing whether or not the user has decided that this setup
        /// will make use of SSH keys instead of a password
        /// </summary>
        private bool userIsUsingSshKeys;

        /// <summary>
        /// The username that will be used to authenticate with the router's SSH server
        /// </summary>
        private string sshUsername;

        /// <summary>
        /// The password that will be used to authenticate with the router's SSH server.
        /// This value will only be utilised if <see cref="userIsUsingSshKeys"/> is false.
        /// </summary>
        private string sshPassword;

        /// <summary>
        /// Initialises a new instance of the <see cref="RouterSetupViewModel"/> class
        /// </summary>
        /// <param name="networkService">An instance of <see cref="INetworkService"/></param>
        /// <param name="dialogService">An instance of <see cref="IDialogService"/></param>
        public RouterSetupViewModel(INetworkService networkService, IDialogService dialogService)
        {
            this.networkService = networkService;
            this.dialogService = dialogService;
            this.SshPort = NetworkService.DefaultSshPort;

            this.BrowseForSshKeysCommand = new RelayCommand(this.BrowseForSshKeysCommandHandler);
        }

        /// <summary>
        /// A <see cref="CustomValidationAttribute"/> validator for validating that a provided
        /// absolute folder path contains a pair of SSH keys. Being a <see cref="CustomValidationAttribute"/>
        /// allows this validator to resolve dependency injected services via the ViewModel instance's
        /// fields, allowing non-static validation contexts.
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
        /// Gets or sets the IPv4 address of the router.
        /// Uses the underlying property of <see cref="router"/>
        /// </summary>
        [Required(ErrorMessage = "You need to provide an IP address")]
        [ValidIpv4Address]
        public string RouterIpv4Address
        {
            get => this.routerIpv4Address;
            set => this.SetProperty(ref this.routerIpv4Address, value, true);
        }

        /// <summary>
        /// Gets or sets the port on the router used to expose its SSH server.
        /// Uses the underlying property of <see cref="router"/>
        /// </summary>
        [Required(ErrorMessage = "You need to provide a port")]
        [ValidNetworkPort]
        public string SshPort
        {
            get => this.sshPort;
            set => this.SetProperty(ref this.sshPort, value, true);
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
            set => this.SetProperty(ref this.sshKeyFolder, value, true);
        }

        /// <summary>
        /// Gets or sets a bool representing whether or not the user is using SSH keys to authenticate
        /// with their router
        /// </summary>
        [Required]
        public bool UserIsUsingSshKeys
        {
            get => this.userIsUsingSshKeys;
            set
            {
                this.SetProperty(ref this.userIsUsingSshKeys, value, true);
                this.OnPropertyChanged(nameof(this.ShowSshPasswordTextBox));
            }
        }

        /// <summary>
        /// Gets or sets the username that will be used to authenticate with the router's SSH server 
        /// </summary>
        [Required(ErrorMessage = "You need to provide an SSH username")]
        [MaxLength(32, ErrorMessage = "Username cannot be longer than 32 characters")]
        public string SshUsername
        {
            get => this.sshUsername;
            set => this.SetProperty(ref this.sshUsername, value, true);
        }

        /// <summary>
        /// Gets or sets the password that will be used to authenticate with the router's SSH server.
        /// </summary>
        [Required(ErrorMessage = "You need to provide an SSH password")]
        public string SshPassword
        {
            get => this.sshPassword;
            set => this.SetProperty(ref this.sshPassword, value, true);
        }

        /// <summary>
        /// Gets a bool representing whether the SSH password text box should be shown.
        /// This is false if the user has selected to use SSH keys for their authentication
        /// method
        /// </summary>
        public bool ShowSshPasswordTextBox => !this.UserIsUsingSshKeys;

        /// <summary>
        /// Method for handing the <see cref="BrowseForSshKeysCommand"/>
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
                await this.dialogService.ShowMessageAsync(
                    this,
                    "SSH keys located",
                    $"A pair of SSH keys have been located inside '{path}'!");
            });

            this.SshKeyFolder = path;
        }
    }
}