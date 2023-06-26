namespace NVRAMTuner.Client.Models
{
    using System;

    /// <summary>
    /// Model representing a log entry
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Gets or sets the time at which the log was created
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// Gets or sets the time at which the log was created, but formatted to the user's local
        /// culture
        /// </summary>
        public string PrettyLogTime { get; set; }

        /// <summary>
        /// Gets or sets the log message
        /// </summary>
        public string LogMessage { get; set; }
    }
}