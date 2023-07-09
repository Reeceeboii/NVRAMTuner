namespace NVRAMTuner.Client.Models.Nvram
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Model representing a set of variables retrieved from a router
    /// </summary>
    public class Nvram
    {
        /// <summary>
        /// Gets or sets an <see cref="IEnumerable{T}"/> of <see cref="IVariable"/> instances
        /// instances
        /// </summary>
        public IEnumerable<IVariable> Variables { get; set; }

        /// <summary>
        /// Gets or sets the total size in bytes of the router's NVRAM
        /// </summary>
        public int TotalSizeBytes { get; set; }

        /// <summary>
        /// Gets or sets the remaining size in bytes of the router's NVRAM
        /// </summary>
        public int RemainingSizeBytes { get; set; }

        /// <summary>
        /// Gets or sets the size of the variables in bytes
        /// </summary>
        public int VariableSizeBytes { get; set; }

        /// <summary>
        /// Gets or sets the time at which this set of variables was retrieved
        /// </summary>
        public DateTime RetrievedAt { get; set; }
    }
}