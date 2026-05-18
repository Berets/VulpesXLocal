using System.Windows.Data;
using System.Windows.Media;

namespace VulpesX.Shared.Controls.Converters
{
    public class TempoIDColoreDefaultConverter : IValueConverter
    {
        public required SolidColorBrush GreenColor { get; set; }
        public required SolidColorBrush OrangeColor { get; set; }
        public required SolidColorBrush YellowColor { get; set; }
        public required SolidColorBrush RedColor { get; set; }
        public required SolidColorBrush BlueColor { get; set; }
        public required SolidColorBrush DefaultColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value.ToString())
            {
                case (Constants._TEMPOID_INIZIO):
                    return GreenColor;
                case (Constants._TEMPOID_SOSPENSIONE):
                    return OrangeColor;
                case (Constants._TEMPOID_RIPRESA):
                    return YellowColor;
                case (Constants._TEMPOID_FINE):
                    return RedColor;
                case (Constants._TEMPOID_INIZIOPIAZZAMENTO):
                    return GreenColor;
                case (Constants._TEMPOID_FINEPIAZZAMENTO):
                    return RedColor;
                case (Constants._TEMPOID_VERSAMENTO):
                    return BlueColor;
                default:
                    return DefaultColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
