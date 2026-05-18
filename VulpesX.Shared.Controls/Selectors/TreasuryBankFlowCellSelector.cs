using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using VulpesX.Models.Models.Reports.Accounting;

namespace VulpesX.Shared.Controls.Selectors
{
    public class TreasuryBankFlowCellSelector : DataTemplateSelector
    {
        public required DataTemplate DefaultCell10 { get; set; }
        public required DataTemplate DefaultCell20 { get; set; }
        public required DataTemplate DefaultCell30 { get; set; }
        public required DataTemplate TemporaryDetailsCell10 { get; set; }
        public required DataTemplate TemporaryDetailsCell20 { get; set; }
        public required DataTemplate TemporaryDetailsCell30 { get; set; }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item is BankFluxMonthItem)
            {
                var entity = item as BankFluxMonthItem;

                if (entity != null)
                {
                    if (entity.IsTemporary)
                    {
                        switch ((container as GridViewCell)?.Column.Tag.ToString())
                        {
                            case "10": return TemporaryDetailsCell10;
                            case "20": return TemporaryDetailsCell20;
                            case "30": return TemporaryDetailsCell30;
                        }
                    }
                    else
                    {
                        switch ((container as GridViewCell)?.Column.Tag.ToString())
                        {
                            case "10": return DefaultCell10;
                            case "20": return DefaultCell20;
                            case "30": return DefaultCell30;
                        }
                    }
                }
            }
            return DefaultCell10;
        }

    }
}
