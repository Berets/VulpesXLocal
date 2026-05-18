using Microsoft.Extensions.DependencyInjection;
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
    public class TreasuryCommitmentDateCellSelector : DataTemplateSelector
    {
        public required DataTemplate DefaultCell { get; set; }
        public required DataTemplate ExpiredCell { get; set; }
        public required DataTemplate ExpiringCell { get; set; }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item is TES_IMFI)
            {
                var entity = item as TES_IMFI;

                if (entity != null)
                {
                    if (entity.ifdata < DateTime.Now.Date)
                    {
                        return ExpiredCell;
                    }
                    else
                    {
                        if (entity.ifdata == DateTime.Now.Date)
                            return ExpiringCell;
                        else
                            return DefaultCell;
                    }
                }
            }
            return DefaultCell;
        }
    }
}
