﻿namespace NVRAMTuner.Client.Converters
{
    using Parameters;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// A converter that can be used to convert a bool value to a <see cref="Visibility"/> enumeration value.
    /// This converter is configurable such that it can be inverted using a member of <see cref="ConfigurableVisConverterParams"/>
    /// as a ConverterParameter from a XAML binding
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class ConfigurableBoolToVisibilityConverter : IValueConverter
    {
        /// <inheritdoc cref="IValueConverter.Convert"/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                throw new ArgumentNullException();
            }

            bool castVal = (bool)value;

            ConfigurableVisConverterParams mode = 
                (ConfigurableVisConverterParams)Enum.Parse(typeof(ConfigurableVisConverterParams), 
                    (string)parameter);

            if (mode == ConfigurableVisConverterParams.Normal)
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