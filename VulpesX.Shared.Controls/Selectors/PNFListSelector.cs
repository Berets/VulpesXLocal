using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VulpesX.Models.Default.Partials;

namespace VulpesX.Shared.Controls.Selectors
{
    public class PNFListSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var entity = item as SectionalItem;
            if (entity == null)
                return DefaultRowStyle;
            if (string.IsNullOrWhiteSpace(entity.EntityType) || entity.EntityType == "C")
                return DefaultRowStyle;

            if (string.IsNullOrWhiteSpace(entity.LockedInfoText))
            {
                return DefaultRowStyle;
            }
            else
            {
                return LockedRowStyle;
            }
        }

        public required Style LockedRowStyle { get; set; }
        public required Style DefaultRowStyle { get; set; }
    }
}
