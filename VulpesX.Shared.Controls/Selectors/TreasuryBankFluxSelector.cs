using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VulpesX.Models.Models.Reports.Accounting;

namespace VulpesX.Shared.Controls.Selectors
{
    public class TreasuryBankFluxSelector : StyleSelector
    {
        public required Style TemporaryRowStyle { get; set; }
        public required Style DefaultRowStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is BankFluxMonthItem)
            {
                var entity = item as BankFluxMonthItem;
                if (entity == null)
                    return DefaultRowStyle;
                else
                    if (entity.IsTemporary)
                    return TemporaryRowStyle;
            }
            return DefaultRowStyle;
        }
    }
}
