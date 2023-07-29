namespace NVRAMTuner.Client.Models.Enums
{
    /// <summary>
    /// Enumeration representing the different types of delimiters available to users
    /// to use to split diff text up
    /// </summary>
    public enum VariableDiffDelimiter
    {
        /// <summary>
        /// No split, leave the diff as it is
        /// </summary>
        NoSplit,

        /// <summary>
        /// Split on comma characters
        /// </summary>
        Comma,

        /// <summary>
        /// Split on less than characters
        /// </summary>
        LessThan
    }
}