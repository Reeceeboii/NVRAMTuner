namespace NVRAMTuner.Client.Messages
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models;

    /// <summary>
    /// Message that is sent whenever an active router connection is terminated
    /// </summary>
    public class RouterDisconnectMessage : ValueChangedMessage<Router>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="RouterDisconnectMessage"/> class
        /// </summary>
        /// <param name="router">The <see cref="Router"/> that was disconnected from</param>
        public RouterDisconnectMessage(Router router) : base(router)
        {
        }
    }
}
