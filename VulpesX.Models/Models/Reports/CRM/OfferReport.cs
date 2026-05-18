using VulpesX.Models.Default;

namespace VulpesX.Models.Reports.CRM
{
    public class OfferReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public OFFET00F? Offer { get; set; }
        public byte[]? LogoData { get; set; }
        public byte[]? CertificationsLogoData { get; set; }
        public string? PaymentDescription { get; set; }
        public string? BankData { get; set; }
        public string? CopyInfo { get; set; }
        public string? HeaderFootNote { get; set; }
        public string? FixedText { get; set; }
        public object? LinguaDictionary { get; set; }
    }
}
