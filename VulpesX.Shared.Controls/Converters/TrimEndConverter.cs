using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VulpesX.Shared.Controls.Converters
{
    public class TrimEndConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value as string)?.TrimEnd();

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (value as string)?.TrimEnd();
    }
}
