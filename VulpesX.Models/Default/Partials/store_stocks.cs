using System.Collections.ObjectModel;
using VulpesX.Models.Default.Partials;

namespace VulpesX.Models.Default
{
    public partial class store_stocks
    {
        public tab_articolo? Product { get; set; }
        public store_stores? Store { get; set; }
        public string? StoreDescription { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductTypeDescription { get; set; }
        public bool IsInfinite { get; set; }
        public string? UM { get; set; }
        public bool product_haslots { get; set; }
        public decimal? EngagedQuantity { get; set; }
        public string? QuantityDisplay
        {
            get
            {
                if (IsInfinite)
                    return "∞";
                else
                    return Info?.QuantityAvailable.ToString("N6");
            }
        }
        public string? QuantityStockDisplay
        {
            get
            {
                if (IsInfinite)
                    return "∞";
                else
                    return Info?.QuantityStock?.ToString("N6");
            }
        }
        public StockInfo Info { get; set; } = new StockInfo();
        private ObservableCollection<store_stocks_lots>? lots;
        public ObservableCollection<store_stocks_lots>? Lots { get => lots; set { lots = value; NotifyPropertyChanged("Lots"); } }
    }
}
