namespace NVRAMTuner.Client.Models.Enums
{
    /// <summary>
    /// An enumeration representing the different available application themes
    /// </summary>
    public enum ApplicationTheme
    {
        /// <summary>
        /// Default theme, syncs with the host operating system
        /// </summary>
        SyncWithOs,

        #region DarkThemes

        /// <summary>
        /// The dark blue theme
        /// </summary>
        DarkBlue,

        /// <summary>
        /// The dark green theme
        /// </summary>
        DarkOrange,

        /// <summary>
        /// The dark red theme
        /// </summary>
        DarkRed,

        #endregion

        #region LightThemes

        /// <summary>
        /// The light blue theme
        /// </summary>
        LightBlue,

        /// <summary>
        /// The light green theme
        /// </summary>
        LightOrange,

        /// <summary>
        /// The light red theme
        /// </summary>
        LightRed,

        #endregion
    }
}