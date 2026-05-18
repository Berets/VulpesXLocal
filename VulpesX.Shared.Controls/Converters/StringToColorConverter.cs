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
    public class StringToColorConverter : IValueConverter
    {
        public required SolidColorBrush DefaultColor { get; set; }
        public SolidColorBrush? OrangeRedColor { get; set; }
        public SolidColorBrush? GreenColor { get; set; }
        public SolidColorBrush? BlueColor { get; set; }
        public SolidColorBrush? YellowColor { get; set; }
        public SolidColorBrush? RedColor { get; set; }
        public SolidColorBrush? VulpesColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var tipoID = (value != null) ? value.ToString() : "W";

                switch (tipoID)
                {
                    case ("O"):
                        return OrangeRedColor ?? DefaultColor;
                    case ("G"):
                        return GreenColor ?? DefaultColor;
                    case ("V"):
                        return VulpesColor ?? DefaultColor;
                    case ("B"):
                        return BlueColor ?? DefaultColor;
                    case ("Y"):
                        return YellowColor ?? DefaultColor;
                    case ("R"):
                        return RedColor ?? DefaultColor;
                    case ("T"):
                        return new SolidColorBrush(Colors.Transparent);
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
