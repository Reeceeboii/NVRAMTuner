namespace NVRAMTuner.Client.Messages
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models.Enums;

    /// <summary>
    /// A message denoting that the user has requested to move to the router setup page
    /// </summary>
    public class NavigationRequestMessage : ValueChangedMessage<NavigableViewModel>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="NavigationRequestMessage"/> class
        /// </summary>
        /// <param name="nvm">An instance of <see cref="NavigableViewModel"/></param>
        public NavigationRequestMessage(NavigableViewModel nvm) : base(nvm)
        {
        }
    }
}
