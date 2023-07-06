namespace NVRAMTuner.Client.Utils
{
    using Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Application them helper class
    /// </summary>
    public static class ApplicationThemes
    {
        /// <summary>
        /// Dictionary that maps <see cref="ApplicationTheme"/> enumeration values
        /// to their string counterparts
        /// </summary>
        private static readonly Dictionary<ApplicationTheme, string> ThemeToStringMap =
            new Dictionary<ApplicationTheme, string>
            {
                // Dark themes
                { ApplicationTheme.DarkBlue, "Dark.Blue" },
                { ApplicationTheme.DarkOrange, "Dark.Orange" },
                { ApplicationTheme.DarkRed, "Dark.Red" },
                // Light themes
                { ApplicationTheme.LightBlue, "Light.Blue" },
                { ApplicationTheme.LightOrange, "Light.Orange" },
                { ApplicationTheme.LightRed, "Light.Red" }
            };

        /// <summary>
        /// Converts a <see cref="ApplicationTheme"/> enumeration value to a string
        /// that can be used to set the application theme
        /// </summary>
        /// <param name="theme">An <see cref="ApplicationTheme"/> member</param>
        /// <returns>A string representation of <paramref name="theme"></paramref></returns>
        /// <exception cref="InvalidEnumArgumentException">If <paramref name="theme"/> is not a
        /// member of <see cref="ApplicationTheme"/></exception>
        public static string ThemeToString(ApplicationTheme theme)
        {
            if (!Enum.IsDefined(typeof(ApplicationTheme), theme))
            {
                throw new InvalidEnumArgumentException(nameof(theme));
            }

            return ThemeToStringMap[theme];
        }
    }
}