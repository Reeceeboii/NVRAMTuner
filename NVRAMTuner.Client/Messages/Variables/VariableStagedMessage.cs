namespace NVRAMTuner.Client.Messages.Variables
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models;

    /// <summary>
    /// Message representing the staging of a new variable change,
    /// represented by a <see cref="VariableDelta"/> instance
    /// </summary>
    public class VariableStagedMessage : ValueChangedMessage<VariableDelta>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VariableStagedMessage"/> class
        /// </summary>
        /// <param name="delta">An instance of <see cref="VariableDelta"/></param>
        public VariableStagedMessage(VariableDelta delta) : base(delta)
        {
        }
    }
}
