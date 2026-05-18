using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VulpesX.Models.Default;
using VulpesX.Shared.Utilities;

namespace VulpesX.Shared.Controls.Selectors
{
    public class OrdersListSelector : StyleSelector
    {
        public required Style DefaultRowStyle { get; set; }
        public Style? CanceledRowStyle { get; set; }
        public Style? PartialRowStyle { get; set; }
        public Style? NotReadyRowStyle { get; set; }
        public Style? ReadyRowStyle { get; set; }


        public override Style SelectStyle(object item, DependencyObject container)
        {
            var entity = item as ORDIT00F;
            if (entity == null)
                return DefaultRowStyle;

            if (entity.canceled != null)
            {
                return CanceledRowStyle ?? DefaultRowStyle;
            }
            else
            {
                if (entity.flgchi == "F")
                {
                    if (entity.ProductionOrdersNeeded == 0)
                    {
                        if ((entity.Rows ?? new()).Any(any => any.ODSTA == "?"))
                            return PartialRowStyle ?? DefaultRowStyle;
                        else
                            return ReadyRowStyle ?? DefaultRowStyle;
                    }
                    else
                    {
                        var result = OrdersListSelectorHandler.Call(entity.otsoci, entity.OTANNO, entity.OTNUOR, entity.ProductionOrdersNeeded);

                        if (result == "N")
                            return NotReadyRowStyle ?? DefaultRowStyle;

                        if (result == "P")
                        {
                            return PartialRowStyle ?? DefaultRowStyle;
                        }
                        if (result == "R")
                        {
                            return ReadyRowStyle ?? DefaultRowStyle;
                        }
                    }
                }
                return DefaultRowStyle;
            }
        }
    }
}
