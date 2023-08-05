namespace NVRAMTuner.Client.Converters.Parameters
{
    using System.Windows;

    /// <summary>
    /// This enumeration holds possible parameters to provide converters that aim to convert between
    /// given types and <see cref="Visibility"/> enumeration values
    ///
    /// If given <see cref="Normal"/>, specific converters will return <see cref="Visibility.Visible"/>
    /// when the input is true, and <see cref="Visibility.Collapsed"/> when the input is false, as expected.
    ///
    ///
    /// If given <see cref="Reverse"/>, specific converters will return <see cref="Visibility.Collapsed"/>
    /// when the input is true, and <see cref="Visibility.Visible"/> when the input is false, the opposite
    /// of the expected behaviour.
    /// </summary>
    public enum ConfigurableVisConverterParams
    {
        /// <summary>
        /// Normal behaviour value
        /// </summary>
        Normal,

        /// <summary>
        /// Reverse behaviour value
        /// </summary>
        Reverse
    }
}