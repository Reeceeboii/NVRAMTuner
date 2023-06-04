namespace NVRAMTuner.Client.Services.Interfaces
{
    using System;

    /// <summary>
    /// Interface for a service that wraps around <see cref="System.Environment"/> to
    /// allow dependency injection and unit testing.
    /// </summary>
    public interface IEnvironmentService
    {
        /// <summary>
        /// Wrapper around <see cref="Environment.GetFolderPath(Environment.SpecialFolder)"/>
        /// to allow dependency injection and unit testing.
        ///
        ///     "Gets the path to the system special folder that is identified by the specified enumeration."
        /// </summary>
        /// <param name="specialFolder">A member of the <see cref="Environment.SpecialFolder"/> enum</param>
        /// <returns>The string associated with the path of <paramref name="specialFolder"/></returns>
        string GetFolderPath(Environment.SpecialFolder specialFolder);
    }
}