using System.Windows;
using Telerik.Windows.DragDrop;
using Telerik.Windows.DragDrop.Behaviors;

namespace VulpesX.Shared.Controls.DragDrop;

public class DragVisualProvider : DependencyObject, IDragVisualProvider
{
    public DataTemplate DraggedItemTemplate
    {
        get
        {
            return (DataTemplate)GetValue(DraggedItemTemplateProperty);
        }
        set
        {
            SetValue(DraggedItemTemplateProperty, value);
        }
    }

    public static readonly DependencyProperty DraggedItemTemplateProperty =
    DependencyProperty.Register("DraggedItemTemplate", typeof(DataTemplate), typeof(DragVisualProvider), new PropertyMetadata(null));

    public FrameworkElement CreateDragVisual(DragVisualProviderState state)
    {
        var visual = new Telerik.Windows.DragDrop.DragVisual();
        visual.Content = state.DraggedItems.OfType<object>().FirstOrDefault();
        visual.ContentTemplate = this.DraggedItemTemplate;

        return visual;
    }

    public Point GetDragVisualOffset(DragVisualProviderState state)
    {
        return state.RelativeStartPoint;
    }

    public bool UseDefaultCursors { get; set; }
}
