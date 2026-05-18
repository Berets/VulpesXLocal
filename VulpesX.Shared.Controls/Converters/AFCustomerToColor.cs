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
    public class AFCustomerToColor : IValueConverter
    {
        public required SolidColorBrush DefaultColor { get; set; }
        public SolidColorBrush? ICOColor { get; set; }
        public SolidColorBrush? DirectColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var tipoID = (value != null) ? value.ToString() : "W";

                switch (tipoID)
                {
                    case ("I"):
                        return ICOColor ?? DefaultColor;
                    case ("D"):
                        return DirectColor ?? DefaultColor;
                    default:
                        return DefaultColor;
                }
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
