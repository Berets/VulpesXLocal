using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VulpesX.Models.Models.CRM;

namespace VulpesX.Shared.Controls.Selectors
{
    public class FATTT00FTrendCellSelector : DataTemplateSelector
    {
        public required DataTemplate NegativeRowStyle { get; set; }
        public required DataTemplate NeutroRowStyle { get; set; }
        public required DataTemplate PositiveRowStyle { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is InvoicingTrendMonth)
            {
                var entity = item as InvoicingTrendMonth;

                if (entity != null)
                {
                    if (entity.TrendPercentage == 0)
                    {
                        return NeutroRowStyle;
                    }
                    else
                    {
                        if (entity.TrendPercentage > 0)
                            return PositiveRowStyle;
                        else
                            return NegativeRowStyle;
                    }
                }
            }
            return NeutroRowStyle;
        }
    }
}
