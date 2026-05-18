using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Rendering;

namespace VulpesX.Shared.Controls.Gantt
{
    public class BaselineContainerTime : Control, IDataContainer
    {
        public static readonly DependencyProperty StartPlannedDateProperty = DependencyProperty.Register("StartPlannedDate", typeof(DateTime), typeof(BaselineContainerTime), null);

        public static readonly DependencyProperty EndPlannedDateProperty = DependencyProperty.Register("EndPlannedDate", typeof(DateTime), typeof(BaselineContainerTime), null);

      

        public BaselineContainerTime()
        {
            this.DefaultStyleKey = typeof(BaselineContainerTime);
        }

        public DateTime? StartPlannedDate
        {
            get
            {
                return (DateTime)GetValue(StartPlannedDateProperty);
            }

            set
            {
                SetValue(StartPlannedDateProperty, value);
            }
        }

        public DateTime? EndPlannedDate
        {
            get
            {
                return (DateTime)GetValue(EndPlannedDateProperty);
            }

            set
            {
                SetValue(EndPlannedDateProperty, value);
            }
        }

        private object? data;
        public object? DataItem
        {
            get
            {
                return this.data;
            }

            set
            {
                if (this.data != value)
                {
                    this.data = value;
                    var info = this.data as BaselineEventTime;
                    if (info != null)
                    {
                        this.StartPlannedDate = info.StartPlannedDate;
                        this.EndPlannedDate = info.EndPlannedDate;
                        this.DataContext = info;
                    }
                }
            }
        }
    }

}
