using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class ExpiresReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public byte[]? LogoData { get; set; }
        public ABE? Entity { get; set; }
        public DateTime ExpireDate { get; set; }
        public string? ReportTitle { get; set; }
        public string? PrintedText { get; set; }
        public string? EntityTypeID { get; set; }
        public decimal GrandTotal { get; set; }
        public string? GrandTotalText => $"TOTALE SCADENZE : {GrandTotal.ToString("N2")}";
        public List<ExpiresReportMasterItem>? Rows { get; set; }
    }

    public class ExpiresReportMasterItem
    {
        public DateTime ExpireDate { get; set; }
        public string? EntityID { get; set; }
        public string? EntityDescription { get; set; }
        public decimal EntityTotal { get; set; }
        public string EntityTotalText => $"{PrintSign} {EntityTotal.ToString("N2")}";
        public string? PrintSign { get; set; }
        public List<ExpiresReportItem>? Rows { get; set; }
    }

    public class ExpiresReportItem
    {
        public string? EntityID { get; set; }
        public string? EntityDescription { get; set; }
        public DateTime ExpireDate { get; set; }
        public string? ReferenceID { get; set; }
        public DateTime ReferenceDate { get; set; }
        public string? PaymentTypeID { get; set; }
        public string? PaymentTypeDescription { get; set; }
        public string? CausalDescription { get; set; }
        public decimal Amount { get; set; }
        public string? Sign { get; set; }
        public string? EntityTypeID { get; set; }
        public string? PrintSign { get; set; }
        public string? LockedInfoText { get; set; }
        public DateTime? Locked { get; set; }
        public string? LockedUserID { get; set; }
        public string? LockedReason { get; set; }
    }
}
