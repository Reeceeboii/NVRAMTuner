namespace NVRAMTuner.Client.ViewModels.Variables
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using CommunityToolkit.Mvvm.Messaging;
    using Messages;
    using Messages.Variables;
    using Models;
    using Models.Nvram;

    /// <summary>
    /// ViewModel for the Edits view
    /// </summary>
    public class EditsViewModel : ObservableRecipient, IRecipient<VariableSelectedMessage>
    {
        /// <summary>
        /// Backing field for <see cref="SelectedVariable"/>
        /// </summary>
        private IVariable selectedVariable;

        /// <summary>
        /// Backing field for <see cref="EditableValue"/>
        /// </summary>
        private string editableValue;

        /// <summary>
        /// Initialises a new instance of the <see cref="EditsViewModel"/> class
        /// </summary>
        /// <param name="messenger">Instance of <see cref="IMessenger"/></param>
        public EditsViewModel(IMessenger messenger) : base(messenger)
        {
            this.IsActive = true;

            this.RollbackChangesCommand = new RelayCommand(this.RollbackChangesCommandHandler, this.RollbackOrStageCommandCanExecute);
            this.StageChangesCommand = new RelayCommand(this.StageChangesCommandHandler, this.RollbackOrStageCommandCanExecute);
        }

        /// <summary>
        /// Gets or sets a command used to roll back changes applied to a specific variable
        /// </summary>
        public IRelayCommand RollbackChangesCommand { get; set; }

        /// <summary>
        /// Gets or sets a command used to stage any changes made to the current variable
        /// represented by <see cref="SelectedVariable"/>
        /// </summary>
        public IRelayCommand StageChangesCommand { get; set; }

        /// <summary>
        /// Gets or sets the currently selected variable that the use is able to view details about and edit
        /// </summary>
        public IVariable SelectedVariable
        {
            get => this.selectedVariable;
            set
            {
                if (value == this.selectedVariable)
                {
                    return;
                }

                this.SetProperty(ref this.selectedVariable, value);
            }
        }

        /// <summary>
        /// Gets or sets an editable value. This is initialised as a copy of the variable's
        /// value so that the edits can always be rolled back to the originals pulled from the router if the user wishes
        /// </summary>
        public string EditableValue
        {
            get => this.editableValue;
            set
            {
                this.SetProperty(ref this.editableValue, value);

                this.RollbackChangesCommand.NotifyCanExecuteChanged();
                this.StageChangesCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Recipient method for <see cref="VariableSelectedMessage"/> instances
        /// </summary>
        /// <param name="message">Instance of <see cref="VariableSelectedMessage"/></param>
        public void Receive(VariableSelectedMessage message)
        {
            if (message.Value == null)
            {
                return;
            }

            this.SelectedVariable = message.Value;
            this.EditableValue = message.Value.OriginalValue;
            this.RollbackChangesCommand.NotifyCanExecuteChanged();
            this.StageChangesCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Method to handle the <see cref="RollbackChangesCommand"/>
        /// </summary>
        private void RollbackChangesCommandHandler()
        {
            this.EditableValue = this.SelectedVariable.OriginalValue;
        }

        /// <summary>
        /// Method to handle the <see cref="StageChangesCommand"/>
        /// </summary>
        private void StageChangesCommandHandler()
        {
            IVariable appliedDelta = this.selectedVariable;
            appliedDelta.ValueDelta = this.EditableValue;
            VariableDelta delta = new VariableDelta(this.selectedVariable, appliedDelta);

            this.Messenger.Send(new VariableStagedMessage(delta));
            this.Messenger.Send(new LogMessage($"Staged changes to {this.SelectedVariable.Name}"));

            this.SelectedVariable = null;
            this.RollbackChangesCommand.NotifyCanExecuteChanged();
            this.StageChangesCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Method used to determine if the <see cref="RollbackChangesCommand"/> or
        /// the <see cref="StageChangesCommand"/> can execute. As these commands can both
        /// only be executed if there has been a change to the value of the selected variable,
        /// they can share a CanExecute method.
        /// </summary>
        /// <returns>A bool representing whether the commands can execute</returns>
        private bool RollbackOrStageCommandCanExecute()
        {
            if (this.SelectedVariable != null)
            {
                return this.EditableValue != this.SelectedVariable.OriginalValue;
            }

            return false;
        }
    }
}