namespace NVRAMTuner.Client.Messages.Settings
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models.Enums;

    /// <summary>
    /// Message to represent a request to change theme
    /// </summary>
    public class ThemeChangeMessage : ValueChangedMessage<ApplicationTheme>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ThemeChangeMessage"/> class
        /// </summary>
        /// <param name="theme">A member of the <see cref="ApplicationTheme"/> enumeration</param>
        public ThemeChangeMessage(ApplicationTheme theme) : base(theme)
        {
        }
    }
}