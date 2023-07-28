namespace NVRAMTuner.Client.Validators
{
    using Services;
    using Services.Network;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Custom validation annotation for determining if a value is a valid IPv4 address.
    /// Uses <see cref="NetworkService.VerifyIpv4Address"/> to verify the value.
    /// </summary>
    public sealed class ValidIpv4AddressAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates the value
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="validationContext">The validation context</param>
        /// <returns>A <see cref="ValidationResult"/> instance</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return NetworkService.VerifyIpv4Address((string)value)
                ? ValidationResult.Success
                : new ValidationResult("Not a valid IP address");
        }
    }
}