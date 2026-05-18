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
    public class LaunchRawListSelector : StyleSelector
    {
        public required Style ReadyRowStyle { get; set; }
        public required Style PartialRowStyle { get; set; }
        public required Style MissingRowStyle { get; set; }
        public required Style AvailableRowStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is pro_ordine_composizione)
            {
                var entity = item as pro_ordine_composizione;

                if (entity != null)
                {
                    if (entity.QuantitaImpegnata + entity.QuantitaGiaImpegnata == entity.Quantita)
                    {
                        return ReadyRowStyle;
                    }
                    else
                    {
                        if (entity.QuantitaImpegnata + entity.QuantitaGiaImpegnata < entity.Quantita &&
                            entity.QuantitaImpegnata + entity.QuantitaGiaImpegnata > 0)
                        {
                            return PartialRowStyle;
                        }
                        else
                        {
                            if (entity.Availabilities != null && entity.Availabilities.Count > 0)
                                return AvailableRowStyle;
                            else
                                return MissingRowStyle;
                        }
                    }
                }
            }
            return MissingRowStyle;
        }

  
    }
}
