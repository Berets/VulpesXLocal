using VulpesX.Models.Default;

namespace VulpesX.Models.Reports.SRM
{
    public class LotLabelReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public byte[]? LogoData { get; set; }

        public tab_articolo? Articolo { get; set; }
        public store_stocks_lots? Lotto { get; set; }
    }
}
