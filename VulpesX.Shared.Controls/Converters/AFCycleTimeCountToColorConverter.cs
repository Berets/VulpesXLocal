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
    public class AFCycleTimeCountToColorConverter : IValueConverter
    {
        public required SolidColorBrush DefaultColor { get; set; }
        public SolidColorBrush? ZeroColor { get; set; }
        public SolidColorBrush? OneColor { get; set; }
        public SolidColorBrush? ManyColor { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int valueInt = (int)value;

                if (valueInt == 0)
                    return ZeroColor ?? DefaultColor;

                if (valueInt == 1)
                    return OneColor ?? DefaultColor;

                if (valueInt > 1)
                    return ManyColor ?? DefaultColor;

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
