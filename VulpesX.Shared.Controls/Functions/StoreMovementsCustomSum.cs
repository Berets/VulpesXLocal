using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Functions
{
    public class StoreMovementsCustomSum : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "SumByMovementSign"; }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(Aggregates);
            }
        }

        internal class SumResult
        {
            public required string UM { get; set; }
            public decimal Total { get; set; }

            public override bool Equals(object? obj)
            {
                if (obj == null)
                    return false;
                if (!(obj is SumResult))
                    return false;

                return ((SumResult)obj).UM == UM;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(UM);
            }
        }
    }

    internal static partial class Aggregates
    {
        public static string SumByMovementSign<TSource>(IEnumerable<store_movements> Rows)
        {

            var results = new List<StoreMovementsCustomSum.SumResult>();

            foreach (var item in Rows.OrderBy(o => o.date))
            {
                if (item != null && item.quantity.HasValue)
                {
                    var exist = results.Where(w => w.UM == item.UM).FirstOrDefault();
                    if (exist != null)
                    {
                        if (item.Sign == "+")
                            exist.Total += item.quantity.Value;
                        if (item.Sign == "-")
                            exist.Total -= item.quantity.Value;
                    }
                    else
                    {
                        results.Add(new StoreMovementsCustomSum.SumResult()
                        {
                            UM = item.UM ?? string.Empty,
                            Total = item.Sign == "+" ? item.quantity.Value : (item.Sign == "-" ? item.quantity.Value * -1 : 0)
                        });
                    }
                }
            }

            StringBuilder sb = new StringBuilder("QUANTITA': ");
            foreach (var item in results)
            {
                sb.Append($"{item.Total.ToString("N6")} {item.UM} | ");
            }

            return sb.ToString();
        }
    }
}
