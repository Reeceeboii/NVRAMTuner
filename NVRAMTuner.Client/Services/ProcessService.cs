namespace NVRAMTuner.Client.Services
{
    using System.Diagnostics;

    /// <summary>
    /// Service class for managing interactions with external processes.
    /// Acts as a wrapper around the <see cref="System.Diagnostics.Process"/> class
    /// </summary>
    public class ProcessService : IProcessService
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
        public Process Start(string fileName, string arguments)
        {
            return Process.Start(fileName, arguments);
        }

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
        public Process Start(string fileName)
        {
            return Process.Start(fileName);
        }
    }
}