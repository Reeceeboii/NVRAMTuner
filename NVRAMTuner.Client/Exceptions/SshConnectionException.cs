namespace NVRAMTuner.Client.Exceptions
{
    using System;

    /// <summary>
    /// Custom exception type representing a failure to connect to a remote SSH server
    /// </summary>
    public class SshConnectionException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="SshConnectionException"/> class
        /// </summary>
        public SshConnectionException() { }

        /// <summary>
        /// Initialises a new instance of the <see cref="SshConnectionException"/> class
        /// </summary>
        /// <param name="message">A message explaining the exception</param>
        public SshConnectionException(string message) :base(message) { }

        /// <summary>
        /// Initialises a new instance of the <see cref="SshConnectionException"/> class
        /// </summary>
        /// <param name="message">A message explaining the exception</param>
        /// <param name="inner">The inner exception that caused this <see cref="SshConnectionException"/>
        /// to be raised in the first place</param>
        public SshConnectionException(string message, Exception inner) : base(message, inner) { }
    }
}