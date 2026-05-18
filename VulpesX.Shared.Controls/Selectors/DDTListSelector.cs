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
    public class DDTListSelector : StyleSelector
    {
        public required Style DefaultRowStyle { get; set; }
        public Style? CanceledRowStyle { get; set; }
        public Style? PartialRowStyle { get; set; }
        public Style? NotReadyRowStyle { get; set; }
        public Style? NotSyncRowStyle { get; set; }
        public Style? ReadyRowStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var entity = item as BOLLT00F;

            if (entity != null)
            {
                if (entity.canceled != null)
                {
                    return CanceledRowStyle ?? DefaultRowStyle;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(entity.BTSTATO))
                    {
                        var result = DDTListSelectorHandler.Call(entity.bolsoc, entity.BTANNO, entity.BTBOLL);

                        if (result == "N")
                            return NotReadyRowStyle ?? DefaultRowStyle;

                        if (result == "P")
                        {
                            return PartialRowStyle ?? DefaultRowStyle;
                        }
                        if (result == "S")
                        {
                            return NotSyncRowStyle ?? DefaultRowStyle;
                        }

                        return NotReadyRowStyle ?? DefaultRowStyle;
                    }
                }
            }

            return DefaultRowStyle;
        }
    }
}
