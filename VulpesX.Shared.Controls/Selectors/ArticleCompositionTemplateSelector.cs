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
    public class ArticleCompositionTemplateSelector : DataTemplateSelector
    {
        public required DataTemplate ComponentTemplate { get; set; }

        public required DataTemplate ComponentRootTemplate { get; set; }

        public required DataTemplate StageTemplate { get; set; }

        public required DataTemplate SummaryTemplate { get; set; }

        public required DataTemplate MilestoneTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is tab_articolo_composizione)
            {
                var composizione = (item as tab_articolo_composizione);

                if (composizione != null)
                {
                    if (composizione.ESummary)
                        return SummaryTemplate;

                    if (composizione.EMilestone)
                        return MilestoneTemplate;

                    if (composizione.EReparto)
                        return StageTemplate;
                    else
                        return (composizione.EComponente) ? ComponentTemplate : ComponentRootTemplate;
                }
            }
            return null;
        }

  
    }
}
