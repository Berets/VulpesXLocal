using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Core;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Gantt
{
    public class TimeLineBaselineBehavior : DefaultGanttTimeLineVisualizationBehavior
    {
        protected override IEnumerable<IEventInfo> GetEventInfos(TimeLineVisualizationState state, HierarchicalItem hierarchicalItem)
        {
            foreach (var eventInfo in base.GetEventInfos(state, hierarchicalItem))
            {
                yield return eventInfo;
            }

            var task = hierarchicalItem.SourceItem as pro_ordine_composizione;

            var deadline = task != null ? task.DataConsegna : default(DateTime?);

            if (task != null)
            {
                if (task.Stops != null)
                {
                    foreach (var spot in task.Stops)
                    {
                        var baselineStartDate = task != null ? spot.Dalle : DateTime.MinValue;

                        if (baselineStartDate != DateTime.MinValue)
                        {
                            var roundedDeadline = state.Rounder.Round(new DateRange(baselineStartDate, spot.Alle));
                            var baselineRange = new Range<long>(roundedDeadline.Start.Ticks, roundedDeadline.End.Ticks);

                            if (baselineRange.IntersectsWith(state.VisibleTimeRange))
                            {
                                yield return new BaselineEventInfo(baselineRange, hierarchicalItem.Index, spot);
                            }
                        }
                    }
                }
            }

            if (task != null)
            {
                if (task.Times != null)
                {
                    foreach (var time in task.Times)
                    {
                        var baselineStartDate = task != null ? time.Start : DateTime.MinValue;

                        if (baselineStartDate != DateTime.MinValue)
                        {
                            var roundedDeadline = state.Rounder.Round(new DateRange(baselineStartDate, time.End));
                            var baselineRange = new Range<long>(roundedDeadline.Start.Ticks, roundedDeadline.End.Ticks);

                            if (baselineRange.IntersectsWith(state.VisibleTimeRange))
                            {
                                yield return new BaselineEventTime(baselineRange, hierarchicalItem.Index, time);
                            }
                        }
                    }
                }
            }


            if (deadline.HasValue)
            {
                var roundedDeadline = state.Rounder.Round(new DateRange(deadline.Value, deadline.Value));
                var deadlineRange = new Range<long>(roundedDeadline.Start.Ticks, roundedDeadline.End.Ticks);

                if (deadlineRange.IntersectsWith(state.VisibleTimeRange))
                {
                    var spot = hierarchicalItem.SourceItem as pro_ordine_composizione;
                    if (spot != null)
                        yield return new TimeLineDeadlineEventInfo(deadlineRange, hierarchicalItem.Index, spot);
                }
            }

        }
    }
}
