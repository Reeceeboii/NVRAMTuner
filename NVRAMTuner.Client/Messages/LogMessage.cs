namespace NVRAMTuner.Client.Messages
{
    using Models;
    using System;

    /// <summary>
    /// A message representing a new log entry
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="LogMessage"/> class
        /// </summary>
        /// <param name="message">The log message</param>
        public LogMessage(string message)
        {
            this.Value = new LogEntry
            {
                LogTime = DateTime.Now, LogMessage = message
            };
        }

        /// <summary>
        /// Gets the message's value
        /// </summary>
        public LogEntry Value { get; }
    }
}