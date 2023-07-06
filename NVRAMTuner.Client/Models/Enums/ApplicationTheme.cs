namespace NVRAMTuner.Client.Models.Enums
{
    using System;

    /// <summary>
    /// An enumeration representing the different available application themes
    /// </summary>
    [Serializable]
    public enum ApplicationTheme
    {
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