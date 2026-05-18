using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Core;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Gantt
{
    public class TimeLineDeadlineEventInfo : SlotInfo
    {
        private pro_ordine_composizione task;

        public TimeLineDeadlineEventInfo(Range<long> timeRange, int index, pro_ordine_composizione task) : base(timeRange, index, index)
        {
            this.task = task;
        }

        public string Deadline
        {
            get
            {
                return (this.task.DataConsegna.HasValue) ? this.task.DataConsegna.Value.ToShortDateString() : string.Empty;
            }
        }

        public bool IsExpired
        {
            get
            {
                return this.task.IsExpired;
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as TimeLineDeadlineEventInfo);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
