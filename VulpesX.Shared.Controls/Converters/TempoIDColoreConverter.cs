using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VulpesX.Shared.Controls.Converters
{
    public sealed class TempoIDColoreConverter : IValueConverter
    {
        public required SolidColorBrush StartColor { get; set; }
        public required SolidColorBrush StopColor { get; set; }
        public required SolidColorBrush EndColor { get; set; }
        public required SolidColorBrush NeutralColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var tipoID = (value != null) ? value.ToString() : "0";

                if (tipoID == Constants._TEMPOID_INIZIOPIAZZAMENTO || tipoID == Constants._TEMPOID_INIZIO || tipoID == Constants._TEMPOID_RIPRESA || tipoID == Constants._TEMPOID_VERSAMENTO)
                {
                    return StartColor;
                }
                else if (tipoID == Constants._TEMPOID_FINEPIAZZAMENTO || tipoID == Constants._TEMPOID_SOSPENSIONE)
                {
                    return StopColor;
                }
                else if (tipoID == Constants._TEMPOID_FINE)
                {
                    return EndColor;
                }
                else
                {
                    return NeutralColor;
                }
            }
            catch (Exception)
            {
                return StartColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
