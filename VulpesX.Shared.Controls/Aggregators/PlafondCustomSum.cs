using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Aggregators
{
    public class PlafondCustomSum : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "SumByType"; }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(PlafondAggregates);
            }
        }
    }

    public static partial class PlafondAggregates
    {
        public static string SumByType<TSource>(IEnumerable<ACC_PLAFOND_ROWS> Rows)
        {
            StringBuilder sb = new StringBuilder();
            decimal totalInvoice = Rows.Where(w => w.InvoiceTypeID == "F" || w.InvoiceTypeID == "B").Sum(sum => sum.cliimpimpo ?? 0);
            decimal totalCredit = Rows.Where(w => w.InvoiceTypeID == "N").Sum(sum => sum.cliimpimpo ?? 0) * -1;

            sb.Append($"FATTURE: {totalInvoice.ToString("N2")} | NOTE CREDITO: {totalCredit.ToString("N2")}\nSALDO: {(totalInvoice - totalCredit).ToString("N2")}\n");

            return sb.ToString();
        }
    }
}
