using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Selectors
{
    public class BoxTemplateSelector : DataTemplateSelector
    {
        public required DataTemplate Box { get; set; }
        public required DataTemplate NoBox { get; set; }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if ((container as GridViewCell)?.DataContext is pro_ordine_composizione_tempo)
            {
                var dataContext = (container as GridViewCell)?.DataContext as pro_ordine_composizione_tempo;

                if (dataContext != null)
                {
                    if ((dataContext.Box ?? 0) > 0)
                    {
                        return Box;
                    }
                    else
                    {
                        return NoBox;
                    }
                }
                else
                    return NoBox;
            }
            else
                return NoBox;
        }

 
    }
}
