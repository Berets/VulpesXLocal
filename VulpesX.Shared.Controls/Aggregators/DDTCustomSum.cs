using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;
using VulpesX.Models.Default;
using VulpesX.Shared.Generics;

namespace VulpesX.Shared.Controls.Aggregators
{
    public class DDTCustomSum : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "SumByUM"; }
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
        public static string SumByUM<TSource>(IEnumerable<BOLLT00F> List)
        {
            List<GenericStringDecimal> ums = new List<GenericStringDecimal>();

            foreach (var item in List)
            {
                foreach (var row in item.Rows ?? new())
                {
                    var existing = ums.Where(w => w.Description == row.BOUNIM).FirstOrDefault();
                    if (existing != null)
                    {
                        existing.Value += row.BOQTAV;
                    }
                    else
                    {
                        ums.Add(new GenericStringDecimal() { Description = row.BOUNIM, Value = row.BOQTAV });
                    }
                }
            }

            var sb = new StringBuilder();
            foreach (var um in ums)
            {
                sb.Append($"TOTALE {um.Description}: {Math.Round(um.Value, 2, MidpointRounding.AwayFromZero).ToString("N6")}\n");
            }


            return sb.ToString();
        }
    }
}
