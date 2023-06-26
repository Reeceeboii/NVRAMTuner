namespace NVRAMTuner.Client.Messages
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models;
    using System;

    /// <summary>
    /// A message representing a new log entry
    /// </summary>
    public class LogMessage : ValueChangedMessage<LogEntry>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="LogMessage"/> class
        /// </summary>
        /// <param name="entry">A <see cref="LogEntry"/> instance</param>
        public LogMessage(LogEntry entry) : base(entry)
        {
            entry.LogTime = DateTime.Now;
        }
    }
}