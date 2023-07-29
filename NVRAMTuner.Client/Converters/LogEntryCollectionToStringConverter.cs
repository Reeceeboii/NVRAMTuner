namespace NVRAMTuner.Client.Converters
{
    using Models;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using Utils;

    /// <summary>
    /// A multi value converter that takes an <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/> instances
    /// and uses the <see cref="StringUtils.LogEntryCollectionToString"/> method to convert all of them to a string
    /// </summary>
    [ValueConversion(typeof(ObservableCollection<LogEntry>), typeof(string))]
    public class LogEntryCollectionToStringConverter : IMultiValueConverter
    {
        /// <inheritdoc cref="IMultiValueConverter.Convert"/>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is ObservableCollection<LogEntry> cln) || !cln.Any())
            {
                return string.Empty;
            }

            return StringUtils.LogEntryCollectionToString(cln);
        }

        /// <inheritdoc cref="IMultiValueConverter.ConvertBack"/>
        /// <exception cref="NotImplementedException">This is not used or implemented</exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}