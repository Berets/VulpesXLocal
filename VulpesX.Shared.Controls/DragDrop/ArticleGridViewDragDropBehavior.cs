using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.DragDrop;
using Telerik.Windows.DragDrop.Behaviors;

namespace VulpesX.Shared.Controls.DragDrop
{
    public class ArticleGridViewDragDropBehavior
    {
        private RadGridView? _associatedObject;
        public RadGridView? AssociatedObject
        {
            get
            {
                return _associatedObject;
            }
            set
            {
                _associatedObject = value;
            }
        }

        private static Dictionary<RadGridView, ArticleGridViewDragDropBehavior> instancesBehavior;

        static ArticleGridViewDragDropBehavior()
        {
            instancesBehavior = new Dictionary<RadGridView, ArticleGridViewDragDropBehavior>();
        }

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            ArticleGridViewDragDropBehavior? behavior = GetAttachedBehavior(obj as RadGridView);

            if (behavior != null)
            {
                behavior.AssociatedObject = obj as RadGridView;

                if (value)
                {
                    behavior.Initialize();
                }
                else
                {
                    behavior.CleanUp();
                }
                obj.SetValue(IsEnabledProperty, value);
            }
        }

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(GridViewDragDropBehavior), new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

        public static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            SetIsEnabled(dependencyObject, (bool)e.NewValue);
        }

        private static ArticleGridViewDragDropBehavior? GetAttachedBehavior(RadGridView? listBox)
        {
            if (listBox != null)
            {
                if (!instancesBehavior.ContainsKey(listBox))
                {
                    instancesBehavior[listBox] = new ArticleGridViewDragDropBehavior();
                    instancesBehavior[listBox].AssociatedObject = listBox;
                }

                return instancesBehavior[listBox];
            }

            return null;
        }

        protected virtual void Initialize()
        {
            this.UnsubscribeFromDragDropEvents();
            this.SubscribeToDragDropEvents();
        }

        protected virtual void CleanUp()
        {
            this.UnsubscribeFromDragDropEvents();
        }

        private void SubscribeToDragDropEvents()
        {
            DragDropManager.AddDragInitializeHandler(this.AssociatedObject, OnDragInitialize);
            DragDropManager.AddGiveFeedbackHandler(this.AssociatedObject, OnGiveFeedback);
            DragDropManager.AddDropHandler(this.AssociatedObject, OnDrop);
            DragDropManager.AddDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted);
            DragDropManager.AddDragOverHandler(this.AssociatedObject, OnDragOver);
        }

        private void UnsubscribeFromDragDropEvents()
        {
            DragDropManager.RemoveDragInitializeHandler(this.AssociatedObject, OnDragInitialize);
            DragDropManager.RemoveGiveFeedbackHandler(this.AssociatedObject, OnGiveFeedback);
            DragDropManager.RemoveDropHandler(this.AssociatedObject, OnDrop);
            DragDropManager.RemoveDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted);
            DragDropManager.RemoveDragOverHandler(this.AssociatedObject, OnDragOver);
        }

        private void OnDragInitialize(object sender, DragInitializeEventArgs e)
        {
            DropIndicationDetails details = new DropIndicationDetails();
            var listBoxItem = e.OriginalSource as GridViewRowItem ?? (e.OriginalSource as FrameworkElement).ParentOfType<GridViewRowItem>();

            var item = listBoxItem != null ? listBoxItem.DataContext : (sender as RadGridView)?.SelectedItem;

            //if (item is tab_articolo)
            //    item = new ArticoloComposizioneService().Single((item as tab_articolo).SocietaID, (item as tab_articolo).ID, null);

            details.CurrentDraggedItem = item;

            IDragPayload dragPayload = DragDropPayloadManager.GeneratePayload(null);

            dragPayload.SetData("DraggedData", item);
            dragPayload.SetData("DropDetails", details);

            e.Data = dragPayload;

            e.DragVisual = new Telerik.Windows.DragDrop.DragVisual()
            {
                Content = details,
                ContentTemplate = this.AssociatedObject?.Resources["DraggedItemTemplate"] as DataTemplate
            };
            e.DragVisualOffset = e.RelativeStartPoint;
            e.AllowedEffects = DragDropEffects.All;
        }

        private void OnGiveFeedback(object sender, Telerik.Windows.DragDrop.GiveFeedbackEventArgs e)
        {
            e.SetCursor(Cursors.Arrow);
            e.Handled = true;
        }

        private void OnDragDropCompleted(object sender, DragDropCompletedEventArgs e)
        {
            var draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");

            if (e.Effects != DragDropEffects.None)
            {
            }
        }

        private void OnDrop(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            var draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");
            var details = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DropIndicationDetails;
            var itemsType = (this.AssociatedObject?.ItemsSource as IList)?.AsQueryable().ElementType;

            if (details == null || draggedItem == null || draggedItem.GetType() != itemsType)
            {
                return;
            }

            if (e.Effects != DragDropEffects.None)
            {
                var collection = (sender as RadGridView)?.ItemsSource as IList;

                if (collection != null)
                    collection.Add(draggedItem);
            }
        }

        private void OnDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            var draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");

            var itemsType = (this.AssociatedObject?.ItemsSource as IList)?.AsQueryable().ElementType;

            e.Effects = DragDropEffects.None;

            var dropDetails = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DropIndicationDetails;

            if (dropDetails != null)
            {
                dropDetails.CurrentDraggedOverItem = this.AssociatedObject;
                dropDetails.CurrentDropPosition = DropPosition.Inside;
            }

            e.Handled = true;
        }
    }
}
