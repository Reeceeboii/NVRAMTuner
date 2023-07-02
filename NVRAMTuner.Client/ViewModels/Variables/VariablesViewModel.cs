namespace NVRAMTuner.Client.ViewModels.Variables
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Messaging;
    using Messages.Variables;
    using Models.Nvram;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// ViewModel for the variables view
    /// </summary>
    public class VariablesViewModel : ObservableRecipient, IRecipient<VariablesChangedMessage>
    {
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
        /// Initialises a new instance of the <see cref="VariablesViewModel"/> class
        /// </summary>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        public VariablesViewModel(IMessenger messenger) : base(messenger)
        {
            this.Variables = new ObservableCollection<IVariable>();
            this.IsActive = true;
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
        /// Gets or sets the currently selected variable in the datagrid
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
        }
    }
}