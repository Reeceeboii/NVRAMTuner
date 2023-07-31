namespace NVRAMTuner.Client.ViewModels.Variables
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using MahApps.Metro.Controls.Dialogs;
    using Messages;
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
        /// Backing field for <see cref="VariableDeltas"/>
        /// </summary>
        private ObservableCollection<IVariable> variableDeltas;

        /// <summary>
        /// Backing field for <see cref="SelectedDelta"/>
        /// </summary>
        private IVariable selectedDelta;

        /// <summary>
        /// Initialises a new instance of the <see cref="StagedChangesViewModel"/> class
        /// </summary>
        /// <param name="messengerService">An instance of <see cref="IMessengerService"/></param>
        /// <param name="dialogService">An instance of <see cref="IDialogService"/></param>
        public StagedChangesViewModel(IMessengerService messengerService, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            this.messengerService = messengerService;

            this.ClearStagedDeltasCommand = new AsyncRelayCommand(this.ClearStagedDeltasCommandHandlerAsync, 
                () => this.VariableDeltas.Any());

            this.VariableDeltas = new ObservableCollection<IVariable>();

            // register messages
            this.messengerService.Register<StagedChangesViewModel, VariableStagedMessage>(
                this, (recipient, message) => recipient.Receive(message));

            this.messengerService.Register<StagedChangesViewModel, RequestSelectedStagedVariableMessage>(
                this, (recipient, message) => this.Receive(message));

            this.messengerService.Register<StagedChangesViewModel, ClearStagedVariablesMessage>(
                this, (recipient, message) => this.Receive(message));

            this.messengerService.Register<StagedChangesViewModel, RequestNumOfStagedVariablesMessage>(
                this, (recipient, message) => this.Receive(message));
        }

        /// <summary>
        /// Gets the command used to clear all of the currently staged deltas
        /// </summary>
        public IAsyncRelayCommand ClearStagedDeltasCommand { get; }

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
        /// Recipient method for the <see cref="VariableStagedMessage"/> message
        /// </summary>
        /// <param name="message">An instance of <see cref="VariableStagedMessage"/></param>
        public void Receive(VariableStagedMessage message)
        {
            this.VariableDeltas.Add(message.Value);
            this.ClearStagedDeltasCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Recipient method for the <see cref="RequestSelectedStagedVariableMessage"/> message
        /// </summary>
        /// <param name="message">An instance of <see cref="RequestSelectedStagedVariableMessage"/></param>
        public void Receive(RequestSelectedStagedVariableMessage message)
        {
            message.Reply(this.SelectedDelta);
        }

        /// <summary>
        /// Recipient method for the <see cref="RequestNumOfStagedVariablesMessage"/> request message
        /// </summary>
        /// <param name="message">An instance of <see cref="RequestNumOfStagedVariablesMessage"/></param>
        public void Receive(RequestNumOfStagedVariablesMessage message)
        {
            message.Reply(this.VariableDeltas.Count);
        }

        /// <summary>
        /// Recipient method for the <see cref="ClearStagedVariablesMessage"/> message.
        /// Clears the list of deltas upon receiving this message
        /// </summary>
        /// <param name="_">An instance of <see cref="ClearStagedVariablesMessage"/></param>
        public void Receive(ClearStagedVariablesMessage _)
        {
            this.VariableDeltas.Clear();
            this.ClearStagedDeltasCommand.NotifyCanExecuteChanged();
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

            if (unstageDialogResult == MessageDialogResult.FirstAuxiliary)
            {
                return;
            }

            bool keepChanges = unstageDialogResult == MessageDialogResult.Affirmative;
            this.messengerService.Send(new VariablesUnstagedMessage(this.VariableDeltas.ToList(), keepChanges));

            this.VariableDeltas.Clear();
            this.ClearStagedDeltasCommand.NotifyCanExecuteChanged();

            this.messengerService.Send(new LogMessage($"{staged} variable{(staged > 1 ? "s" : string.Empty)} unstaged"));
        }
    }
}
