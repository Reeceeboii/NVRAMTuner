namespace NVRAMTuner.Client.Services
{
    using Interfaces;
    using Models.Enums;
    using System;

    /// <summary>
    /// Service class for managing interactions with the <see cref="Environment"/>.
    /// Acts as a wrapper around the <see cref="System.Environment"/> class
    /// </summary>
    public class EnvironmentService : IEnvironmentService
    {
        /// <summary>
        /// Wrapper around <see cref="Environment.GetFolderPath(Environment.SpecialFolder)"/>
        /// to allow dependency injection and unit testing.
        ///
        ///     "Gets the path to the system special folder that is identified by the specified enumeration."
        /// </summary>
        /// <param name="specialFolder">A member of the <see cref="Environment.SpecialFolder"/> enum</param>
        /// <returns>The string associated with the path of <paramref name="specialFolder"/></returns>
        public string GetFolderPath(Environment.SpecialFolder specialFolder)
        {
            return Environment.GetFolderPath(specialFolder);
        }
    }
}