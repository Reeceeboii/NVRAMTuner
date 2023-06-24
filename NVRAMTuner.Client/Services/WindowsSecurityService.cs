namespace NVRAMTuner.Client.Services
{
    using Interfaces;
    using System.Security.Principal;

    /// <summary>
    /// Service class to act as a wrapper around <see cref="System.Security"/> methods required
    /// for the functioning of NVRAMTuner
    /// </summary>
    public class WindowsSecurityService : IWindowsSecurityService
    {
        /// <summary>
        /// Gets the Security Identifier (SID) for the current user
        /// </summary>
        /// <returns>A <see cref="SecurityIdentifier"/> instance</returns>
        public SecurityIdentifier RetrieveUserSid()
        {
            return WindowsIdentity.GetCurrent().User;
        }
    }
}