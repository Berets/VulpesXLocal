using VulpesX.Models.Default;

namespace VulpesX.Models.Reports.Shipping
{
    public class DDTReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public BOLLT00F? DDT { get; set; }
        public required byte[]? LogoData { get; set; }
        public byte[]? CertificationsLogoData { get; set; }
        public string? FixedText { get; set; }
        public object? LinguaDictionary { get; set; }
    }
}
