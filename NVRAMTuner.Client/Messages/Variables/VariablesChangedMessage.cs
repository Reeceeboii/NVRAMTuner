namespace NVRAMTuner.Client.Messages.Variables
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models.Nvram;

    /// <summary>
    /// <see cref="ValueChangedMessage{T}"/> representing a new updated set of variables
    /// wrapped in an <see cref="Nvram"/> instance
    /// </summary>
    public class VariablesChangedMessage : ValueChangedMessage<Nvram>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VariablesChangedMessage"/> class
        /// </summary>
        /// <param name="nvram">An instance of <see cref="Nvram"/></param>
        public VariablesChangedMessage(Nvram nvram) : base(nvram)
        {
        }
    }
}