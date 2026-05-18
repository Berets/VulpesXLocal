using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class BankFluxItem
    {
        public string? CausalID { get; set; }
        public string? CausalDescription { get; set; }
        public decimal Total => Amounts.Sum();
        public decimal[] Amounts { get; set; } = new decimal[12];
    }
}
