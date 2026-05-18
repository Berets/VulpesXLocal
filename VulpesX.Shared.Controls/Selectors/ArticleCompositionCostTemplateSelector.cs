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
    public class ArticleCompositionCostTemplateSelector : DataTemplateSelector
    {

        public required DataTemplate ComponentHeadCostTemplate { get; set; }

        public required DataTemplate ComponentWardCostTemplate { get; set; }

        public required DataTemplate ComponentCostTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is tab_articolo_composizione)
            {
                var composizione = (item as tab_articolo_composizione);

                if (composizione != null)
                {
                    if (composizione.EMilestone)
                        return ComponentHeadCostTemplate;

                    if (composizione.EReparto)
                        return ComponentWardCostTemplate;
                    else
                        return (composizione.EComponente) ? ComponentCostTemplate : ComponentHeadCostTemplate;
                }
            }
            return null;
        }


    }
}
