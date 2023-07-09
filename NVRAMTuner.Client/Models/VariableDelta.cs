namespace NVRAMTuner.Client.Models
{
    using Nvram;
    using System;

    /// <summary>
    /// Model representing a delta (or change) between the state of a variable.
    /// These models are staged before being committed to NVRAM on the router
    /// </summary>
    public class VariableDelta
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VariableDelta"/> class
        /// </summary>
        /// <param name="original">A copy of the original variable</param>
        /// <param name="delta">The variable with any changes applied</param>
        /// <exception cref="InvalidOperationException">If the 2 variables that implement
        /// <see cref="IVariable"/> are not of the same concrete type</exception>
        public VariableDelta(IVariable original, IVariable delta)
        {
            if (original.GetType() != delta.GetType())
            {
                throw new InvalidOperationException($"Concrete types provided to {nameof(VariableDelta)} must match");
            }

            this.Original = original;
            this.Delta = delta;
        }

        /// <summary>
        /// Gets or privately sets the original variable
        /// </summary>
        public IVariable Original { get; private set; }

        /// <summary>
        /// Gets or privately sets the variable's delta
        /// </summary>
        public IVariable Delta { get; private set; }
    }
}
