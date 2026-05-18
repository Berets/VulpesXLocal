using VulpesX.Models.Default;

namespace VulpesX.Models.Reports.SRM
{
    public class PurchaseOrderReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public acq_orders_heads? PurchaseOrder { get; set; }
        public byte[]? LogoData { get; set; }
        public byte[]? CertificationsLogoData { get; set; }
        public PAGFOR? Payment { get; set; }
        public string? BankData { get; set; }
        public string? CopyInfo { get; set; }
        public string? HeaderFootNote { get; set; }
        public string? FixedText { get; set; }
        public object? LinguaDictionary { get; set; }
    }
}
