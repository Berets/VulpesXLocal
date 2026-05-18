using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Telerik.Pivot.Core;
using Telerik.Windows.Controls;

namespace VulpesX.Shared.Controls.PerstistanceManager.Pivot
{
    [DataContract]
    public class DataProviderSettings
    {
        [DataMember]
        public object[]? Aggregates { get; set; }

        [DataMember]
        public object[]? Filters { get; set; }

        [DataMember]
        public object[]? Rows { get; set; }

        [DataMember]
        public object[]? Columns { get; set; }

        [DataMember]
        public int AggregatesLevel { get; set; }

        [DataMember]
        public PivotAxis AggregatesPosition { get; set; }

        [DataMember]
        public PivotLayoutType HorizontalLayout { get; set; }

        [DataMember]
        public PivotLayoutType VerticalLayout { get; set; }

        [DataMember]
        public RowTotalsPosition RowTotalsPosition { get; set; }

        [DataMember]
        public RowTotalsPosition RowGrandTotalsPosition { get; set; }

        [DataMember]
        public ColumnTotalsPosition ColumnTotalsPosition { get; set; }

        [DataMember]
        public ColumnTotalsPosition ColumnGrandTotalsPosition { get; set; }

        [DataMember]
        public bool ShowAggregateValuesInline { get; set; }
    }
}
