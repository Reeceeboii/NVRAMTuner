namespace NVRAMTuner.Client.Services.Interfaces
{
    using Models.Nvram;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for a service that handles retrieving NVRAM variables from a router
    /// </summary>
    public interface IVariableService
    {
        /// <summary>
        /// Loads all NVRAM variables by contacting the router and running the 'nvram show' command.
        /// Returns this data in an <see cref="Nvram"/> variable
        /// </summary>
        /// <returns>An asynchronous<see cref="Task{TResult}"/></returns>
        Task<Nvram> GetNvramVariablesAsync();
    }
}