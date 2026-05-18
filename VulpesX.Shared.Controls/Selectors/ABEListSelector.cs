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
    public class ABEListSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var entity = item as ABE;
            if (entity == null)
                return DefaultRowStyle;

            if (entity.IsObsolete != null)
            {
                return ObsoleteRowStyle ?? DefaultRowStyle;
            }
            else
            {
                if (entity.abecfe == "P")
                {
                    return ProspectRowStyle ?? DefaultRowStyle;
                }
                else
                {
                    if(entity.IsTop)
                        return TopRowStyle ?? DefaultRowStyle;

                    return DefaultRowStyle;
                }
            }
        }

        public Style? ObsoleteRowStyle { get; set; }
        public Style? ProspectRowStyle { get; set; }
        public Style? TopRowStyle { get; set; }
        public required Style DefaultRowStyle { get; set; }
    }
}
