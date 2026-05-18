using VulpesX.Models.Default;

namespace VulpesX.Models.Reports.Production
{
    public class FinalLotLabelReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public byte[]? LogoData { get; set; }

        public pro_ordine_lotti? Data { get; set; }
    }
}
