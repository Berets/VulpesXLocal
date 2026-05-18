using System.Globalization;
using System.Windows.Data;

namespace VulpesX.Shared.Controls.Converters
{
    public sealed class TempoIDDescrizioneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                switch ((value).ToString())
                {
                    case (Constants._TEMPOID_INIZIO):
                        return "Inizio produzione";
                    case (Constants._TEMPOID_FINE):
                        return "Fine produzione";
                    case (Constants._TEMPOID_RIPRESA):
                        return "Ripresa";
                    case (Constants._TEMPOID_SOSPENSIONE):
                        return "Sospensione";
                    case (Constants._TEMPOID_VERSAMENTO):
                        return "Versamento";
                    case (Constants._TEMPOID_INIZIOPIAZZAMENTO):
                        return "Inizio piazzamento";
                    case (Constants._TEMPOID_FINEPIAZZAMENTO):
                        return "Fine piazzamento";
                    case (Constants._TEMPOID_COMPLETA):
                        return "Completa";
                    default:
                        return "Non codificata";
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
