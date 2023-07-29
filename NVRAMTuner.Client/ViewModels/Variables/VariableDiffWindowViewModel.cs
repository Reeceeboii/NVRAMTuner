namespace NVRAMTuner.Client.ViewModels.Variables
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Messaging;
    using Messages.Variables;
    using Models.Nvram;
    using System.Collections.Generic;

    /// <summary>
    /// ViewModel for the VariableDiff window
    /// </summary>
    public class VariableDiffWindowViewModel : ObservableRecipient
    {
        /// <summary>
        /// Backing field for <see cref="WindowTitle"/>
        /// </summary>
        private string windowTitle;

        /// <summary>
        /// Backing field for <see cref="SelectedStagedVariable"/>
        /// </summary>
        private IVariable selectedStagedVariable;

        /// <summary>
        /// Backing field for <see cref="UnifiedDiff"/>
        /// </summary>
        private bool unifiedDiff;

        /// <summary>
        /// Backing field for <see cref="DiffSplitCharacters"/>
        /// </summary>
        private List<string> diffSplitCharacters;

        /// <summary>
        /// Initialises a new instance of the <see cref="VariableDiffWindowViewModel"/> class
        /// </summary>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        public VariableDiffWindowViewModel(IMessenger messenger) : base(messenger)
        {
            RequestSelectedStagedVariableMessage response = this.Messenger.Send<RequestSelectedStagedVariableMessage>();
            this.SelectedStagedVariable = response.Response;

            this.WindowTitle = $"Diff of '{this.SelectedStagedVariable.Name}'";

            this.UnifiedDiff = false;
        }

        /// <summary>
        /// Gets or sets the staged variable whose diff is currently being viewed
        /// </summary>
        public IVariable SelectedStagedVariable
        {
            get => this.selectedStagedVariable;
            private set => this.SetProperty(ref this.selectedStagedVariable, value);
        }

        /// <summary>
        /// Gets or sets the diff window's title
        /// </summary>
        public string WindowTitle
        {
            get => this.windowTitle;
            private set => this.SetProperty(ref this.windowTitle, value);
        }

        /// <summary>
        /// Gets or sets a value representing whether or not the diff should be displayed in a unified fashion
        /// </summary>
        public bool UnifiedDiff
        {
            get => this.unifiedDiff;
            set => this.SetProperty(ref this.unifiedDiff, value);
        }

        /// <summary>
        /// Gets or sets a list of values that can be used to split diff values.
        /// This is provided as the majority (or perhaps all) of the NVRAM values are single line
        /// values with no line breaks. This makes diffs pretty useless.
        /// </summary>
        public List<string> DiffSplitCharacters
        {
            get => this.diffSplitCharacters;
            set => this.SetProperty(ref this.diffSplitCharacters, value);
        }
    }
}