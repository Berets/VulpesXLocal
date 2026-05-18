using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class BankFluxMonthItem
    {
        public int Month { get; set; }
        public string? MonthDescription { get; set; }
        public decimal Total => Amounts.Sum();
        public decimal[] Amounts { get; set; } = new decimal[3];
        public bool IsTemporary { get; set; } = false;
    }
}
