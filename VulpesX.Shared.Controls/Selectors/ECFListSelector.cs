using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VulpesX.Models.Models.Reports.Accounting;
using static VulpesX.Models.Models.Reports.Accounting.MastrinoReport;

namespace VulpesX.Shared.Controls.Selectors
{
    public class ECFListSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var entity = item as MastrinoECReportItem;
            if (entity == null)
                return DefaultRowStyle;
            if (entity.EntityType == "C")
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
