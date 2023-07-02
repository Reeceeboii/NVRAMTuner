namespace NVRAMTuner.Client.Messages.Variables
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models.Nvram;

    /// <summary>
    /// Message representing a new <see cref="IVariable"/> that has been selected from the variable list by the user
    /// </summary>
    public class VariableSelectedMessage : ValueChangedMessage<IVariable>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VariableSelectedMessage"/> class
        /// </summary>
        /// <param name="variable">Instance of <see cref="IVariable"/></param>
        public VariableSelectedMessage(IVariable variable) : base(variable)
        {
        }
    }
}
