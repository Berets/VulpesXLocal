using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Rendering;

namespace VulpesX.Shared.Controls.Gantt
{
    public class TimeLineDeadlineContainer : Control, IDataContainer
    {
        public static readonly DependencyProperty DeadlineProperty = DependencyProperty.Register("Deadline", typeof(string), typeof(TimeLineDeadlineContainer), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty IsExpiredProperty = DependencyProperty.Register("IsExpired", typeof(bool), typeof(TimeLineDeadlineContainer), new PropertyMetadata(false));


        public TimeLineDeadlineContainer()
        {
            this.DefaultStyleKey = typeof(TimeLineDeadlineContainer);
            this.DataItem = this.Deadline;
        }

        public bool IsExpired
        {
            get { return (bool)GetValue(IsExpiredProperty); }
            set { SetValue(IsExpiredProperty, value); }
        }

        public string Deadline
        {
            get { return (string)GetValue(DeadlineProperty); }
            set { SetValue(DeadlineProperty, value); }
        }

        private object? data;
        public object? DataItem
        {
            get { return this.data; }
            set
            {
                if (this.data != value)
                {
                    this.data = value;
                    var info = this.data as TimeLineDeadlineEventInfo;
                    if (info != null)
                    {
                        this.Deadline = info.Deadline;
                        this.IsExpired = info.IsExpired;
                    }
                }
            }
        }
    }

}
