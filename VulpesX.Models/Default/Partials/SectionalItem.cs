using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default.Partials
{
    public class SectionalItem
    {
        public int RowID { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? Sign { get; set; }
        public decimal Amount { get; set; }
        public string? CurrencyID { get; set; }
        public string? CurrencyDoc { get; set; }
        public decimal CurrencyAmount { get; set; }
        public int? OriginalRowID { get; set; }
        public string? Note { get; set; }
        public string? LockedInfoText { get; set; }
        public string? EntityType { get; set; }

        public decimal? CurrencyValue { get; set; }
        public decimal? CurrencyChange { get; set; }
    }
}
