using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;

namespace VulpesX.Shared.Controls.Functions
{
    public class TimeSpanSum : EnumerableSelectorAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get
            {
                // which method of the ExtensionMethodsType will handle the summation
                return "Sum";
            }
        }

        // assign new type to handle summation
        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(Statistics);
            }
        }
    }

    public static class Statistics
    {
        public static TimeSpan Sum<T, TTimeSpan>(IEnumerable<T> source, Func<T, TimeSpan> selector)
        {
            return source.Select(selector).Aggregate(TimeSpan.Zero, (t1, t2) => t1 + t2);
        }
    }
}
