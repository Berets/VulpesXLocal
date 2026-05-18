using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class BOLLD00F1
    {
        public string? UM { get; set; }
        public string? RowUM { get; set; }
        public decimal RowQuantity { get; set; }
        private store_stocks_lots? lot;
        public store_stocks_lots? Lot
        {
            get => lot; set
            {
                lot = value;
                Product = value?.Product;
                NotifyPropertyChanged("Product");
                NotifyPropertyChanged("Lot");
            }
        }
        private tab_articolo? product;
        public tab_articolo? Product
        {
            get => product;
            set
            {
                product = value;
                NotifyPropertyChanged("Product");
            }
        }
        private ObservableCollection<store_stocks_lots>? lots;
        public ObservableCollection<store_stocks_lots>? Lots { get => lots; set { lots = value; NotifyPropertyChanged("Lots"); } }
    }
}
