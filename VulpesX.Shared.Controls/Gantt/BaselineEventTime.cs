using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Core;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Gantt
{
    public class BaselineEventTime : SlotInfo
    {
        private readonly pro_ordine_composizione_tempo time;

        public BaselineEventTime(Range<long> timeRange, int index, pro_ordine_composizione_tempo time) : base(timeRange, index, index)
        {
            this.time = time;
        }

        public DateTime StartPlannedDate
        {
            get
            {
                return time.Start;
            }
        }

        public DateTime EndPlannedDate
        {
            get
            {
                return time.End;
            }
        }

        public string Title
        {
            get
            {
                return time.Title ?? string.Empty;
            }
        }

        public string Description
        {
            get
            {
                return time.Description ?? string.Empty;
            }
        }

        public string ToolTip
        {
            get
            {
                return time.ToolTip;
            }
        }
    }

}
