using VulpesX.Models.Default;

namespace VulpesX.Models.Reports.Production
{
    public class ProductionReport
    {
        public byte[]? LogoData { get; set; }
        public pro_ordine? Order { get; set; }

        public List<pro_ordine_composizione>? Productions { get; set; }
        public List<store_stocks_engage>?Engages { get; set; }

        public AZIENDA? CompanyInfo { get; set; }
    }
}
