namespace NVRAMTuner.Client.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// A converter that can be used to convert an int value to a <see cref="Colors"/> value.
    /// The conversion is based on the sign of the value passed in.
    ///
    /// A positive sign results in <see cref="Colors.Orange"/>, and a negative sign in <see cref="Colors.Green"/>.
    /// Zero results in <see cref="Colors.Transparent"/>.
    ///
    /// This is used in the context of growths/reductions in overall NVRAM size. Increases are highlighted in orange,
    /// and reductions in green.
    /// </summary>
    [ValueConversion(typeof(int), typeof(Colors))]
    public class IntSignToSolidColourBrushConverter : IValueConverter
    {
        /// <summary>
        /// <inheritdoc cref="IValueConverter.Convert"/>
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            int castVal = (int)value;

            if (castVal == 0)
            {
                return Colors.Transparent;
            }

            return castVal >= 1 ? Colors.Orange : Colors.Green;
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