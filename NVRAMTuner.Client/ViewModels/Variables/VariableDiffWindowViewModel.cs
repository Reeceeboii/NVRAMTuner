namespace NVRAMTuner.Client.ViewModels.Variables
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Messaging;
    using Messages.Variables.Staged;
    using Models.Enums;
    using Models.Nvram;
    using System;
    using System.Collections.Generic;
    using System.Text;

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
        /// Backing field for <see cref="SelectedDiffSplitCharacter"/>
        /// </summary>
        private string selectedDiffSplitCharacter;

        /// <summary>
        /// Backing field for <see cref="OldText"/>
        /// </summary>
        private string oldText;

        /// <summary>
        /// Backing field for <see cref="NewText"/>
        /// </summary>
        private string newText;

        /// <summary>
        /// Backing field for <see cref="DelimiterEnumToStringMap"/>
        /// </summary>
        private Dictionary<VariableDiffDelimiter, string> delimiterEnumToStringMap;

        /// <summary>
        /// Initialises a new instance of the <see cref="VariableDiffWindowViewModel"/> class
        /// </summary>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        public VariableDiffWindowViewModel(IMessenger messenger) : base(messenger)
        {
            RequestSelectedStagedVariableMessage response = this.Messenger.Send<RequestSelectedStagedVariableMessage>();
            this.SelectedStagedVariable = response.Response;

            this.WindowTitle = $"Diff of '{this.SelectedStagedVariable.Name}'";

            this.DelimiterEnumToStringMap = new Dictionary<VariableDiffDelimiter, string>
            {
                { VariableDiffDelimiter.NoSplit, "No split" },
                { VariableDiffDelimiter.Comma, "," },
                { VariableDiffDelimiter.LessThan, "<" }
            };

            this.SelectedDiffSplitCharacter = this.DelimiterEnumToStringMap[0];

            this.OldText = this.SelectedStagedVariable.OriginalValue;
            this.NewText = this.SelectedStagedVariable.ValueDelta;

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
        /// Gets or sets the old text to display in the diff view
        /// </summary>
        public string OldText
        {
            get => this.oldText;
            set => this.SetProperty(ref this.oldText, value);
        }

        /// <summary>
        /// Gets or sets the new text to display in the diff view
        /// </summary>
        public string NewText
        {
            get => this.newText;
            set => this.SetProperty(ref this.newText, value);
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
        /// Gets or sets a dictionary values that can be used to split diff values.
        /// This is provided as the majority (or perhaps all) of the NVRAM values are single line
        /// values with no line breaks. This makes diffs pretty useless.
        /// </summary>
        public Dictionary<VariableDiffDelimiter, string> DelimiterEnumToStringMap
        {
            get => this.delimiterEnumToStringMap;
            set => this.SetProperty(ref this.delimiterEnumToStringMap, value);
        }

        /// <summary>
        /// Gets or sets the currently selected diff split character
        /// </summary>
        public string SelectedDiffSplitCharacter
        {
            get => this.selectedDiffSplitCharacter;
            set
            {
                if (value == this.DelimiterEnumToStringMap[VariableDiffDelimiter.NoSplit])
                {
                    this.OldText = this.SelectedStagedVariable.OriginalValue;
                    this.NewText = this.SelectedStagedVariable.ValueDelta;
                    this.SetProperty(ref this.selectedDiffSplitCharacter, value);
                    return;
                }

                this.SetProperty(ref this.selectedDiffSplitCharacter, value);
                
                StringBuilder sb = new StringBuilder();


                // TODO - these are slightly scuffed. They add new blank lines for no reason
                foreach (string line in this.OldText.Split(char.Parse(value)))
                {
                    sb.Append($"{line}{Environment.NewLine}");
                }
                this.OldText = sb.ToString();

                sb.Clear();

                foreach (string line in this.NewText.Split(char.Parse(value)))
                {
                    sb.Append($"{line}{Environment.NewLine}");
                }
                this.NewText = sb.ToString();
            }
        }
    }
}