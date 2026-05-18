
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class IVAReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public int Year { get; set; }
        public int StartingPage { get; set; }
        public int PageYear => PrintSince.Year;
        public DateTime PrintSince { get; set; }
        public DateTime PrintUntil { get; set; }
        public List<PNIVA>? Rows { get; set; }
        public List<VATRecap>? SalesVATs { get; set; }
        public List<VATRecap>? PurchasesVATs { get; set; }
        public string? TemporaryText { get; set; }
        public string? IntervalText => $"Dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")}";
        public decimal PurchasesAmountVATTotal => PurchasesVATs?.Sum(sum => sum.TotalVATAmount) ?? 0;
        public decimal PurchasesAmountNoVATTotal => PurchasesVATs?.Sum(sum => sum.TotalNoVATAmount) ?? 0;
        public decimal PurchasesTotal => PurchasesAmountVATTotal - PurchasesAmountNoVATTotal;
        public decimal PreviousAmount { get; set; }
        public decimal SalesAmountVATTotal => SalesVATs?.Sum(sum => sum.TotalVATAmount) ?? 0;
        public decimal SalesAmountNoVATTotal => SalesVATs?.Sum(sum => sum.TotalNoVATAmount) ?? 0;
        public decimal SalesTotal => SalesAmountVATTotal - SalesAmountNoVATTotal;
        public decimal PreviousMonthAmount { get; set; }
        public decimal SplitPaymentTotal => SalesVATs?.Where(w => w.IsSplitPayment).Sum(sum => sum.TotalVATAmount) ?? 0;
        public string? TotalText => Total > 0 ? "TOTALE IVA A DEBITO" : Total < 0 ? "TOTALE IVA A CREDITO" : "TOTALE IVA";
        public decimal Total => SalesTotal - ((PreviousMonthAmount - CompensationAmount) + PreviousAmount + PurchasesTotal);
        public decimal TotalDisplay => Total < 0 ? Total * -1 : Total;
        public decimal CompensationAmount { get; set; }
    }
}
