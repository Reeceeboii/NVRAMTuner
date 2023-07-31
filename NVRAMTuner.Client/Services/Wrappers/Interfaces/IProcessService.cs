namespace NVRAMTuner.Client.Services.Wrappers.Interfaces
{
    using System.Diagnostics;

    /// <summary>
    /// Interface for a service that wraps around <see cref="Process"/> to
    /// allow dependency injection and unit testing.
    /// </summary>
    public interface IProcessService
    {
        /// <summary>
        /// <inheritdoc cref="Process.Start(string, string)"/>
        /// </summary>
        Process Start(string fileName, string arguments);

        /// <summary>
        /// <inheritdoc cref="Process.Start(string)"/>
        /// </summary>
        Process Start(string fileName);
    }
}