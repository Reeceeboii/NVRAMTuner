namespace NVRAMTuner.Client.Messages
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using System;

    /// <summary>
    /// A message denoting that an error has occurred and that its message should be displayed in a dialog
    /// </summary>
    public class DialogErrorMessage : ValueChangedMessage<Exception>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DialogErrorMessage"/> class
        /// </summary>
        /// <param name="ex"></param>
        public DialogErrorMessage(Exception ex) : base(ex)
        {
        }
    }
}
