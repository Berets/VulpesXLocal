using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Aggregators
{
    public class PNRigheCustomSum : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "SumBySign"; }
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
        public static string SumBySign<TSource>(IEnumerable<PNRIGHE> Rows)
        {
            StringBuilder sb = new StringBuilder();
            decimal dare = 0;
            decimal avere = 0;

            foreach (var item in Rows)
            {
                if (item.N1SEGN == "D")
                {
                    dare += item.N1IMEU.HasValue ? item.N1IMEU.Value : 0;
                }
                else
                {
                    avere += item.N1IMEU.HasValue ? item.N1IMEU.Value : 0;
                }
            }

            sb.Append($"TOTALE: {(dare > avere ? "[D]" : (dare < avere ? "[A]" : String.Empty))} {(dare >= avere ? (dare - avere).ToString("N2") : (avere - dare).ToString("N2"))}");

            return sb.ToString();
        }
    }
}
