using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VulpesX.Models.Default.Partials
{
    public class StockInfo : Base
    {
        public string? ProductID { get; set; }
        public string? Lot { get; set; }
        public string? Ubication { get; set; }
        public DateTime? Expire { get; set; }
        public string? StoreID { get; set; }
        public string? StoreDescription { get; set; }
        public string StoreFullDescriptionSearchable => $"{StoreID} {StoreDescription}";
        public decimal? QuantityStock { get; set; }
        public decimal? QuantityProduction { get; set; }
        public decimal? QuantityEngaged { get; set; }
        public decimal? QuantityEngagedForOrder { get; set; }
        private decimal quantityToEngage;
        public decimal QuantityToEngage
        {
            get => quantityToEngage;
            set
            {
                quantityToEngage = value;
                NotifyPropertyChanged("QuantityToEngage");
            }
        }
        public decimal? QuantityOrdered { get; set; }
        public string QuantityAvailableDisplay
        {
            get
            {
                if (QuantityAvailable == decimal.MaxValue)
                    return "∞";
                else
                    return QuantityAvailable.ToString("N6");
            }
        }
        public bool IncludeOrdered { get; set; }
        public decimal QuantityAvailable => (QuantityStock ?? 0) + (IncludeOrdered ? QuantityOrdered ?? 0 : 0) - (QuantityEngaged ?? 0);
        public ObservableCollection<store_stocks_lots>? Lots { get; set; }
    }
}
