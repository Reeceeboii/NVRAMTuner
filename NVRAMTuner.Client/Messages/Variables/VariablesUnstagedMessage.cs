namespace NVRAMTuner.Client.Messages.Variables
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models;
    using System.Collections.Generic;

    /// <summary>
    /// Message representing the unstaging of a set of variables,
    /// represented by a list of <see cref="VariableDelta"/> instances
    /// </summary>
    public class VariablesUnstagedMessage : ValueChangedMessage<List<VariableDelta>>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VariableStagedMessage"/> class
        /// </summary>
        /// <param name="delta">A <see cref="List{T}"/> of <see cref="VariableDelta"/> instances</param>
        /// <param name="abandonChanges">Represents whether or not all of the unstaged variables
        /// should have their changes abandoned or not</param>
        public VariablesUnstagedMessage(List<VariableDelta> delta, bool abandonChanges) : base(delta)
        {
            this.AbandonChanges = abandonChanges;
        }

        /// <summary>
        /// Gets a bool representing whether or not all of the unstaged variables
        /// should have their changes abandoned
        /// </summary>
        public bool AbandonChanges { get; }
    }
}