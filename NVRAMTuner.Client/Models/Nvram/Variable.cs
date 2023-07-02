namespace NVRAMTuner.Client.Models.Nvram
{
    /// <summary>
    /// Abstract model class representing all items that NVRAM variables share
    /// </summary>
    public abstract class Variable<T> : IVariable
    {
        /// <summary>
        /// Gets or sets the variable's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the variable's value
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the variable's size in bytes
        /// </summary>
        public int SizeBytes { get; set; }

        /// <summary>
        /// Gets or sets a description of the variable
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the default value of the variable as defined by the firmware
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a bool denoting whether or not this variable has special display properties
        /// </summary>
        public bool SpecialVariable { get; set; }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>A string representing this variable</returns>
        public override string ToString()
        {
            return $"Variable: {this.Name}";
        }
    }
}