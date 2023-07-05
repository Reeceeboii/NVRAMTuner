namespace NVRAMTuner.Client.Messages.Theme
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models.Enums;

    /// <summary>
    /// Message representing a request to receive the current application theme
    /// in the form of an <see cref="ApplicationTheme"/> member
    /// </summary>
    public class ThemeRequestMessage : RequestMessage<ApplicationTheme>
    {
    }
}