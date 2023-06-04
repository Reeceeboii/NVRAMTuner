namespace NVRAMTuner.Client.Messages
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models;

    /// <summary>
    /// A message denoting that the user's SSH keys have been found.
    /// Attaches a <see cref="SshKeyPair"/> instance to the message
    /// containing information about the keys.
    /// </summary>
    public class SshKeysFoundMessage : ValueChangedMessage<SshKeyPair>
    {
        public SshKeysFoundMessage(SshKeyPair keyPair) : base(keyPair)
        {
        }
    }
}