
using System.Windows;
using Telerik.Windows.Controls;

namespace VulpesX.Shared.Controls.Gantt
{
    public class GanttExpanderHelper
    {
        static RadGanttView? myGantt;
        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(GanttExpanderHelper), new PropertyMetadata(OnIsEnabledChanged));

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                myGantt = d as RadGanttView;

                if (myGantt != null)
                {
                    myGantt.ExpandCollapseService.HierarchicalCollectionAdapter.CollectionChanged -= HierarchicalCollectionAdapter_CollectionChanged;
                    myGantt.ExpandCollapseService.HierarchicalCollectionAdapter.CollectionChanged += HierarchicalCollectionAdapter_CollectionChanged;
                }
            }
        }

        static void HierarchicalCollectionAdapter_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs? e)
        {
            //if (e.OldItems != null)
            //    (myGantt.DataContext as GanttListViewModel).ExpandeCollapseEvent(e.OldItems, false);
            //if (e.NewItems != null)
            //    (myGantt.DataContext as GanttListViewModel).ExpandeCollapseEvent(e.NewItems, true);
        }
    }
}
