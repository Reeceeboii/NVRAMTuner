namespace NVRAMTuner.Client.Services.Wrappers.Interfaces
{
    using CommunityToolkit.Mvvm.Messaging;

    /// <summary>
    /// Interface for a service class wrapping around various <see cref="IMessenger"/> methods
    /// </summary>
    public interface IMessengerService
    {
        /// <summary>
        /// <inheritdoc cref="IMessenger.Send{TMessage,TToken}"/>
        /// </summary>
        TMessage Send<TMessage>(TMessage message) where TMessage : class;

        /// <summary>
        /// <inheritdoc cref="IMessenger.Send{TMessage,TToken}"/>
        /// </summary>
        TMessage Send<TMessage>() where TMessage : class, new();

        /// <summary>
        /// <inheritdoc cref="IMessenger.Register{TRecipient,TMessage,TToken}"/>
        /// </summary>
        void Register<TRecipient, TMessage>(TRecipient recipient, MessageHandler<TRecipient, TMessage> handler)
            where TRecipient : class
            where TMessage : class;
    }
}