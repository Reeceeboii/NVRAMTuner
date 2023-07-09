namespace NVRAMTuner.Client.Models.Nvram
{
    /// <summary>
    /// Interface for the highest level representation of an NVRAM variable
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// Gets or sets the variable's name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the variable's original value
        /// </summary>
        string OriginalValue { get; set; }

        /// <summary>
        /// Gets or sets a delta (change) applied to the original value
        /// </summary>
        string ValueDelta { get; set; }

        /// <summary>
        /// Gets or sets the variable's size in bytes
        /// </summary>
        int SizeBytes { get; set; }

        /// <summary>
        /// Gets or sets a description of the variable
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the default value of the variable as defined by the firmware
        /// </summary>
        string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a bool denoting whether or not this variable has special display properties
        /// </summary>
        bool SpecialVariable { get; set; }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>A string representing this variable</returns>
        string ToString();
    }
}