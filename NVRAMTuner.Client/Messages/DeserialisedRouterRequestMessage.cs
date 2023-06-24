#nullable enable

namespace NVRAMTuner.Client.Messages
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models;

    /// <summary>
    /// A <see cref="RequestMessage{T}"/> sent by a recipient to request that if present,
    /// a serialised and encrypted router file should be deserialised, decrypted and send
    /// back as a reply. Router may be null if there is no file present
    /// </summary>
    public class DeserialisedRouterRequestMessage : RequestMessage<Router?>
    {
    }
}