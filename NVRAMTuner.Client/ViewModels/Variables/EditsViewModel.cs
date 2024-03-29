﻿namespace NVRAMTuner.Client.ViewModels.Variables
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Messages;
    using Messages.Variables;
    using Messages.Variables.Staged;
    using Models.Nvram;
    using Services.Wrappers.Interfaces;
    using System;

    /// <summary>
    /// ViewModel for the Edits view
    /// </summary>
    public class EditsViewModel : ObservableObject
    {
        /// <summary>
        /// Backing field for <see cref="SelectedVariable"/>
        /// </summary>
        private IVariable selectedVariable;

        /// <summary>
        /// Instance of <see cref="IMessengerService"/>
        /// </summary>
        private readonly IMessengerService messengerService;

        /// <summary>
        /// Initialises a new instance of the <see cref="EditsViewModel"/> class
        /// </summary>
        /// <param name="messengerService">Instance of <see cref="IMessengerService"/></param>
        public EditsViewModel(IMessengerService messengerService)
        {
            this.messengerService = messengerService;

            this.RollbackChangesCommand = new RelayCommand(this.RollbackChangesCommandHandler, this.RollbackOrStageCommandCanExecute);
            this.StageChangesCommand = new RelayCommand(this.StageChangesCommandHandler, this.RollbackOrStageCommandCanExecute);

            // register messages
            this.messengerService.Register<EditsViewModel, VariableSelectedMessage>(this, 
                (recipient, message) => this.Receive(message));
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
            private set => this.SetProperty(ref this.selectedVariable, value); 
        }

        /// <summary>
        /// Recipient method for <see cref="VariableSelectedMessage"/> instances
        /// </summary>
        /// <param name="message">Instance of <see cref="VariableSelectedMessage"/></param>
        private void Receive(VariableSelectedMessage message)
        {
            if (message.Value == null)
            {
                return;
            }

            // unsubscribe from the previous variable's published delta change event if a new (& different) selection is made
            if (this.SelectedVariable != null && message.Value.Name != this.SelectedVariable.Name)
            {
                this.SelectedVariable.ValueDeltaChanged -= this.SelectedValueOnValueDeltaChanged;
            }
            
            this.SelectedVariable = message.Value;
            this.SelectedVariable.ValueDeltaChanged += this.SelectedValueOnValueDeltaChanged;

            this.RollbackChangesCommand.NotifyCanExecuteChanged();
            this.StageChangesCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Method to handle the <see cref="RollbackChangesCommand"/>
        /// </summary>
        private void RollbackChangesCommandHandler()
        {
            this.SelectedVariable.ValueDelta = this.SelectedVariable.OriginalValue;
        }

        /// <summary>
        /// Method to handle the <see cref="StageChangesCommand"/>
        /// </summary>
        private void StageChangesCommandHandler()
        {
            if (this.SelectedVariable.ValueDelta == this.SelectedVariable.OriginalValue)
            {
                return;
            }

            this.SelectedVariable.ValueDeltaChanged -= this.SelectedValueOnValueDeltaChanged;

            this.messengerService.Send(new VariableStagedMessage(this.SelectedVariable));
            this.messengerService.Send(new LogMessage($"Staged changes to {this.SelectedVariable.Name}"));

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
                return this.SelectedVariable.ValueDelta != this.SelectedVariable.OriginalValue;
            }

            return false;
        }

        /// <summary>
        /// Event handler for the <see cref="IVariable.ValueDeltaChanged"/> event.
        /// When the delta changes, we want to re-evaluate the can-execute state
        /// of <see cref="RollbackChangesCommand"/> and <see cref="StageChangesCommand"/>
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event args</param>
        private void SelectedValueOnValueDeltaChanged(object sender, EventArgs e)
        {
            this.RollbackChangesCommand.NotifyCanExecuteChanged();
            this.StageChangesCommand.NotifyCanExecuteChanged();
        }
    }
}