using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Aggregators
{
    public class StoreStocksCustomSumAvailability : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "SumByAvailability"; }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(Aggregates);
            }
        }
    }
}
