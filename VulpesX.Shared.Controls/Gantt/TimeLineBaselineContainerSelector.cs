using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Rendering.Virtualization;

namespace VulpesX.Shared.Controls.Gantt
{
    public class TimeLineBaselineContainerSelector : DefaultTimeLineContainerSelector
    {
        protected static readonly ContainerTypeIdentifier BaselineContainerType = ContainerTypeIdentifier.FromType<BaselineContainer>();
        protected static readonly ContainerTypeIdentifier BaselineContainerTimeType = ContainerTypeIdentifier.FromType<BaselineContainerTime>();
        protected static readonly ContainerTypeIdentifier DeadlineContainerCustomType = ContainerTypeIdentifier.FromType<TimeLineDeadlineContainer>();

        public override ContainerTypeIdentifier GetContainerType(object item)
        {
            if (item is BaselineEventInfo)
            {
                return BaselineContainerType;
            }

            if (item is BaselineEventTime)
            {
                return BaselineContainerTimeType;
            }

            if (item is TimeLineDeadlineEventInfo)
            {
                return DeadlineContainerCustomType;
            }

            return base.GetContainerType(item);
        }
    }
}
