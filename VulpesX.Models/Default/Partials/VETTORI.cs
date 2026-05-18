using System.Collections.ObjectModel;
using VulpesX.Models.Ufp;

namespace VulpesX.Models.Default
{
    public partial class VETTORI
    {
        public string FullDescriptionSearchable => $"{vetcod} {vetdes?.Trim()}";
        public string? SupplierFullDescription { get; set; }
        public string? DDTInfo => vetind != "#" ? $"{vetind?.Trim()}, P.I.{vetpiva} Albo {vetcalb?.Trim()}" : null;
        public decimal? vetpivadec { get; set; }
    }
}
