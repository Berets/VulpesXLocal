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
    public class AFComplexityToColor : IValueConverter
    {
        public required SolidColorBrush DefaultColor { get; set; }
        public SolidColorBrush? StandardColor { get; set; }
        public SolidColorBrush? MediaColor { get; set; }
        public SolidColorBrush? ComplessaColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var tipoID = (value != null) ? value.ToString() : "W";

                switch (tipoID)
                {
                    case ("S"):
                        return StandardColor ?? DefaultColor;
                    case ("M"):
                        return MediaColor ?? DefaultColor;
                    case ("C"):
                        return ComplessaColor ?? DefaultColor;
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
