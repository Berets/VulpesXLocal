using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class IVABookReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public int AccountingYear { get; set; }
        public int StartingPage { get; set; }
        public int PageYear => PrintSince.Year;
        public DateTime PrintSince { get; set; }
        public DateTime PrintUntil { get; set; }
        public List<PNIVA>? Rows { get; set; }
        public List<VATRecap>? VATs { get; set; }
        public LIBRIIVA? IVABook { get; set; }
        public string? TemporaryText { get; set; }
        public decimal AmountTotal => VATs?.Sum(sum => sum.TotalAmount) ?? 0;
        public decimal AmountVATTotal => VATs?.Sum(sum => sum.TotalVATAmount) ?? 0;
        public decimal AmountNoVATTotal => VATs?.Sum(sum => sum.TotalNoVATAmount) ?? 0;
        public decimal VATToPay => VATs != null && VATs.Count > 0 ? AmountVATTotal - AmountNoVATTotal : 0m;
    }
}
