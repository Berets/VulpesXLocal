using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls.Scheduling;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Gantt
{
    public class CustomDragDropBehavior : GanttDragDropBehavior
    {
        public RadGanttView? ganttView = null;
        private DateTime lastDropTime;
        public TimeSpan? deltaDropTime;
        public static ObservableCollection<pro_ordine_composizione> taskResources = new();
        private static Dictionary<RadGanttView, CustomDragDropBehavior> dictionaryBehavior = new Dictionary<RadGanttView, CustomDragDropBehavior>();

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(CustomDragDropBehavior), new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));
        public static readonly DependencyProperty TaskResourcesProperty = DependencyProperty.RegisterAttached("TaskResources", typeof(ObservableCollection<pro_ordine_composizione>), typeof(CustomDragDropBehavior), new PropertyMetadata(OnTaskResourcesPropertyChanged));

        static CustomDragDropBehavior()
        {
            dictionaryBehavior = new Dictionary<RadGanttView, CustomDragDropBehavior>();
        }

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            CustomDragDropBehavior? behavior = GetAttachedBehavior(obj as RadGanttView);

            if (behavior != null)
            {
                behavior.ganttView = obj as RadGanttView;
                behavior.ganttView!.DragDropBehavior = behavior;

                obj.SetValue(IsEnabledProperty, value);
            }
        }

        public static void SetTaskResources(DependencyObject obj, ObservableCollection<pro_ordine_composizione> value)
        {
        }

        public static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            SetIsEnabled(dependencyObject, (bool)e.NewValue);
        }

        private static void OnTaskResourcesPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            var collection = args.NewValue as INotifyCollectionChanged;
            if (collection != null)
            {
                taskResources = collection as ObservableCollection<pro_ordine_composizione> ?? new();
            }
        }

        private static CustomDragDropBehavior? GetAttachedBehavior(RadGanttView? ganttView)
        {
            if (ganttView != null)
            {
                if (!dictionaryBehavior.ContainsKey(ganttView))
                {
                    dictionaryBehavior[ganttView] = new CustomDragDropBehavior();
                    dictionaryBehavior[ganttView].ganttView = ganttView;
                }

                return dictionaryBehavior[ganttView];
            }
            return null;
        }

        protected override bool CanStartDrag(SchedulingDragDropState state)
        {
            if (state.DraggedItem is pro_ordine_composizione)
                if ((state.DraggedItem as pro_ordine_composizione)?.ETerminato ?? false || ((state.DraggedItem as pro_ordine_composizione)?.EFisso ?? false))
                    return false;

            lastDropTime = state.DraggedItem.Start;
            deltaDropTime = null;

            return false;
            //return base.CanStartDrag(state);
        }

        protected override void Drop(SchedulingDragDropState state)
        {
            base.Drop(state);

            var timeToDrop = state.DestinationSlot.Start;
            var task = state.DraggedItem as pro_ordine_composizione;

            if(task != null)
                ResetIsUpdated(GetFather(task));

            deltaDropTime = timeToDrop.Subtract(lastDropTime);

            if (task != null)
            {
                if (task.IsSummarize)
                {
                    this.DropSummary(task);
                    this.CheckSummary(task);
                }
                else
                {
                    this.DropTask(task);
                }
            }

            lastDropTime = timeToDrop;

            deltaDropTime = new TimeSpan();

            if (task != null)
                ResetIsUpdated(GetFather(task));
        }

        #region Private methods
        public void DropSummarize(pro_ordine_composizione Task)
        {
            if (Task.Dependencies.Count > 0 && Task.Dependencies.OrderByDescending(o => o.FromTask.End).FirstOrDefault()?.FromTask.End > Task.Start)
            {
                var duration = Task.End.Subtract(Task.Start);

                Task.Start = Task.Dependencies.OrderByDescending(o => o.FromTask.End).FirstOrDefault()?.FromTask.End ?? Task.Start;
                Task.End = Task.Start.Add(duration);

                foreach (var cld in Task.Children.Cast<pro_ordine_composizione>())
                {
                    if (!(cld.EFisso ?? false) && !cld.ETerminato)
                    {
                        duration = cld.End.Subtract(cld.Start);

                        cld.Start = Task.Start;
                        cld.End = cld.Start.Add(duration);
                    }
                }
            }

            foreach (var dependantTask in Task.DependantTasks)
            {
                if (!(dependantTask.EFisso ?? false) && !dependantTask.ETerminato)
                {
                    var duration = dependantTask.End.Subtract(dependantTask.Start);

                    if (dependantTask.Start < Task.End)
                    {
                        dependantTask.IsUpdated = true;
                        dependantTask.Start = Task.End;
                        dependantTask.End = dependantTask.Start.Add(duration);

                        if (dependantTask.IsSummarize)
                            this.DropSummarize(dependantTask);
                        else
                            this.DropTask(dependantTask);
                    }
                }
            }
        }

        public void DropTask(pro_ordine_composizione Task)
        {
            if (!(Task.EFisso ?? false) && !Task.ETerminato)
            {
                if (Task.Dependencies.Count > 0 && Task.Dependencies.OrderByDescending(o => o.FromTask.End).FirstOrDefault()?.FromTask.End > Task.Start)
                {
                    var delta = Task.End.Subtract(Task.Start);

                    Task.Start = Task.Dependencies.OrderByDescending(o => o.FromTask.End).FirstOrDefault()?.FromTask.End ?? Task.Start;
                    Task.End = Task.Start.Add(delta);
                }

                Task.End = Task.Start.Add(Task.EffectiveDuration);
            }

            Task.OverlappedSpots.Clear();
            UpdateRealDurationOnStartup(Task);

            foreach (var dependantTask in Task.DependantTasks)
            {
                if (!(dependantTask.EFisso ?? false) && !dependantTask.ETerminato)
                {
                    var duration = dependantTask.End.Subtract(dependantTask.Start);

                    if (dependantTask.Start < Task.End)
                    {
                        dependantTask.IsUpdated = true;
                        dependantTask.Start = Task.End;
                        dependantTask.End = dependantTask.Start.Add(duration);

                        if (dependantTask.IsSummarize)
                        {
                            this.DropSummarize(dependantTask);

                            deltaDropTime = new TimeSpan();

                            this.DropSummary(dependantTask);
                            this.CheckSummary(dependantTask);
                        }
                        else
                        {
                            dependantTask.OverlappedSpots.Clear();
                            this.DropTask(dependantTask);
                        }
                    }
                }
                //else blocca lo spostamento di un task di cui il figlio è terminato
                //{
                //    Task.End = dependantTask.Start;
                //    Task.Start = Task.End.Subtract(Task.EffectiveDuration);
                //}
            }

            UpdateFather(Task);
        }

        public void DropSummary(pro_ordine_composizione Task)
        {
            foreach (var tsk in Task.Children.Cast<pro_ordine_composizione>())
            {
                if (!(tsk.EFisso ?? false) && !tsk.ETerminato)
                {
                    if (deltaDropTime.HasValue && deltaDropTime.Value.Ticks != 0 && !tsk.IsUpdated)
                    {
                        tsk.Start = tsk.Start.Add(deltaDropTime.Value);
                        tsk.End = tsk.End.Add(deltaDropTime.Value);
                    }
                }
                DropSummary(tsk);
            }
        }

        public void CheckSummary(pro_ordine_composizione Task)
        {
            foreach (var tsk in Task.Children.Cast<pro_ordine_composizione>())
            {
                if (!tsk.IsSummarize)
                {
                    DropTask(tsk);
                }

                CheckSummary(tsk);
            }

            if (Task.IsSummarize && Task.Children.Any())
            {
                Task.Start = Task.Children.Cast<pro_ordine_composizione>().OrderBy(o => o.Start).FirstOrDefault()?.Start ?? Task.Start;
                Task.End = Task.Children.Cast<pro_ordine_composizione>().OrderByDescending(o => o.End).FirstOrDefault()?.End ?? Task.End;
            }
        }

        public void UpdateFather(pro_ordine_composizione Task)
        {
            pro_ordine_composizione? father = Task.Padre;

            while (father != null && father.IsSummarize)
            {
                father.Start = father.Children.Cast<pro_ordine_composizione>().OrderBy(o => o.Start).FirstOrDefault()?.Start ?? Task.Start;
                father.End = father.Children.Cast<pro_ordine_composizione>().OrderByDescending(o => o.End).FirstOrDefault()?.End ?? Task.End;

                DropSummarize(father);

                father = father.Padre;
            }
        }

        public pro_ordine_composizione GetFather(pro_ordine_composizione Task)
        {
            pro_ordine_composizione? father = Task.Padre;

            while (father != null)
            {
                if (father.Padre != null)
                    father = father.Padre;
                else
                    break;
            }

            return father ?? Task;
        }

        public void ResetIsUpdated(pro_ordine_composizione Task)
        {
            foreach (var tsk in Task.Children.Cast<pro_ordine_composizione>())
            {
                tsk.IsUpdated = false;

                ResetIsUpdated(tsk);
            }
        }

        private void UpdateRealDurationOnStartup(pro_ordine_composizione Task)
        {
            if (Task.Stops != null)
            {
                if (Task.Stops.Any())
                {
                    bool _entered = false;

                    DateTime _taskStart = new DateTime(Task.Start.Year, Task.Start.Month, Task.Start.Day, Task.Start.Hour, Task.Start.Minute, Task.Start.Second);
                    DateTime _taskEnd = new DateTime(Task.End.Year, Task.End.Month, Task.End.Day, Task.End.Hour, Task.End.Minute, Task.End.Second);

                    var _spotStart = Task.Stops.Where(o => (o.Dalle <= _taskStart && o.Alle > _taskStart)).OrderByDescending(o => o.Alle).FirstOrDefault();

                    if (!_entered && _spotStart != null)
                    {
                        Task.Start = _spotStart.Alle;
                        Task.End = Task.Start.Add(Task.EffectiveDuration);

                        _taskStart = new DateTime(Task.Start.Year, Task.Start.Month, Task.Start.Day, Task.Start.Hour, Task.Start.Minute, Task.Start.Second);
                        _taskEnd = new DateTime(Task.End.Year, Task.End.Month, Task.End.Day, Task.End.Hour, Task.End.Minute, Task.End.Second);

                        _entered = true;

                        UpdateRealDurationOnStartup(Task);
                    }

                    var _spotInside = Task.Stops.Where(o => (o.Dalle <= _taskStart && o.Alle >= _taskEnd)).OrderBy(o => o.Dalle).FirstOrDefault();

                    if (!_entered && _spotInside != null)
                    {
                        Task.Start = _spotInside.Alle;
                        Task.End = Task.Start.Add(Task.EffectiveDuration);

                        Task.OverlappedSpots.Add(_spotInside);

                        _taskStart = new DateTime(Task.Start.Year, Task.Start.Month, Task.Start.Day, Task.Start.Hour, Task.Start.Minute, Task.Start.Second);
                        _taskEnd = new DateTime(Task.End.Year, Task.End.Month, Task.End.Day, Task.End.Hour, Task.End.Minute, Task.End.Second);

                        _entered = true;

                        UpdateRealDurationOnStartup(Task);
                    }

                    var _spotOverlap = Task.Stops.Where(o => (o.Dalle >= _taskStart && o.Alle <= _taskEnd) && !Task.OverlappedSpots.Contains(o)).OrderBy(o => o.Dalle);

                    if (!_entered && _spotOverlap.Any())
                    {
                        Task.End = Task.Start.Add(Task.EffectiveDuration + Task.TotalOverlappedSpotsDuration);

                        foreach (var ovr in _spotOverlap)
                        {
                            Task.End = Task.End.Add(ovr.Duration);
                            Task.OverlappedSpots.Add(ovr);
                        }

                        _taskStart = new DateTime(Task.Start.Year, Task.Start.Month, Task.Start.Day, Task.Start.Hour, Task.Start.Minute, Task.Start.Second);
                        _taskEnd = new DateTime(Task.End.Year, Task.End.Month, Task.End.Day, Task.End.Hour, Task.End.Minute, Task.End.Second);

                        _entered = true;

                        UpdateRealDurationOnStartup(Task);
                    }

                    var _spotEnd = Task.Stops.Where(o => (o.Dalle <= _taskEnd && o.Alle >= _taskEnd)).OrderBy(o => o.Dalle).FirstOrDefault();

                    if (!_entered && _spotEnd != null && !Task.OverlappedSpots.Contains(_spotEnd))
                    {
                        if ((Task.End - _spotEnd.Dalle).Ticks > 0)
                        {
                            Task.End = _spotEnd.Alle.Add((Task.End - _spotEnd.Dalle));
                            Task.OverlappedSpots.Add(_spotEnd);

                            _taskStart = new DateTime(Task.Start.Year, Task.Start.Month, Task.Start.Day, Task.Start.Hour, Task.Start.Minute, Task.Start.Second);
                            _taskEnd = new DateTime(Task.End.Year, Task.End.Month, Task.End.Day, Task.End.Hour, Task.End.Minute, Task.End.Second);

                            _entered = true;

                            UpdateRealDurationOnStartup(Task);
                        }
                    }

                    Task.EffectiveDuration = (Task.End - Task.Start) - Task.TotalOverlappedSpotsDuration;
                }
            }

            CheckQueue(Task);
        }

        private void CheckQueue(pro_ordine_composizione Task)
        {
            bool entered = false;

            if (taskResources != null && taskResources.Where(o => o.ResourceID == Task.ResourceID).Any() && Task.ResourceID != null)
            {
                foreach (var tsk in taskResources.Where(o => o.UniqueId != Task.UniqueId && o.ResourceID == Task.ResourceID).OrderBy(o => o.Start).ToList())
                {
                    if (!entered && tsk.IsElaborated && ((Task.Start >= tsk.Start && Task.Start < tsk.End) || (Task.End > tsk.Start && Task.End <= tsk.End) || (Task.Start <= tsk.Start && Task.End >= tsk.End)) && !Task.DependantTasks.Where(o => o.UniqueId == tsk.UniqueId).Any())
                    {
                        Task.Start = tsk.End.AddSeconds(1);
                        Task.End = Task.Start.Add(Task.EffectiveDuration);
                        Task.IsElaborated = true;

                        Task.OverlappedSpots.Clear();

                        UpdateRealDurationOnStartup(Task);

                        entered = true;
                        break;
                    }
                }
            }

            if(taskResources != null)
                {
                var currentTask = taskResources.Where(o => o.UniqueId == Task.UniqueId).FirstOrDefault();

                if (currentTask != null)
                    currentTask.IsElaborated = true;
            }
        }
        #endregion
    }
}
