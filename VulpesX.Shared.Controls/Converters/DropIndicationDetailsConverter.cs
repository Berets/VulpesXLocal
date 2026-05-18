using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace VulpesX.Shared.Controls.Converters
{
    public class DropIndicationDetailsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DropPosition)
            {
                switch ((DropPosition)value)
                {
                    case DropPosition.Before:
                        return "Prima";
                    case DropPosition.Inside:
                        return "Dentro";
                    case DropPosition.After:
                        return "Dopo";
                    case DropPosition.Undefined:
                        return "Indefinito";
                    default:
                        return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
