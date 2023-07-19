﻿namespace NVRAMTuner.Client.ViewModels.Variables
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Messaging;
    using Messages;
    using Messages.Variables;
    using Models.Nvram;
    using Models.Nvram.Concrete;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// ViewModel for the variables view
    /// </summary>
    public class VariablesViewModel : ObservableRecipient
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
        /// Backing field for <see cref="VariableSizeBytes"/>
        /// </summary>
        private int variableSizeBytes;

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
            this.VariableSizeBytes = message.Value.VariableSizeBytes;
        }

        /// <summary>
        /// Recipient method for <see cref="RouterDisconnectMessage"/> messages
        /// </summary>
        /// <param name="message">An instance of <see cref="RouterDisconnectMessage"/></param>
        public void Receive(RouterDisconnectMessage message)
        {
            this.Variables.Clear();
        }

        /// <summary>
        /// Recipient method for <see cref="VariableStagedMessage"/> messages.
        /// This method has absolutely abysmal performance but I don't want come up with a more
        /// elegant solution right now. And plus my CPU is so fast I'm biased towards this
        /// not actually mattering at all. HOWEVER: TODO - performance
        /// </summary>
        /// <param name="message">An instance of <see cref="VariableStagedMessage"/></param>
        public void Receive(VariableStagedMessage message)
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
    }
}