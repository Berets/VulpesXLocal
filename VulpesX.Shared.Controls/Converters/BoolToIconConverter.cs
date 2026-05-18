using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace VulpesX.Shared.Controls.Converters
{
    public class BoolToIconConverter : IValueConverter
    {
        public required PathGeometry TrueIcon { get; set; }
        public required PathGeometry FalseIcon { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return ((bool)value) ? TrueIcon : FalseIcon;
            }
            else
            {
                return FalseIcon;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return FalseIcon;
        }
    }
}
