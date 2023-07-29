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

        /// <inheritdoc cref="IValueConverter.Convert"/>
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

        /// <inheritdoc cref="IValueConverter.ConvertBack"/>
        /// <exception cref="NotImplementedException">This is not used or implemented</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}