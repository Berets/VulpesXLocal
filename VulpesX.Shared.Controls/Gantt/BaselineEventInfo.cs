using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Core;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Gantt
{
    public class BaselineEventInfo : SlotInfo
    {
        private readonly tab_produzione_calendario_chiusura spot;

        public BaselineEventInfo(Range<long> timeRange, int index, tab_produzione_calendario_chiusura spot) : base(timeRange, index, index)
        {
            this.spot = spot;
        }

        public DateTime StartPlannedDate
        {
            get
            {
                return spot.Dalle;
            }
        }

        public DateTime EndPlannedDate
        {
            get
            {
                return spot.Alle;
            }
        }

        public string ToolTip
        {
            get
            {
                return spot.ToolTip;
            }
        }
    }
}
