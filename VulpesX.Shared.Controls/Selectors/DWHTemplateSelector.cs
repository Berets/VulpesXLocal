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
    public class DWHTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DWH_Template)
            {
                var navigation = item as DWH_Template;

                if (navigation != null)
                {
                    if (navigation.IsTemplate)
                        return SaveTemplate;
                    else
                        return this.FolderTemplate;
                }

                return base.SelectTemplate(item, container);
            }

            return new DataTemplate();
        }

        public required HierarchicalDataTemplate FolderTemplate { get; set; }

        public required HierarchicalDataTemplate SaveTemplate { get; set; }
    }

}
