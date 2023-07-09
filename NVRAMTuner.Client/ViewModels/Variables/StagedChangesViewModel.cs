namespace NVRAMTuner.Client.ViewModels.Variables
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Messaging;
    using Messages.Variables;
    using Models;
    using System.Collections.ObjectModel;

    /// <summary>
    /// ViewModel for the staged changes view
    /// </summary>
    public class StagedChangesViewModel : ObservableRecipient, IRecipient<VariableStagedMessage>
    {
        /// <summary>
        /// Backing field for <see cref="VariableDeltas"/>
        /// </summary>
        private ObservableCollection<VariableDelta> variableDeltas;

        /// <summary>
        /// Initialises a new instance of the <see cref="StagedChangesViewModel"/> class
        /// </summary>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        public StagedChangesViewModel(IMessenger messenger) : base(messenger)
        {
            this.IsActive = true;
            this.VariableDeltas = new ObservableCollection<VariableDelta>();
        }

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
        }
    }
}
