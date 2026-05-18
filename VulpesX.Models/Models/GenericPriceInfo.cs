using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models
{
    public class GenericPriceInfo
    {
        public decimal Price { get; set; }
        public decimal? Discount1 { get; set; }
        public decimal? Discount2 { get; set; }
        public decimal? Discount3 { get; set; }
        public string? DiscountType1 { get; set; }
        public string? DiscountType2 { get; set; }
        public string? DiscountType3 { get; set; }
        public decimal? Surcharge { get; set; }
        public string? SurchargeType { get; set; }
        public string OthersText => $"[-] {(Discount1.HasValue && Discount1.Value > 0 ? Discount1.Value.ToString("N2") + (DiscountType1 == "V" ? "€" : "%") : " ")} {(Discount2.HasValue && Discount2.Value > 0 ? Discount2.Value.ToString("N2") + (DiscountType2 == "V" ? "€" : "%") : " ")} {(Discount3.HasValue && Discount3.Value > 0 ? Discount3.Value.ToString("N2") + (DiscountType3 == "V" ? "€" : "%") : " ")} [+] {(Surcharge.HasValue && Surcharge.Value > 0 ? Surcharge.Value.ToString("N2") + (SurchargeType == "V" ? "€" : "%") : " ")}";
        public string? Note { get; set; }
        public string? CustomerProductID { get; set; }
        public string? CustomerProductDescription { get; set; }
        public string? SupplierProductID { get; set; }
        public string? SupplierProductDescription { get; set; }
    }
}
