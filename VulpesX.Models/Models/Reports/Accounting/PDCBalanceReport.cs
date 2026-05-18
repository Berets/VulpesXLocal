using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class PDCBalanceReport
    {
        public string? Description { get; set; }
        public List<PDCBalanceReportItem>? Rows { get; set; }
        public decimal TotalAmount { get; set; }
        public string? TotalAmountSign { get; set; }
        public string TotalText => $"TOTALE {Description} : {TotalAmount.ToString("N2")} {TotalAmountSign}";
        public decimal? BalanceAmount { get; set; }
        public string? BalanceAmountSign { get; set; }
        public string? BalanceText => BalanceAmount.HasValue ? $"SBILANCIO : {BalanceAmount?.ToString("N2")} {BalanceAmountSign}" : null;

        public class PDCBalanceReportItem
        {
            public string? GroupID { get; set; }
            public string? AccountID { get; set; }
            public string? SubaccountID { get; set; }
            public string? SubaccountDescription { get; set; }
            public decimal? Amount { get; set; }
            public string? Sign { get; set; }
            public string? ExternalCode { get; set; }
            public string? AlternativeCode { get; set; }
            // subtotal
            public bool IsSubTotal { get; set; } = false;
            public bool IsGroupSubtotal { get; set; } = false;
            public string? SubtotalText { get; set; }
            public decimal? SubTotalAmount { get; set; }
            public string? SubTotalAmountSign { get; set; }
            public string? SubTotalMark { get; set; }
        }
    }

    public class PDCBalanceSubReport
    {
        public PDCBalanceReport? LeftGroup { get; set; }
        public PDCBalanceReport? RightGroup { get; set; }
    }
    public class PDCBalanceReportOpposed
    {
        public AZIENDA? CompanyInfo { get; set; }
        public DateTime DateLimit { get; set; }
        public string DateLimitText => $"AL {DateLimit.ToString("dd/MM/yyyy")}";
        public string? CostCenter { get; set; }
        public PDCBalanceSubReport? SubreportAP { get; set; }
        public PDCBalanceSubReport? SubreportCR { get; set; }

    }
}
