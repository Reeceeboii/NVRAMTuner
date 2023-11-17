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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO.Abstractions;
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
        /// Instance of <see cref="IFileSystem"/>
        /// </summary>
        private readonly IFileSystem fileSystem;

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
        /// <param name="fileSystem">An instance of <see cref="IFileSystem"/></param>
        public StagedChangesViewModel(
            IMessengerService messengerService,
            IDialogService dialogService,
            INetworkService networkService,
            ISettingsService settingsService,
            IFileSystem fileSystem)
        {
            this.messengerService = messengerService;
            this.dialogService = dialogService;
            this.networkService = networkService;
            this.settingsService = settingsService;
            this.fileSystem = fileSystem;

            this.ClearStagedDeltasCommand = new AsyncRelayCommand(
                this.ClearStagedDeltasCommandHandlerAsync, 
                () => this.VariableDeltas.Any());

            this.CommitStagedDeltasCommand = new AsyncRelayCommand(
                this.CommitStagedDeltasCommandHandlerAsync,
                () => this.VariableDeltas.Any());

            this.ClearSingleStagedDeltaCommand = new AsyncRelayCommand(
                this.ClearSingleStagedDeltaCommandHandlerAsync,
                () => this.SelectedDelta != null);

            this.SaveDeltasToScriptFileCommand = new RelayCommand(
                this.SaveDeltasToScriptFileCommandHandler,
                () => this.VariableDeltas.Any());

            this.VariableDeltas = new ObservableCollection<IVariable>();
            this.VariableDeltas.CollectionChanged += (sender, args) => this.NotifyCommandsBasedOnPresentDeltas();

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
        /// Gets the command to clear a single staged delta (this is the selected variable, as
        /// the single unstage option can only be accessed via a click to get to the context
        /// menu
        /// </summary>
        public IAsyncRelayCommand ClearSingleStagedDeltaCommand { get; }

        /// <summary>
        /// Gets the command used to commit the currently staged deltas to the router
        /// </summary>
        public IAsyncRelayCommand CommitStagedDeltasCommand { get; }

        /// <summary>
        /// Gets the command used to save the current staged deltas to a shell script file
        /// </summary>
        public IRelayCommand SaveDeltasToScriptFileCommand { get; }

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
            this.ClearDeltas();
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
        /// <returns>An asynchronous <see cref="Task"/></returns>
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
        /// <returns>An asynchronous <see cref="Task{TResult}"/></returns>
        private async Task ClearStagedDeltasCommandHandlerAsync()
        {
            int staged = this.VariableDeltas.Count;

            MessageDialogResult unstageDialogResult = await this.ConfirmStagedDeltaClearDialog(false);

            // 'cancel'
            if (unstageDialogResult == MessageDialogResult.FirstAuxiliary)
            {
                return;
            }

            bool keepChanges = unstageDialogResult == MessageDialogResult.Affirmative;
            this.messengerService.Send(new VariablesUnstagedMessage(this.VariableDeltas.ToList(), keepChanges));

            this.ClearDeltas();

            this.messengerService.Send(new LogMessage($"{staged} variable{(staged > 1 ? "s" : string.Empty)} unstaged"));
        }

        /// <summary>
        /// Handler method for the <see cref="ClearSingleStagedDeltaCommand"/>
        /// </summary>
        /// <returns>An asynchronous <see cref="Task{TResult}"/></returns>
        private async Task ClearSingleStagedDeltaCommandHandlerAsync()
        {
            MessageDialogResult unstageDialogResult = await this.ConfirmStagedDeltaClearDialog(false);

            // 'cancel'
            if (unstageDialogResult == MessageDialogResult.FirstAuxiliary)
            {
                return;
            }

            bool keepChanges = unstageDialogResult == MessageDialogResult.Affirmative;
            this.messengerService.Send(new VariablesUnstagedMessage(new List<IVariable> { this.SelectedDelta }, keepChanges));

            this.ClearDeltas();

            this.messengerService.Send(new LogMessage($"Unstaged '{this.SelectedDelta.Name}'"));
        }

        /// <summary>
        /// Handler method for the <see cref="SaveDeltasToScriptFileCommand"/>
        /// </summary>
        private void SaveDeltasToScriptFileCommandHandler()
        {
            string path = this.dialogService.ShowSaveAsDialog(
                "Shell files (*.sh)|*.sh|All files (*.*)|*.*",
                "NVRAMTuner-Script.sh");

            if (path == string.Empty)
            {
                return;
            }

            this.fileSystem.File.WriteAllText(path, this.networkService.BuildShellScriptFile(this.VariableDeltas.ToList()));
        }

        /// <summary>
        /// Display a dialog asking the user if they sure they wish to unstage the changes, and
        /// whether or not they want to unstage AND abandon their current changes, or to unstage
        /// but keep the changes applied upon returning the variables to the main list
        /// </summary>
        /// <param name="singleDelta">A bool representing whether or not a single delta is being unstaged</param>
        /// <returns>A <see cref="MessageDialogResult"/> wrapped in an asynchronous <see cref="Task{T}"/></returns>
        private async Task<MessageDialogResult> ConfirmStagedDeltaClearDialog(bool singleDelta)
        {
            return await this.dialogService.ShowMessageAsync(
                this,
                $"Clear staged variable{(singleDelta ? string.Empty : "s")}?",
                ViewModelStrings.ClearStagedChangesDialogMessage,
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = "Unstage & keep changes",
                    NegativeButtonText = "Unstaged & abandon changes",
                    FirstAuxiliaryButtonText = "Cancel",
                    DefaultButtonFocus = MessageDialogResult.Affirmative
                });
        }

        /// <summary>
        /// Clears the staged changes/deltas and notifies relevant commands
        /// </summary>
        private void ClearDeltas()
        {
            this.VariableDeltas.Clear();
            this.NetChangeSizeBytes = 0;
        }

        /// <summary>
        /// Notify any commands that rely on the presence of staged variables/deltas to update
        /// their CanExecute statuses
        /// </summary>
        private void NotifyCommandsBasedOnPresentDeltas()
        {
            this.ClearStagedDeltasCommand.NotifyCanExecuteChanged();
            this.CommitStagedDeltasCommand.NotifyCanExecuteChanged();
            this.SaveDeltasToScriptFileCommand.NotifyCanExecuteChanged();
        }
    }
}
