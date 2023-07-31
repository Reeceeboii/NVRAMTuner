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
    }
}