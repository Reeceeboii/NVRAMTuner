namespace NVRAMTuner.Client.Services.Wrappers
{
    using CommunityToolkit.Mvvm.Messaging;
    using Interfaces;

    /// <summary>
    /// Service class aimed to wrap <see cref="IMessenger"/>
    /// </summary>
    public class MessengerService : IMessengerService
    {
        /// <summary>
        /// An instance of <see cref="IMessenger"/>
        /// </summary>
        private readonly IMessenger messenger;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessengerService"/> class
        /// </summary>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        public MessengerService(IMessenger messenger)
        {
            this.messenger = messenger;
        }

        /// <summary>
        /// <inheritdoc cref="IMessenger.Send{TMessage,TToken}"/>
        /// </summary>
        public TMessage Send<TMessage>(TMessage message) where TMessage : class
        {
            return this.messenger.Send(message);
        }
    }
}