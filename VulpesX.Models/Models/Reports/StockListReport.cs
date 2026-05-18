using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports
{
    public class StocksListReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public byte[]? LogoData { get; set; }

        public string? ReportTitle { get; set; }
        public string? PrintedText { get; set; }
        public bool ShowStores => Stores != null && Stores.Count > 0;
        public List<StoreInfo>? Stores { get; set; }

    }

    public class StoreInfo
    {
        public string? StoreID { get; set; }
        public string? StoreDescription { get; set; }
        public List<ProductInfo>? ProductInfos { get; set; }
    }

    public class ProductInfo
    {
        public string? ID { get; set; }
        public string? Description { get; set; }
        public string? ProductTypeDescription { get; set; }
        public string? UM { get; set; }
        public string? Available { get; set; }
        public string? Stock { get; set; }
        public decimal Engaged { get; set; }
        public decimal Ordered { get; set; }
        public bool ShowLots => Lots != null;

        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }

        public List<LotInfo>? Lots { get; set; }
        public List<store_movements>? Movements { get; set; }
        public List<store_stocks_engage>? Engages { get; set; }
    }
    public class LotInfo
    {
        public string? Lot { get; set; }
        public string? SupplierLot { get; set; }
        public string? Location { get; set; }
        public string? Expire { get; set; }
        public string? UM { get; set; }
        public string? Available { get; set; }
        public string? Stock { get; set; }
        public decimal Engaged { get; set; }
    }
}
