namespace NVRAMTuner.Client.ViewModels.Variables
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using MahApps.Metro.Controls.Dialogs;
    using Messages;
    using Messages.Variables;
    using Messages.Variables.Staged;
    using Models.Nvram;
    using Resources;
    using Services.Interfaces;
    using Services.Wrappers.Interfaces;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// ViewModel for the staged changes view
    /// </summary>
    public class StagedChangesViewModel : ObservableObject
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
        /// Instance of <see cref="INetworkService"/>
        /// </summary>
        private readonly INetworkService networkService;

        /// <summary>
        /// Instance of <see cref="ISettingsService"/>
        /// </summary>
        private readonly ISettingsService settingsService;

        /// <summary>
        /// Backing field for <see cref="VariableDeltas"/>
        /// </summary>
        private ObservableCollection<IVariable> variableDeltas;

        /// <summary>
        /// Backing field for <see cref="SelectedDelta"/>
        /// </summary>
        private IVariable selectedDelta;

        /// <summary>
        /// Backing field for <see cref="OriginalSizeBytes"/>
        /// </summary>
        private int originalSizeBytes;

        /// <summary>
        /// Backing field for <see cref="NetChangeSizeBytes"/>
        /// </summary>
        private int netChangeSizeBytes;

        /// <summary>
        /// Backing field for <see cref="SizeAfterCommitBytes"/>
        /// </summary>
        private int sizeAfterCommitBytes;

        /// <summary>
        /// Initialises a new instance of the <see cref="StagedChangesViewModel"/> class
        /// </summary>
        /// <param name="messengerService">An instance of <see cref="IMessengerService"/></param>
        /// <param name="dialogService">An instance of <see cref="IDialogService"/></param>
        /// <param name="networkService">An instance of <see cref="INetworkService"/></param>
        /// <param name="settingsService">An instance of <see cref="ISettingsService"/></param>
        public StagedChangesViewModel(
            IMessengerService messengerService,
            IDialogService dialogService,
            INetworkService networkService,
            ISettingsService settingsService)
        {
            this.messengerService = messengerService;
            this.dialogService = dialogService;
            this.networkService = networkService;
            this.settingsService = settingsService;

            this.ClearStagedDeltasCommand = new AsyncRelayCommand(this.ClearStagedDeltasCommandHandlerAsync, 
                () => this.VariableDeltas.Any());

            this.CommitStagedDeltasCommand = new AsyncRelayCommand(this.CommitStagedDeltasCommandHandlerAsync,
                () => this.VariableDeltas.Any());

            this.VariableDeltas = new ObservableCollection<IVariable>();

            // register messages
            this.messengerService.Register<StagedChangesViewModel, VariableStagedMessage>(this, (r, m) => r.Receive(m));
            this.messengerService.Register<StagedChangesViewModel, RequestSelectedStagedVariableMessage>(this, (r, m) => r.Receive(m));
            this.messengerService.Register<StagedChangesViewModel, ClearStagedVariablesMessage>(this, (r, m) => r.Receive(m));
            this.messengerService.Register<StagedChangesViewModel, RequestNumOfStagedVariablesMessage>(this, (r, m) => r.Receive(m));
            this.messengerService.Register<StagedChangesViewModel, VariablesChangedMessage>(this, (r, m) => r.Receive(m));
        }

        /// <summary>
        /// Gets the command used to clear all of the currently staged deltas
        /// </summary>
        public IAsyncRelayCommand ClearStagedDeltasCommand { get; }

        /// <summary>
        /// Gets the command used to commit the currently staged deltas to the router
        /// </summary>
        public IAsyncRelayCommand CommitStagedDeltasCommand { get; }

        /// <summary>
        /// Gets or sets an <see cref="ObservableCollection{T}"/> of <see cref="IVariable"/> instances,
        /// representing all of the currently staged changes
        /// </summary>
        public ObservableCollection<IVariable> VariableDeltas
        {
            get => this.variableDeltas;
            set => this.SetProperty(ref this.variableDeltas, value);
        }

        /// <summary>
        /// The currently selected delta from the list of staged variables
        /// </summary>
        public IVariable SelectedDelta
        {
            get => this.selectedDelta;
            set => this.SetProperty(ref this.selectedDelta, value);
        }

        /// <summary>
        /// Gets or sets the original size of all the NVRAM variables, in bytes
        /// </summary>
        public int OriginalSizeBytes
        {
            get => this.originalSizeBytes;
            private set => this.SetProperty(ref this.originalSizeBytes, value);
        }

        /// <summary>
        /// Gets or sets the current net change to <see cref="OriginalSizeBytes"/> taking into account the
        /// changes applied via all current staged variables
        /// </summary>
        public int NetChangeSizeBytes
        {
            get => this.netChangeSizeBytes;
            private set => this.SetProperty(ref this.netChangeSizeBytes, value);
        }

        /// <summary>
        /// Gets or sets the expected size occupied in NVRAM by the variables
        /// after all the staged changes are committed
        /// </summary>
        public int SizeAfterCommitBytes
        {
            get => this.sizeAfterCommitBytes;
            private set => this.SetProperty(ref this.sizeAfterCommitBytes, value);
        }

        /// <summary>
        /// Recipient method for the <see cref="VariableStagedMessage"/> message
        /// </summary>
        /// <param name="message">An instance of <see cref="VariableStagedMessage"/></param>
        private void Receive(VariableStagedMessage message)
        {
            this.VariableDeltas.Add(message.Value);
            this.NetChangeSizeBytes += message.Value.ValueDelta.Length - message.Value.OriginalValue.Length;
            this.SizeAfterCommitBytes += this.NetChangeSizeBytes;
            this.ClearStagedDeltasCommand.NotifyCanExecuteChanged();
            this.CommitStagedDeltasCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Recipient method for the <see cref="RequestSelectedStagedVariableMessage"/> message
        /// </summary>
        /// <param name="message">An instance of <see cref="RequestSelectedStagedVariableMessage"/></param>
        private void Receive(RequestSelectedStagedVariableMessage message)
        {
            message.Reply(this.SelectedDelta);
        }

        /// <summary>
        /// Recipient method for the <see cref="RequestNumOfStagedVariablesMessage"/> request message
        /// </summary>
        /// <param name="message">An instance of <see cref="RequestNumOfStagedVariablesMessage"/></param>
        private void Receive(RequestNumOfStagedVariablesMessage message)
        {
            message.Reply(this.VariableDeltas.Count);
        }

        /// <summary>
        /// Recipient method for the <see cref="ClearStagedVariablesMessage"/> message.
        /// Clears the list of deltas upon receiving this message
        /// </summary>
        /// <param name="_">An instance of <see cref="ClearStagedVariablesMessage"/></param>
        private void Receive(ClearStagedVariablesMessage _)
        {
            this.VariableDeltas.Clear();
            this.NetChangeSizeBytes = 0;
            this.ClearStagedDeltasCommand.NotifyCanExecuteChanged();
            this.CommitStagedDeltasCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Recipient method for the <see cref="VariablesChangedMessage"/> message
        /// </summary>
        /// <param name="message">An instance of <see cref="VariablesChangedMessage"/></param>
        private void Receive(VariablesChangedMessage message)
        {
            this.OriginalSizeBytes = message.Value.VariableSizeBytes;
            this.SizeAfterCommitBytes = message.Value.VariableSizeBytes;
        }

        /// <summary>
        /// Handler method for the <see cref="CommitStagedDeltasCommand"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private async Task CommitStagedDeltasCommandHandlerAsync()
        {
            if (this.settingsService.DisplayPreCommitWarning)
            {
                MessageDialogResult commitWarningDialogResult = await this.dialogService.ShowMessageAsync(
                    this,
                    "Warning!",
                    ViewModelStrings.CommitWarningDialogMessage,
                    MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                    new MetroDialogSettings
                    {
                        AffirmativeButtonText = "Continue, don't warn me again",
                        NegativeButtonText = "Go back",
                        FirstAuxiliaryButtonText = "Continue, warn me again next time",
                        DefaultButtonFocus = MessageDialogResult.Affirmative
                    });

                if (commitWarningDialogResult == MessageDialogResult.Negative)
                {
                    return;
                }

                if (commitWarningDialogResult == MessageDialogResult.Affirmative)
                {
                    this.settingsService.DisplayPreCommitWarning = false;
                }
            }

            await this.networkService.CommitChangesToRouterAsync(this.VariableDeltas.ToList());
        }

        /// <summary>
        /// Handler method for the <see cref="ClearStagedDeltasCommand"/>
        /// </summary>
        private async Task ClearStagedDeltasCommandHandlerAsync()
        {
            int staged = this.VariableDeltas.Count;

            MessageDialogResult unstageDialogResult = await this.dialogService.ShowMessageAsync(
                this,
                $"Clear {staged} staged variable{(staged > 1 ? "s" : string.Empty)}?",
                ViewModelStrings.ClearStagedChangesDialogMessage,
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = "Unstage & keep changes",
                    NegativeButtonText = "Unstaged & abandon changes",
                    FirstAuxiliaryButtonText = "Cancel",
                    DefaultButtonFocus = MessageDialogResult.Affirmative
                });

            // 'cancel'
            if (unstageDialogResult == MessageDialogResult.FirstAuxiliary)
            {
                return;
            }

            bool keepChanges = unstageDialogResult == MessageDialogResult.Affirmative;
            this.messengerService.Send(new VariablesUnstagedMessage(this.VariableDeltas.ToList(), keepChanges));

            this.VariableDeltas.Clear();
            this.NetChangeSizeBytes = 0;
            this.ClearStagedDeltasCommand.NotifyCanExecuteChanged();
            this.CommitStagedDeltasCommand.NotifyCanExecuteChanged();

            this.messengerService.Send(new LogMessage($"{staged} variable{(staged > 1 ? "s" : string.Empty)} unstaged"));
        }
    }
}
