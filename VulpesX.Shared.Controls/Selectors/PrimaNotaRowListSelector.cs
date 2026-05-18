using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Selectors
{
    public class PrimaNotaRowListSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is PNRIGHE)
            {
                var entity = item as PNRIGHE;
                if (entity == null)
                    return DefaultRowStyle;
                switch (entity.N1tmpPNR)
                {
                    case "S":
                        return TemporaryRowStyle;
                    default:
                        return DefaultRowStyle;
                }
            }
            return DefaultRowStyle;
        }

        public required Style TemporaryRowStyle { get; set; }
        public required Style DefaultRowStyle { get; set; }
    }
}
