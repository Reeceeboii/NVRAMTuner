namespace NVRAMTuner.Client.Services
{
    using System.Linq;

    public class NetworkService
    {
        /// <summary>
        /// Verifies that a given string can be assumed to be a correctly formatted IPv4 address.
        /// This function does not test that a given address exist and/or is reachable over the local
        /// network, however, it can discern if something silly like "1250.34.2.4" or "  1.1."
        /// (or empty/whitespace strings) was/were entered.
        /// </summary>
        /// <param name="address">The IPv4 address to verify</param>
        /// <returns>A bool representing whether or not the <paramref name="address"/> provided
        /// is a validly formatted IPv4 address or not</returns>
        public static bool VerifyIpv4Address(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return false;
            }

            string[] octets = address.Split('.');
            return octets.Length == 4 && octets.All(octet => byte.TryParse(octet, out _));
        }

        /// <summary>
        /// Verifies that a given network port is of a correct format.
        /// </summary>
        /// <param name="port">A port number, given as an int</param>
        /// <returns>A bool representing whether or not <paramref name="port"/>
        /// is a valid port number</returns>
        public static bool VerifyNetworkPort(int port)
        {
            return port >= 1 && port <= 65535;
        }

        /// <summary>
        /// Verifies that a given network port is of a correct format.
        /// This overload accepts a string, and uses the <see cref="VerifyNetworkPort(int)"/>
        /// signature version of this function to further verify the range of the port once
        /// it has been ascertained that the string correctly converts to an int in the first
        /// place.
        /// </summary>
        /// <param name="port">A port number, in string format</param>
        /// <returns>A bool representing whether or not <paramref name="port"/> is a valid
        /// network port</returns>
        public static bool VerifyNetworkPort(string port)
        {
            if (string.IsNullOrWhiteSpace(port))
            {
                return false;
            }

            int.TryParse(port, out int intPort);
            return VerifyNetworkPort(intPort);
        }
    }
}