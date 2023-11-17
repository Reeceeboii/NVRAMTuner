namespace NVRAMTuner.Client.Converters
{
    using Parameters;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// A converter that can be used to convert an int value to a <see cref="Visibility"/> enumeration value.
    /// In normal operation, if the int is >=1, <see cref="Visibility.Visible"/> is returned, else,
    /// <see cref="Visibility.Collapsed"/> is returned.
    ///
    /// This converter is configurable such that it can be inverted using a member of <see cref="ConfigurableVisConverterParams"/>
    /// as a ConverterParameter from a XAML binding
    /// </summary>
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class ConfigurableIntToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// <inheritdoc cref="IValueConverter.Convert"/>
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                throw new ArgumentNullException();
            }

            int castVal = (int)value;

            ConfigurableVisConverterParams mode =
                (ConfigurableVisConverterParams)Enum.Parse(typeof(ConfigurableVisConverterParams),
                    (string)parameter);

            if (mode == ConfigurableVisConverterParams.Normal)
            {
                return castVal >= 1 ? Visibility.Visible : Visibility.Collapsed;
            }

            return castVal >= 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// <inheritdoc cref="IValueConverter.ConvertBack"/>
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}