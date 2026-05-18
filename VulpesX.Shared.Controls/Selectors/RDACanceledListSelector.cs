using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VulpesX.Shared.Controls.Selectors
{
    public class RDACanceledListSelector : StyleSelector
    {
        public required Style CanceledRowStyle { get; set; }
        public required Style DefaultRowStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var entity = item as dynamic;
            if (entity == null)
                return DefaultRowStyle;
            if (entity.canceled != null)
                return CanceledRowStyle;
            else
                return DefaultRowStyle;
        }

     
    }
}
