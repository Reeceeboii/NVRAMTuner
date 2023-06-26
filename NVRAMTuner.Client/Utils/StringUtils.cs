namespace NVRAMTuner.Client.Utils
{
    using Models;
    using System.Collections.ObjectModel;
    using System.Text;

    /// <summary>
    /// A collection of static, reusable string utilities
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Converts an <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/> instances to a
        /// single string that can be served into a UI control, or saved directly to a file
        /// </summary>
        /// <param name="entries">The <see cref="LogEntry"/> instances</param>
        /// <returns>The entries, converted to a string</returns>
        public static string LogEntryCollectionToString(ObservableCollection<LogEntry> entries)
        {
            StringBuilder sb = new StringBuilder();

            foreach (LogEntry entry in entries)
            {
                sb.AppendLine($"{entry.PrettyLogTime} | {entry.LogMessage}");
            }

            return sb.ToString();
        }
    }
}