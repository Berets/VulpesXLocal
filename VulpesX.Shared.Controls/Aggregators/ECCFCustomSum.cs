using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Accounting;
using VulpesX.Models.Models.Reports.Accounting;

namespace VulpesX.Shared.Controls.Aggregators
{
    public class ECCFRecap : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "ECCFRecap"; }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(Aggregates);
            }
        }
    }

    public class ECCFAvereSum : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "ECCFAvereSum"; }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(Aggregates);
            }
        }
    }

    public class ECCFDareSum : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "ECCFDareSum"; }
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
        public static string ECCFRecap<TSource>(IEnumerable<MastrinoECReportItem> Rows)
        {
            StringBuilder sb = new StringBuilder();
            var first = Rows.FirstOrDefault();
            sb.Append($"# SALDO PARTITA N. {first?.ReferenceID?.Trim().PadLeft(10, ' ')} DEL {first?.ReferenceDate?.ToString("d")}");

            return sb.ToString();
        }

        public static string ECCFDareSum<TSource>(IEnumerable<MastrinoECReportItem> Rows)
        {
            StringBuilder sb = new StringBuilder();

            var dare = Rows.Sum(sum => sum.Dare);
            var avere = Rows.Sum(sum => sum.Avere);
            var first = Rows.FirstOrDefault();
            sb.Append($"{(dare >= avere ? (dare - avere).ToString("N2").PadLeft(20, ' ') : "0.00".PadLeft(20, ' '))}");

            return sb.ToString();
        }

        public static string ECCFAvereSum<TSource>(IEnumerable<MastrinoECReportItem> Rows)
        {
            StringBuilder sb = new StringBuilder();

            var dare = Rows.Sum(sum => sum.Dare);
            var avere = Rows.Sum(sum => sum.Avere);
            var first = Rows.FirstOrDefault();
            sb.Append($"{(avere >= dare ? (avere - dare).ToString("N2").PadLeft(20, ' ') : "0.00".PadLeft(20, ' '))}");

            return sb.ToString();
        }

        public static string SumByMovementSign<TSource>(IEnumerable<store_movements> Rows)
        {

            var results = new List<StoreMovementsCustomSum.SumResult>();

            foreach (var item in Rows.OrderBy(o => o.date))
            {
                var exist = results.Where(w => w.UM == item.UM).FirstOrDefault();
                if (exist != null)
                {
                    if (item.Sign == "+")
                        exist.Total += (item.quantity ?? 0);
                    if (item.Sign == "-")
                        exist.Total -= (item.quantity ?? 0);
                }
                else
                {
                    results.Add(new StoreMovementsCustomSum.SumResult()
                    {
                        UM = item.UM,
                        Total = item.Sign == "+" ? (item.quantity ?? 0) : (item.Sign == "-" ? (item.quantity ?? 0) * -1 : 0)
                    });
                }
            }

            StringBuilder sb = new StringBuilder("QUANTITA': ");
            foreach (var item in results)
            {
                sb.Append($"{item.Total.ToString("N6")} {item.UM} | ");
            }

            return sb.ToString();
        }

        public static string SumByAvailability<TSource>(IEnumerable<store_stocks> Rows)
        {
            return $"Totale disponibile: {Rows.Sum(sum => sum.Info?.QuantityAvailable)}";
        }
    }
}
