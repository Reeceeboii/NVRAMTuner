namespace NVRAMTuner.Client.Messages.Variables
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models.Nvram;
    using System.Collections.Generic;

    /// <summary>
    /// Message representing the unstaging of a set of variables,
    /// represented by a list of <see cref="IVariable"/> instances
    /// </summary>
    public class VariablesUnstagedMessage : ValueChangedMessage<List<IVariable>>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VariableStagedMessage"/> class
        /// </summary>
        /// <param name="deltas">A <see cref="List{T}"/> of <see cref="IVariable"/> instances</param>
        /// <param name="keepChanges">Represents whether or not all of the unstaged variables
        /// should have their changes abandoned or not</param>
        public VariablesUnstagedMessage(List<IVariable> deltas, bool keepChanges) : base(deltas)
        {
            this.KeepChanges = keepChanges;
        }

        /// <summary>
        /// Gets a bool representing whether or not all of the unstaged variables
        /// should have their changes abandoned
        /// </summary>
        public bool KeepChanges { get; }
    }
}