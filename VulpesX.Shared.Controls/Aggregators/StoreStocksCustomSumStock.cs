using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Aggregators
{
    public class StoreStocksCustomSumStock : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "SumByStock"; }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(Aggregates);
            }
        }
    }

    public static partial class Aggregates
    {
        public static string SumByStock<TSource>(IEnumerable<store_stocks> Rows)
        {
            return $"Totale giacenza: {Rows.Sum(sum => sum.Info?.QuantityStock)}";
        }
    }
}
