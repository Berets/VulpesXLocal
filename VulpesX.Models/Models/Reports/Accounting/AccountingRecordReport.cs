using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class AccountingRecordReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public byte[]? LogoData { get; set; }
        public PNTESTATA? Head { get; set; }
    }
}
