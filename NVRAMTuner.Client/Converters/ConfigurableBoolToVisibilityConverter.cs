namespace NVRAMTuner.Client.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// A converter that can be used to convert a bool value to a <see cref="Visibility"/> enumeration value.
    /// This converter is configurable such that it can be inverted using a member of <see cref="ConvParams"/>
    /// as a ConverterParameter from a XAML binding
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class ConfigurableBoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Enumeration of possible states for this converter to use:
        ///     <see cref="Normal"/>: true = <see cref="Visibility.Visible"/>, false = <see cref="Visibility.Collapsed"/>
        ///     <see cref="Reverse"/>: true = <see cref="Visibility.Collapsed"/>, false = <see cref="Visibility.Visible"/>
        /// </summary>
        public enum ConvParams
        {
            /// <summary>
            /// The normal converter operation
            /// </summary>
            Normal,

            /// <summary>
            /// The reverse converter operation
            /// </summary>
            Reverse
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                throw new ArgumentNullException();
            }

            bool castVal = (bool)value;
            ConvParams mode = (ConvParams)Enum.Parse(typeof(ConvParams), (string)parameter);

            if (mode == ConvParams.Normal)
            {
                return castVal ? Visibility.Visible : Visibility.Collapsed;
            }

            return castVal ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">This is not implemented</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}