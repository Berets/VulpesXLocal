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
    public class TreasuryMovementAvailabilityCellSelector : DataTemplateSelector
    {
        public required DataTemplate DefaultCell { get; set; }
        public required DataTemplate NegativeCell { get; set; }
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var entity = item as RBCC01F0;

            if (entity != null)
            {
                if (entity.DisponibilitaFutura >= 0)
                {
                    return DefaultCell;
                }
                else
                {
                    return NegativeCell;
                }
            }

            return DefaultCell;
        }

    }
}
