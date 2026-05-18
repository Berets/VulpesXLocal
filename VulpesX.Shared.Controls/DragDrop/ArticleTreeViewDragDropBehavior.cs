using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.DragDrop;
using VulpesX.Models.Default;
using VulpesX.Shared.Utilities;

namespace VulpesX.Shared.Controls.DragDrop
{

    public class ArticleTreeViewDragDropBehavior
    {
        public double treeViewItemHeight { get; set; }

        private bool isTreeSource = false;

        private ObservableCollection<tab_articolo_composizione>? sourceItems = null;
        private ObservableCollection<tab_articolo_composizione>? destinationItems = null;

        private object? sourceItem = null;

        private RadTreeView? _associatedObject;
        public RadTreeView? AssociatedObject
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

        private static Dictionary<RadTreeView, ArticleTreeViewDragDropBehavior> dictionaryBehavior;

        static ArticleTreeViewDragDropBehavior()
        {
            dictionaryBehavior = new Dictionary<RadTreeView, ArticleTreeViewDragDropBehavior>();
        }

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            var rtv = obj as RadTreeView;

            if (rtv != null)
            {
                ArticleTreeViewDragDropBehavior behavior = GetAttachedBehavior(rtv);

                behavior.AssociatedObject = rtv;

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

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ArticleTreeViewDragDropBehavior), new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

        public static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            SetIsEnabled(dependencyObject, (bool)e.NewValue);
        }

        private static ArticleTreeViewDragDropBehavior GetAttachedBehavior(RadTreeView gridview)
        {
            if (!dictionaryBehavior.ContainsKey(gridview))
            {
                dictionaryBehavior[gridview] = new ArticleTreeViewDragDropBehavior();
                dictionaryBehavior[gridview].AssociatedObject = gridview;
            }

            return dictionaryBehavior[gridview];
        }

        protected virtual void Initialize()
        {
            DragDropManager.AddDragInitializeHandler(this.AssociatedObject, OnDragInitialize);
            DragDropManager.AddGiveFeedbackHandler(this.AssociatedObject, OnGiveFeedback);
            DragDropManager.AddDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted);
            DragDropManager.AddDropHandler(this.AssociatedObject, OnDrop);
            DragDropManager.AddDragOverHandler(this.AssociatedObject, OnDragOver);

            this.treeViewItemHeight = 24.0;

            if (this.AssociatedObject != null)
                this.AssociatedObject.ItemPrepared += AssociatedObject_ItemPrepared;
        }

        protected virtual void CleanUp()
        {
            DragDropManager.RemoveDragInitializeHandler(this.AssociatedObject, OnDragInitialize);
            DragDropManager.RemoveGiveFeedbackHandler(this.AssociatedObject, OnGiveFeedback);
            DragDropManager.RemoveDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted);
            DragDropManager.RemoveDropHandler(this.AssociatedObject, OnDrop);
        }

        void AssociatedObject_ItemPrepared(object? sender, RadTreeViewItemPreparedEventArgs e)
        {
            DragDropManager.RemoveDragOverHandler(e.PreparedItem, OnItemDragOver);
            DragDropManager.AddDragOverHandler(e.PreparedItem, OnItemDragOver);
        }

        private void OnDragInitialize(object sender, DragInitializeEventArgs e)
        {
            var treeViewItem = e.OriginalSource as RadTreeViewItem ?? (e.OriginalSource as FrameworkElement).ParentOfType<RadTreeViewItem>();
            var data = treeViewItem != null ? treeViewItem.Item : (sender as RadTreeView)?.SelectedItem;

            var payload = DragDropPayloadManager.GeneratePayload(null);

            var dropDetails = new DropIndicationDetails();
            dropDetails.CurrentDraggedItem = data;

            var visual = new Telerik.Windows.DragDrop.DragVisual()
            {
                Content = dropDetails,
                ContentTemplate = this.AssociatedObject?.Resources["ProductDragTemplate"] as DataTemplate
            };

            payload.SetData("DraggedData", data);
            payload.SetData("DropDetails", dropDetails);

            e.DragVisual = visual;
            e.DragVisualOffset = e.RelativeStartPoint;
            e.Data = payload;
            e.AllowedEffects = DragDropEffects.All;

            FrameworkElement sourceItem = e.OriginalSource as RadTreeViewItem ?? (e.OriginalSource as FrameworkElement).ParentOfType<RadTreeViewItem>();
            if (sourceItem == null)
            {
                this.sourceItems = this.AssociatedObject?.ItemsSource as ObservableCollection<tab_articolo_composizione>;
            }
            else
            {
                this.sourceItems = (sourceItem as RadTreeViewItem)?.ParentItem != null ? (sourceItem as RadTreeViewItem)?.ParentItem.ItemsSource as ObservableCollection<tab_articolo_composizione> : this.AssociatedObject?.ItemsSource as ObservableCollection<tab_articolo_composizione>;
            }

            this.sourceItem = sourceItem?.DataContext;
            this.destinationItems = this.AssociatedObject?.ItemsSource as ObservableCollection<tab_articolo_composizione>;
            this.isTreeSource = true;
        }

        private void OnGiveFeedback(object sender, Telerik.Windows.DragDrop.GiveFeedbackEventArgs e)
        {
            e.SetCursor(Cursors.Arrow);
            e.Handled = true;
        }

        private void OnDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            var dropDetails = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DropIndicationDetails;

            if (dropDetails != null)
            {
                dropDetails.CurrentDropPosition = DropPosition.Undefined;

                e.Handled = true;
            }
        }

        private void OnItemDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            var draggedData = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");
            var dropDetails = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DropIndicationDetails;
            var item = (e.OriginalSource as FrameworkElement).ParentOfType<RadTreeViewItem>();

            if (dropDetails != null)
            {
                var position = GetPosition(item, e.GetPosition(item));
                if (item.Level == 0 && position != DropPosition.Inside)
                {
                    e.Effects = DragDropEffects.None;
                }
                else
                {
                    if (position != DropPosition.Inside)
                    {
                        e.Effects = DragDropEffects.All;
                        this.destinationItems = item.Level > 0 ? (ObservableCollection<tab_articolo_composizione>)item.ParentItem.ItemsSource : (ObservableCollection<tab_articolo_composizione>)this.AssociatedObject!.ItemsSource;

                        var composition = item.Item as tab_articolo_composizione;

                        if (composition != null)
                        {
                            int index = this.destinationItems.IndexOf(composition);
                            dropDetails.DropIndex = position == DropPosition.Before ? index : index + 1;
                        }
                    }
                    else
                    {
                        this.destinationItems = (ObservableCollection<tab_articolo_composizione>)item.ItemsSource;
                        int index = this.destinationItems.Count;

                        if (destinationItems == null)
                        {
                            e.Effects = DragDropEffects.None;
                        }
                        else
                        {
                            e.Effects = DragDropEffects.All;
                            dropDetails.DropIndex = index;
                        }
                    }
                }

                dropDetails.CurrentDraggedOverItem = item.Item;
                dropDetails.CurrentDropPosition = position;

                if (draggedData is tab_articolo_composizione)
                {
                    var composition = draggedData as tab_articolo_composizione;

                    if (composition != null && !composition.EReparto && !composition.EMilestone && !composition.ESummary && this.IsChildOfSource(position, item, composition))
                        e.Effects = DragDropEffects.None;
                }

                if (draggedData is tab_articolo)
                {
                    var article = draggedData as tab_articolo;

                    if (article != null && this.IsChildOfSource(position, item, article))
                        e.Effects = DragDropEffects.None;
                }

                if (draggedData == item.DataContext)
                    e.Effects = DragDropEffects.None;

                if (draggedData is tab_articolo_composizione && IsOverChildren((draggedData as tab_articolo_composizione)?.Componenti, (item.DataContext as tab_articolo_composizione)))
                    e.Effects = DragDropEffects.None;

            }

            e.Handled = true;
        }

        private void OnDrop(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            if (this.isTreeSource)
            {
                var data = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");

                if (data is tab_articolo_composizione)
                {
                    var composition = data as tab_articolo_composizione;

                    if (this.sourceItems != null && composition != null)
                        this.sourceItems.Remove(composition);
                }
            }

            RadTreeViewItem? destinationItem = null;
            if (e.Effects != DragDropEffects.None)
            {
                destinationItem = (e.OriginalSource as FrameworkElement).ParentOfType<RadTreeViewItem>();
                var dropDetails = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DropIndicationDetails;
                var data = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");

                if (destinationItem != null && destinationItems != null && destinationItem.DataContext is tab_articolo_composizione && dropDetails != null)
                {
                    int dropIndex = dropDetails.DropIndex >= destinationItems.Count ? destinationItems.Count : dropDetails.DropIndex < 0 ? 0 : dropDetails.DropIndex;

                    ArticleDropHandler.SyncDrop(data, destinationItems, dropIndex);
                    //if (data is tab_produzione_reparto)
                    //{
                    //    data = new tab_articolo_composizione
                    //    {
                    //        SocietaID = (data as tab_produzione_reparto).SocietaID,
                    //        RepartoID = (data as tab_produzione_reparto).ID,
                    //        ArticoloID = (data as tab_produzione_reparto).ID,
                    //        Descrizione = (data as tab_produzione_reparto).Descrizione,
                    //        RevisioneID = string.Empty,
                    //        Componenti = new ObservableCollection<tab_articolo_composizione>(),
                    //        //Risorse = new tab_produzione_risorsaService().GetListFromReparto((data as tab_produzione_reparto).SocietaID, (data as tab_produzione_reparto).ID),
                    //        Tempo = (data as tab_produzione_reparto).TempoDefault,
                    //        EComponente = true,
                    //    };
                    //}

                    //if (data is string)
                    //{
                    //    data = new tab_articolo_composizione
                    //    {
                    //        SocietaID = (destinationItem.DataContext as tab_articolo_composizione).SocietaID,
                    //        RepartoID = string.Empty,
                    //        ArticoloID = string.Empty,
                    //        Descrizione = string.Empty,
                    //        RevisioneID = string.Empty,
                    //        Componenti = new ObservableCollection<tab_articolo_composizione>(),
                    //        ESummary = (data.ToString() == "Summary"),
                    //        EMilestone = (data.ToString() == "Milestone"),
                    //        EComponente = true,
                    //    };
                    //}

                    //if (data is tab_articolo)
                    //{
                    //    data = new tab_articolo_composizioneService().Single((data as tab_articolo).SocietaID, (data as tab_articolo).ID, (data as tab_articolo).RevisioneID, null);
                    //}

                    //(data as tab_articolo_composizione).Padre = (dropDetails.CurrentDropPosition == DropPosition.Inside) ? destinationItem.DataContext as tab_articolo_composizione : (destinationItem.DataContext as tab_articolo_composizione).Padre;
                    //(data as tab_articolo_composizione).ArticoloID = ((data as tab_articolo_composizione).Padre.IsRoot) ? (data as tab_articolo_composizione).Padre.ArticoloID : (data as tab_articolo_composizione).Padre.ComponenteArticoloID;
                    //(data as tab_articolo_composizione).RevisioneID = ((data as tab_articolo_composizione).Padre.IsRoot) ? (data as tab_articolo_composizione).Padre.RevisioneID : (data as tab_articolo_composizione).Padre.ComponenteRevisioneID;
                    //(data as tab_articolo_composizione).IsRoot = false;

                    //this.destinationItems.Insert(dropIndex, data as tab_articolo_composizione);
                }
            }

            if (destinationItems?.Count > 0)
            {
                if ((destinationItems[0] as tab_articolo_composizione)?.Padre != null)
                {
                    (destinationItems[0] as tab_articolo_composizione)!.Padre!.EEspanso = false;
                }
            }
            if (this.isTreeSource)
            {
                this.isTreeSource = false;
                this.sourceItem = null;
                this.sourceItems = null;
                this.destinationItems = null;
            }
        }

        private void OnDragDropCompleted(object sender, DragDropCompletedEventArgs e)
        {
            if (e.Effects != DragDropEffects.None && this.isTreeSource)
            {
                var data = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");

                var composition = data as tab_articolo_composizione;

                if (this.sourceItems != null && composition != null)
                    this.sourceItems.Remove(composition);
            }
        }

        private bool IsChildOfSource(DropPosition position, FrameworkElement item, tab_articolo_composizione? dragged)
        {
            var dropped = item.DataContext as tab_articolo_composizione;

            if (dragged == null)
                return false;

            while (dropped != null)
            {
                if (dropped.ComponenteArticoloID == dragged.ComponenteArticoloID || dropped.ComponenteArticoloID == dragged.ArticoloID)
                    return true;

                if (this.isTreeSource && position == DropPosition.Inside && dropped.Componenti.Where(o => o.ComponenteArticoloID == dragged.ArticoloID).Any())
                    return true;

                if (!this.isTreeSource && dropped.Componenti.Where(o => o.ComponenteArticoloID == dragged.ArticoloID).Any())
                    return true;

                if (IsFather(dropped, dragged))
                    return true;

                if (position == DropPosition.Inside)
                    return IsChildOfSource(dropped, dragged);

                dropped = null;
            }

            return false;
        }

        private bool IsChildOfSource(DropPosition position, FrameworkElement item, tab_articolo dragged)
        {
            var dropped = item.DataContext as tab_articolo_composizione;

            while (dropped != null)
            {
                if (dropped.ComponenteArticoloID == dragged.ID || dropped.ComponenteArticoloID == dragged.ID)
                    return true;

                if (this.isTreeSource && position == DropPosition.Inside && dropped.Componenti.Where(o => o.ComponenteArticoloID == dragged.ID).Any())
                    return true;

                if (!this.isTreeSource && dropped.Componenti.Where(o => o.ComponenteArticoloID == dragged.ID).Any())
                    return true;

                if (IsFather(dropped, dragged))
                    return true;

                if (position == DropPosition.Inside)
                    return IsChildOfSource(dropped, dragged);

                dropped = null;
            }

            return false;
        }


        private bool IsChildOfSource(tab_articolo_composizione dropped, tab_articolo_composizione dragged)
        {
            foreach (var cmp in dragged.Componenti ?? new ObservableCollection<tab_articolo_composizione>())
            {
                if (dropped.ArticoloID == cmp.ArticoloID)
                    return true;

                IsChildOfSource(dropped, cmp);
            }

            return false;
        }

        private bool IsChildOfSource(tab_articolo_composizione dropped, tab_articolo dragged)
        {
            if (dropped.ArticoloID == dragged.ID)
                return true;

            return false;
        }


        private bool IsFather(tab_articolo_composizione dropped, tab_articolo_composizione dragged)
        {
            var father = dropped.Padre;

            while (father != null)
            {
                if (father.ComponenteArticoloID == dragged.ComponenteArticoloID)
                    return true;

                father = father.Padre;
            }

            return false;
        }

        private bool IsFather(tab_articolo_composizione dropped, tab_articolo dragged)
        {
            var father = dropped.Padre;

            while (father != null)
            {
                if (father.ComponenteArticoloID == dragged.ID)
                    return true;

                father = father.Padre;
            }

            return false;
        }


        private bool IsOverChildren(ObservableCollection<tab_articolo_composizione>? childrens, tab_articolo_composizione? dropped)
        {
            bool retValue = false;
            foreach (var chl in (childrens ?? new ObservableCollection<tab_articolo_composizione>()))
            {
                if (chl == dropped)
                {
                    retValue = true;
                    break;
                }

                retValue = IsOverChildren(chl.Componenti, dropped);

                if (retValue)
                    break;
            }

            return retValue;
        }

        private DropPosition GetPosition(RadTreeViewItem item, Point point)
        {
            if (point.Y < this.treeViewItemHeight / 4)
            {
                return DropPosition.Before;
            }
            else if (point.Y > this.treeViewItemHeight * 3 / 4)
            {
                return DropPosition.After;
            }

            return DropPosition.Inside;
        }
    }

}
