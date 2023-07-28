namespace NVRAMTuner.Client.ViewModels.Variables
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using CommunityToolkit.Mvvm.Messaging;
    using MahApps.Metro.Controls.Dialogs;
    using Messages;
    using Messages.Variables;
    using Models;
    using Resources;
    using Services.Interfaces;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// ViewModel for the staged changes view
    /// </summary>
    public class StagedChangesViewModel : ObservableRecipient, IRecipient<VariableStagedMessage>
    {
        /// <summary>
        /// An instance of <see cref="IDialogService"/>
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// Backing field for <see cref="VariableDeltas"/>
        /// </summary>
        private ObservableCollection<VariableDelta> variableDeltas;

        /// <summary>
        /// Initialises a new instance of the <see cref="StagedChangesViewModel"/> class
        /// </summary>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        /// <param name="dialogService">An instance of <see cref="IDialogService"/></param>
        public StagedChangesViewModel(IMessenger messenger, IDialogService dialogService) : base(messenger)
        {
            this.dialogService = dialogService;

            this.IsActive = true;
            this.ClearStagedDeltasCommand = new AsyncRelayCommand(this.ClearStagedDeltasCommandHandlerAsync, 
                () => this.VariableDeltas.Any());

            this.VariableDeltas = new ObservableCollection<VariableDelta>();
        }

        /// <summary>
        /// Gets the command used to clear all of the currently staged deltas
        /// </summary>
        public IAsyncRelayCommand ClearStagedDeltasCommand { get; }

        /// <summary>
        /// Gets or sets an <see cref="ObservableCollection{T}"/> of <see cref="VariableDelta"/> instances,
        /// representing all of the currently staged changes
        /// </summary>
        public ObservableCollection<VariableDelta> VariableDeltas
        {
            get => this.variableDeltas;
            set => this.SetProperty(ref this.variableDeltas, value);
        }

        /// <summary>
        /// Recipient method for the <see cref="VariableStagedMessage"/>
        /// </summary>
        /// <param name="message">An instance of <see cref="VariableStagedMessage"/></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Receive(VariableStagedMessage message)
        {
            this.VariableDeltas.Add(message.Value);
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
            this.Messenger.Send(new VariablesUnstagedMessage(this.VariableDeltas.ToList(), keepChanges));

            this.VariableDeltas.Clear();
            this.ClearStagedDeltasCommand.NotifyCanExecuteChanged();

            this.Messenger.Send(new LogMessage($"{staged} variable{(staged > 1 ? "s" : string.Empty)} unstaged"));
        }
    }
}
