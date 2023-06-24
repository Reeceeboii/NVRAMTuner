namespace NVRAMTuner.Client.Services.Interfaces
{
    using System.Security.Principal;

    /// <summary>
    /// An interface for a service that handles various Windows Security operations
    /// </summary>
    public interface IWindowsSecurityService
    {
        /// <summary>
        /// Gets the Security Identifier (SID) for the current user
        /// </summary>
        /// <returns>A <see cref="SecurityIdentifier"/> instance</returns>
        SecurityIdentifier RetrieveUserSid();
    }
}