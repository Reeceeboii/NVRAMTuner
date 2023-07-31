namespace NVRAMTuner.Client.Services.Wrappers
{
    using Interfaces;
    using System;

    /// <summary>
    /// Service class for managing interactions with the <see cref="Environment"/>.
    /// Acts as a wrapper around the <see cref="System.Environment"/> class
    /// </summary>
    public class EnvironmentService : IEnvironmentService
    {

        /// <summary>
        /// <inheritdoc cref="Environment.GetFolderPath(Environment.SpecialFolder)"/>
        /// </summary>
        public string GetFolderPath(Environment.SpecialFolder specialFolder)
        {
            return Environment.GetFolderPath(specialFolder);
        }
    }
}