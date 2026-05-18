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
    public class BoolToColorConverter : IValueConverter
    {
        public required SolidColorBrush DefaultColor { get; set; }
        public SolidColorBrush? TrueColor { get; set; }
        public SolidColorBrush? FalseColor { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is bool)
                {
                    if ((bool)value)
                    {
                        return TrueColor ?? DefaultColor;
                    }
                    else
                    {
                        return FalseColor ?? DefaultColor;
                    }
                }

                return DefaultColor;
            }
            catch (Exception)
            {
                return DefaultColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
