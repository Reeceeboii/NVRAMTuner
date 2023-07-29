namespace NVRAMTuner.Client.ViewModels.Variables
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using CommunityToolkit.Mvvm.Messaging;
    using MahApps.Metro.Controls.Dialogs;
    using Messages;
    using Messages.Variables;
    using Messages.Variables.Staged;
    using Models.Nvram;
    using Models.Nvram.Concrete;
    using Resources;
    using Services.Interfaces;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// ViewModel for the variables view
    /// </summary>
    public class VariablesViewModel : ObservableRecipient
    {
        /// <summary>
        /// Instance of <see cref="IVariableService"/>
        /// </summary>
        private readonly IVariableService variableService;

        /// <summary>
        /// Instance of <see cref="IDialogService"/>
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// Backing field for <see cref="Variables"/>
        /// </summary>
        private ObservableCollection<IVariable> variables;

        /// <summary>
        /// Backing field for <see cref="SelectedVariable"/>
        /// </summary>
        private IVariable selectedVariable;

        /// <summary>
        /// Backing field for <see cref="TotalSizeBytes"/>
        /// </summary>
        private int totalSizeBytes;

        /// <summary>
        /// Backing field for <see cref="RemainingSizeBytes"/>
        /// </summary>
        private int remainingSizeBytes;

        /// <summary>
        /// Backing field for <see cref="VariableSizeBytes"/>
        /// </summary>
        private int variableSizeBytes;

        /// <summary>
        /// Initialises a new instance of the <see cref="VariablesViewModel"/> class
        /// </summary>
        /// <param name="variableService">An instance of <see cref="IVariableService"/></param>
        /// <param name="dialogService">An instance of <see cref="IDialogService"/></param>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        public VariablesViewModel(
            IVariableService variableService, 
            IDialogService dialogService,
            IMessenger messenger) : base(messenger)
        {
            this.variableService = variableService;
            this.dialogService = dialogService;

            this.Variables = new ObservableCollection<IVariable>();
            this.IsActive = true;

            this.RefreshVariablesCommand = new AsyncRelayCommand(this.RefreshVariablesCommandHandlerAsync);
        }

        /// <summary>
        /// Gets the asynchronous command used to refresh the current set of variables
        /// </summary>
        public IAsyncRelayCommand RefreshVariablesCommand { get; }

        /// <summary>
        /// Override of <see cref="ObservableRecipient.OnActivated"/>.
        /// Handles message registration
        /// </summary>
        protected override void OnActivated()
        { 
            this.Messenger.Register<VariablesViewModel, VariablesChangedMessage>(
                this, (recipient, message) => this.Receive(message));

            this.Messenger.Register<VariablesViewModel, RouterDisconnectMessage>(
                this, (recipient, message) => this.Receive(message));

            this.Messenger.Register<VariablesViewModel, VariableStagedMessage>(
                this, (recipient, message) => this.Receive(message));

            this.Messenger.Register<VariablesViewModel, VariablesUnstagedMessage>(
                this, (recipient, message) => this.Receive(message));
        }

        /// <summary>
        /// Gets or sets an <see cref="ObservableCollection{T}"/> of <see cref="NvramVariable"/> instances
        /// </summary>
        public ObservableCollection<IVariable> Variables
        {
            get => this.variables;
            set => this.SetProperty(ref this.variables, value);
        }

        /// <summary>
        /// Gets or privately sets the total number of bytes occupied by the router's current
        /// NVRAM configuration
        /// </summary>
        public int TotalSizeBytes
        {
            get => this.totalSizeBytes;
            private set => this.SetProperty(ref this.totalSizeBytes, value);
        }

        /// <summary>
        /// Gets or privately sets the total number of bytes remaining available for new
        /// NVRAM data on the router
        /// </summary>
        public int RemainingSizeBytes
        {
            get => this.remainingSizeBytes;
            private set => this.SetProperty(ref this.remainingSizeBytes, value);
        }

        /// <summary>
        /// Gets or privately sets the total size of all the variables in bytes.
        /// This will be smaller than the total current size of the NVRAM contents
        /// </summary>
        public int VariableSizeBytes
        {
            get => this.variableSizeBytes;
            private set => this.SetProperty(ref this.variableSizeBytes, value);
        }

        /// <summary>
        /// Gets or sets the currently selected variable in the datagrid
        /// </summary>
        public IVariable SelectedVariable
        {
            get => this.selectedVariable;
            set
            {
                this.SetProperty(ref this.selectedVariable, value);
                this.Messenger.Send(new VariableSelectedMessage(value));
            }
        }

        /// <summary>
        /// Recipient method for <see cref="VariablesChangedMessage"/> messages
        /// </summary>
        /// <param name="message">An instance of <see cref="VariablesChangedMessage"/></param>
        public void Receive(VariablesChangedMessage message)
        {
            this.Variables = new ObservableCollection<IVariable>(message.Value.Variables);

            // initially select the first variable in the list
            if (this.Variables.Any())
            {
                this.SelectedVariable = this.Variables[0];
            }

            this.TotalSizeBytes = message.Value.TotalSizeBytes;
            this.RemainingSizeBytes = message.Value.RemainingSizeBytes;
            this.VariableSizeBytes = message.Value.VariableSizeBytes;
        }

        /// <summary>
        /// Recipient method for <see cref="RouterDisconnectMessage"/> messages
        /// </summary>
        /// <param name="_">An instance of <see cref="RouterDisconnectMessage"/></param>
        public void Receive(RouterDisconnectMessage _)
        {
            this.Variables.Clear();
        }

        /// <summary>
        /// Recipient method for <see cref="VariableStagedMessage"/> messages.
        /// This method has absolutely abysmal performance but I don't want come up with a more
        /// elegant solution right now. And plus my CPU is so fast I'm biased towards this
        /// not actually mattering at all. HOWEVER: TODO - performance
        /// </summary>
        /// <param name="_">An instance of <see cref="VariableStagedMessage"/></param>
        public void Receive(VariableStagedMessage _)
        {
            int indexToRemove = this.Variables.IndexOf(this.SelectedVariable);
            this.Variables.RemoveAt(indexToRemove);

            if (!this.Variables.Any())
            {
                throw new InvalidOperationException(
                    "How on earth (and *why*) have you staged every variable? " +
                    "I am crashing to teach you a lesson");
            }

            this.SelectedVariable = null;
        }

        /// <summary>
        /// Method to receive <see cref="VariablesUnstagedMessage"/> messages
        /// </summary>
        /// <param name="message">An instance of <see cref="VariablesUnstagedMessage"/></param>
        public void Receive(VariablesUnstagedMessage message)
        {
            foreach (IVariable delta in message.Value)
            {
                // TODO
                //this.Variables.Add(message.KeepChanges ? delta.Original : delta.Delta);
            }
        }

        /// <summary>
        /// Method to handle the <see cref="RefreshVariablesCommand"/>
        /// </summary>
        /// <returns>An asynchronous <see cref="Task"/></returns>
        private async Task RefreshVariablesCommandHandlerAsync()
        {
            // if there are any staged variables, warn the user that they will be abandoned post-refresh
            RequestNumOfStagedVariablesMessage reqMsg = this.Messenger.Send<RequestNumOfStagedVariablesMessage>();
            if (reqMsg.Response != 0)
            {
                MessageDialogResult refreshConfirmation = await this.dialogService.ShowMessageAsync(
                    this,
                    "Abandon staged changes?",
                    ViewModelStrings.RefreshWhileVariablesAreStagedDialogMessage,
                    MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings
                    {
                        DefaultButtonFocus = MessageDialogResult.Affirmative,
                        AffirmativeButtonText = "Yes, refresh", 
                        NegativeButtonText = "Cancel"
                    });

                if (refreshConfirmation == MessageDialogResult.Affirmative)
                {
                    this.Messenger.Send(new ClearStagedVariablesMessage());
                }
                else
                {
                    return;
                }
            }

            IVariable previouslySelected = this.SelectedVariable;
            await this.variableService.GetNvramVariablesAsync();

            // attempt to select the previously selected variable
            if (previouslySelected != null)
            {
                this.SelectedVariable = this.Variables.First(v => v.Name == previouslySelected.Name);
            }
        }
    }
}