using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Aggregators
{
    public class InvoicesCustomSum : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "SumByType"; }
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
        public static string SumByType<TSource>(IEnumerable<FATTT00F> Rows)
        {
            StringBuilder sb = new StringBuilder();
            decimal totalInvoice = Rows.Where(w => w.FTTIPO == "F" || w.FTTIPO == "B").Sum(sum => sum.Imponibile);
            decimal totalCredit = Rows.Where(w => w.FTTIPO == "N").Sum(sum => sum.Imponibile);

            sb.Append($"FATTURE: {totalInvoice.ToString("N2")} | NOTE CREDITO: {totalCredit.ToString("N2")}\nSALDO: {(totalInvoice - totalCredit).ToString("N2")}\n");

            return sb.ToString();
        }
    }
}
