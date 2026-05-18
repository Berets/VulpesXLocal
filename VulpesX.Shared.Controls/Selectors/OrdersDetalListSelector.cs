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
    public class OrdersDetalListSelector : StyleSelector
    {
        public required Style DefaultRowStyle { get; set; }
        public Style? CanceledRowStyle { get; set; }
        public Style? PartialRowStyle { get; set; }
        public Style? NotReadyRowStyle { get; set; }
        public Style? ReadyRowStyle { get; set; }
        public Style? WaitingRowStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var entity = item as ORDID00F;

            if (entity == null)
                return DefaultRowStyle;

            if (entity.ODSTA == "#")
            {
                return ReadyRowStyle ?? DefaultRowStyle;
            }
            else if (entity.ODSTA == "?")
            {
                return PartialRowStyle ?? DefaultRowStyle;
            }

            if ((entity.LinkedProductionOrder?.LogCanceled.HasValue ?? false) && entity.LinkedProductionOrder?.Stato == "X")
            {
                return CanceledRowStyle ?? DefaultRowStyle;
            }
            else
            {
                if (entity.LinkedProductionOrder?.Stato == "A" || entity.LinkedProductionOrder?.Stato == "O" ||
                    entity.LinkedProductionOrder?.Stato == "R")
                {
                    return NotReadyRowStyle ?? DefaultRowStyle;
                }
                else
                {
                    if (entity.LinkedProductionOrder?.Stato == "S")
                    {
                        return PartialRowStyle ?? DefaultRowStyle;
                    }
                    else
                    {
                        if (entity.LinkedProductionOrder?.Stato == "E")
                            return ReadyRowStyle ?? DefaultRowStyle;
                        else if (entity.LinkedProductionOrder?.Stato == "W")
                            return WaitingRowStyle ?? DefaultRowStyle;
                    }
                }
                return DefaultRowStyle;
            }
        }



    }
}
