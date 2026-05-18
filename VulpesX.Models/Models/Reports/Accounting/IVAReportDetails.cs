
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class IVAReportDetails
    {
        public AZIENDA? CompanyInfo { get; set; }
        public DateTime PrintSince { get; set; }
        public DateTime PrintUntil { get; set; }
        public string? TemporaryText { get; set; }
        public string IntervalText => $"Dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")}";
        public DetailSection? SalesPrevious { get; set; }
        public DetailSection? SalesCurrent { get; set; }
        public DetailSection? SalesPreviousPaid { get; set; }
        public DetailSection? SalesCurrentPaid { get; set; }
        public DetailSection? PurchasesPrevious { get; set; }
        public DetailSection? PurchasesCurrent { get; set; }
        public DetailSection? PurchasesPreviousPaid { get; set; }
        public DetailSection? PurchasesCurrentPaid { get; set; }

        public class DetailSection
        {
            public string? Text { get; set; }
            public string? RecapText { get; set; }
            public string? IVABookType { get; set; }
            public List<PNIVA>? Rows { get; set; }
            public List<VATRecap>? VATs {get; set; }    
            public decimal TotalAmount => (Rows?.Where(w => w.N4SEGN == "+").Sum(sum => sum.N4IMEU) ?? 0) - (Rows?.Where(w => w.N4SEGN == "-").Sum(sum => sum.N4IMEU) ?? 0);
            public decimal TotalVATAmount => (Rows?.Where(w => w.N4SEGN == "+").Sum(sum => sum.N4IVEU) ?? 0) - (Rows?.Where(w => w.N4SEGN == "-").Sum(sum => sum.N4IVEU) ?? 0);
            public decimal TotalNoVATAmount => (Rows?.Where(w => w.N4SEGN == "+").Sum(sum => sum.N4IIEU) ?? 0) - (Rows?.Where(w => w.N4SEGN == "-").Sum(sum => sum.N4IIEU) ?? 0);
            public decimal Balance => TotalVATAmount - TotalNoVATAmount;

        }
    }
}
