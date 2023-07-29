namespace NVRAMTuner.Client.Converters
{
    using CommunityToolkit.Mvvm.Input;
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Input;

    /// <summary>
    /// Converter to convert between an <see cref="IRelayCommand"/>'s <see cref="ICommand.CanExecute"/> method
    /// and a boolean
    /// </summary>
    [ValueConversion(typeof(IRelayCommand), typeof(bool))]
    public class RelayCommandCanExecuteToBoolConverter : IValueConverter
    {
        /// <inheritdoc cref="IValueConverter.Convert"/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IRelayCommand command)
            {
                return command.CanExecute(null);
            }

            return false;
        }

        /// <inheritdoc cref="IValueConverter.ConvertBack"/>
        /// <exception cref="NotImplementedException">This is not used or implemented</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}