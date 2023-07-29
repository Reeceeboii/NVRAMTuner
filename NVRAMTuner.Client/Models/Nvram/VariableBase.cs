namespace NVRAMTuner.Client.Models.Nvram
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using System;

    /// <summary>
    /// Abstract model class representing all items that NVRAM variables share
    /// </summary>
    public abstract class VariableBase<T> : ObservableObject, IVariable
    {
        /// <summary>
        /// Backing field for <see cref="ValueDelta"/>
        /// </summary>
        private string valueDelta;

        /// <summary>
        /// Backing field for <see cref="SizeBytes"/>
        /// </summary>
        private int sizeBytes;

        /// <summary>
        /// Event raised when the <see cref="ValueDelta"/> is changed
        /// </summary>
        public event EventHandler ValueDeltaChanged;

        /// <summary>
        /// Gets or sets the variable's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the variable's original value
        /// </summary>
        public string OriginalValue { get; set; }

        /// <summary>
        /// Gets or sets a delta (change) applied to the original value
        /// </summary>
        public string ValueDelta
        {
            get => this.valueDelta;
            set
            {
                this.SetProperty(ref this.valueDelta, value);
                this.SizeBytes = this.valueDelta.Length;
                this.ValueDeltaChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the variable's value
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the variable's size in bytes
        /// </summary>
        public int SizeBytes
        {
            get => this.sizeBytes;
            set => this.SetProperty(ref this.sizeBytes, value);
        }

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