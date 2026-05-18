using VulpesX.Models.Default;

namespace VulpesX.Models.Reports.SRM
{
    public class ProductLabelReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public byte[]? LogoData { get; set; }

        public tab_articolo? Articolo { get; set; }
    }
}
