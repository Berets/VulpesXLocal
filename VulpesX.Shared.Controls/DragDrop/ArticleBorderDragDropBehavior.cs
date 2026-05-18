using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.DragDrop;
using Telerik.Windows.DragDrop.Behaviors;
using VulpesX.Shared.Controls.Selectors;

namespace VulpesX.Shared.Controls.DragDrop
{
    public class ArticleBorderDragDropBehavior
    {
        private Border? _associatedObject;
        public Border? AssociatedObject
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

        private static Dictionary<Border, ArticleBorderDragDropBehavior> instancesBehavior;

        static ArticleBorderDragDropBehavior()
        {
            instancesBehavior = new Dictionary<Border, ArticleBorderDragDropBehavior>();
        }

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            var border = obj as Border;

            if (border != null)
            {
                ArticleBorderDragDropBehavior? behavior = GetAttachedBehavior(border);

                if (behavior != null)
                {
                    behavior.AssociatedObject = obj as Border;

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
        }

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ArticleBorderDragDropBehavior), new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

        public static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            SetIsEnabled(dependencyObject, (bool)e.NewValue);
        }

        private static ArticleBorderDragDropBehavior? GetAttachedBehavior(Border? listBox)
        {
            if(listBox == null)
                return null;

            if (!instancesBehavior.ContainsKey(listBox))
            {
                instancesBehavior[listBox] = new ArticleBorderDragDropBehavior();
                instancesBehavior[listBox].AssociatedObject = listBox;
            }

            return instancesBehavior[listBox];
        }

        protected virtual void Initialize()
        {
            UnsubscribeFromDragDropEvents();
            SubscribeToDragDropEvents();
        }

        protected virtual void CleanUp()
        {
            UnsubscribeFromDragDropEvents();
        }

        private void SubscribeToDragDropEvents()
        {
            DragDropManager.AddDragInitializeHandler(AssociatedObject, OnDragInitialize);
            DragDropManager.AddGiveFeedbackHandler(AssociatedObject, OnGiveFeedback);
            DragDropManager.AddDropHandler(AssociatedObject, OnDrop);
            DragDropManager.AddDragDropCompletedHandler(AssociatedObject, OnDragDropCompleted);
            DragDropManager.AddDragOverHandler(AssociatedObject, OnDragOver);
        }

        private void UnsubscribeFromDragDropEvents()
        {
            DragDropManager.RemoveDragInitializeHandler(AssociatedObject, OnDragInitialize);
            DragDropManager.RemoveGiveFeedbackHandler(AssociatedObject, OnGiveFeedback);
            DragDropManager.RemoveDropHandler(AssociatedObject, OnDrop);
            DragDropManager.RemoveDragDropCompletedHandler(AssociatedObject, OnDragDropCompleted);
            DragDropManager.RemoveDragOverHandler(AssociatedObject, OnDragOver);
        }

        private void OnDragInitialize(object sender, DragInitializeEventArgs e)
        {
            DropIndicationDetails details = new DropIndicationDetails();
            var listBoxItem = e.OriginalSource as Border ?? (e.OriginalSource as FrameworkElement).ParentOfType<Border>();

            if (AssociatedObject != null && listBoxItem != null)
            {
                string? type = listBoxItem.Tag.ToString();

                details.CurrentDraggedItem = type;

                IDragPayload dragPayload = DragDropPayloadManager.GeneratePayload(null);

                dragPayload.SetData("DraggedData", type);
                dragPayload.SetData("DropDetails", details);

                e.Data = dragPayload;

                e.DragVisual = new Telerik.Windows.DragDrop.DragVisual()
                {
                    Content = details,
                    ContentTemplate = AssociatedObject.Resources["BorderDragTemplate"] as DataTemplate
                };
                e.DragVisualOffset = e.RelativeStartPoint;
                e.AllowedEffects = DragDropEffects.All;
            }
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
            return;
        }

        private void OnDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            return;
        }
    }

}
