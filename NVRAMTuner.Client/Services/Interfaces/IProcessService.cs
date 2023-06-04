namespace NVRAMTuner.Client.Services.Interfaces
{
    using System.Diagnostics;

    /// <summary>
    /// Interface for a service that wraps around <see cref="System.Diagnostics.Process"/> to
    /// allow dependency injection and unit testing.
    /// </summary>
    public interface IProcessService
    {
        /// <summary>
        /// Wrapper around <see cref="Process.Start(string, string)"/> to allow dependency injection
        /// and unit testing.
        /// 
        ///     "Starts a process resource by specifying the name of an
        ///     application and a set of command line arguments. Associates the process resource
        ///     with a new <see cref="Process"/>
        ///     component."
        /// </summary>
        /// <param name="fileName">The filename of the external process to start</param>
        /// <param name="arguments">The arguments pass</param>
        /// <returns>A new <see cref="Process"/> instance</returns>
        Process Start(string fileName, string arguments);

        /// <summary>
        /// Wrapper around <see cref="Process.Start(string)"/> to allow dependency injection
        /// and unit testing.
        /// 
        ///     "Starts a process resource by specifying the name of a
        ///     document or application file. Associates the process resource with a new <see cref="Process"/>
        ///     component."
        /// </summary>
        /// <param name="fileName">The filename of the external process to start</param>
        /// <returns>A new <see cref="Process"/> instance</returns>
        Process Start(string fileName);
    }
}