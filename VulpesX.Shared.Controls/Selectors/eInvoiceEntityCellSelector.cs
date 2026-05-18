using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Selectors
{
    public class eInvoiceEntityCellSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is ACC_EINVOICE_HEADS)
            {
                var entity = item as ACC_EINVOICE_HEADS;

                if (entity != null)
                {
                    if (container is GridViewCell)
                    {
                        if ((container as GridViewCell)?.Column.Tag.ToString() == "supplier")
                        {
                            if (entity.fattfor.HasValue)
                                return SupplierStandardStyle;
                            else
                                return SupplierCreateStyle;
                        }
                        else if ((container as GridViewCell)?.Column.Tag.ToString() == "customer")
                        {
                            if (entity.fattcli.HasValue)
                                return CustomerStandardStyle;
                            else
                                return CustomerCreateStyle;
                        }
                    }
                }
            }
            return null;
        }

        public DataTemplate? SupplierStandardStyle { get; set; }
        public DataTemplate? SupplierCreateStyle { get; set; }
        public DataTemplate? CustomerStandardStyle { get; set; }
        public DataTemplate? CustomerCreateStyle { get; set; }
    }
}
