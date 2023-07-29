namespace NVRAMTuner.Client.Messages.Variables
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models.Nvram;

    /// <summary>
    /// Message representing the staging of a new variable change,
    /// represented by a <see cref="IVariable"/> instance
    /// </summary>
    public class VariableStagedMessage : ValueChangedMessage<IVariable>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VariableStagedMessage"/> class
        /// </summary>
        /// <param name="delta">An instance of <see cref="IVariable"/></param>
        public VariableStagedMessage(IVariable delta) : base(delta)
        {
        }
    }
}
