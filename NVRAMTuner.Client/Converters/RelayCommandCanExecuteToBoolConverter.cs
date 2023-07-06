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
    public class RelayCommandCanExecuteToBoolConverter : IValueConverter
    {
        /// <summary>Converts between a command's execution ability and a boolean</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IRelayCommand command)
            {
                return command.CanExecute(null);
            }

            return false;
        }

        /// <summary>Converts a value. </summary>
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