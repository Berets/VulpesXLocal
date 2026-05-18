using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VulpesX.Models.Models.Accounting;

namespace VulpesX.Shared.Controls.Selectors
{
    public class CheckInvoiceSelector : StyleSelector
    {
        public required Style Collegato { get; set; }
        public required Style NonCollegato { get; set; }
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var dettaglio = item as CheckInvoiceEntranceModel.DettaglioModel;

            if (dettaglio != null)
            {
                if (dettaglio.Entrata != null)
                    return Collegato;
                else
                    return NonCollegato;
            }

            return base.SelectStyle(item, container);
        }
    }
}
