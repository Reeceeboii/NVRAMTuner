namespace NVRAMTuner.Client.Validators
{
    using Services;
    using Services.Network;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Custom validation annotation for determining if a value is a valid network port.
    /// Uses <see cref="NetworkService.VerifyNetworkPort(int)"/> to verify the value once
    /// it has been ascertained that the value itself is actually a valid int value
    /// </summary>
    public sealed class ValidNetworkPortAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates the value
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="validationContext">The validation context</param>
        /// <returns>A <see cref="ValidationResult"/> instance</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (int.TryParse((string)value, out int parsed))
            {
                return NetworkService.VerifyNetworkPort(parsed)
                    ? ValidationResult.Success
                    : new ValidationResult("Not a valid network port");
            }

            return new ValidationResult("Not a valid entry");
        }
    }
}