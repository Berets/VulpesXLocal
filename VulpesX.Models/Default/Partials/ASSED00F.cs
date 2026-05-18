using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ASSED00F
    {
        public decimal? QuantityAvailable { get; set; } 
        public event EventHandler? ProductChanged;
        protected void OnProductChanged(EventArgs e)
        {
            EventHandler? handler = ProductChanged;
            if (handler != null)
                handler(this, e);
        }

        #region Product
        private tab_articolo? product;
        public tab_articolo? Product
        {
            get => product;
            set
            {
                if (product?.ID != value?.ID)
                {
                    product_id = value?.ID;
                    product = value;
                    NotifyPropertyChanged("Product");
                    OnProductChanged(EventArgs.Empty);
                }
            }
        }

        private ObservableCollection<tab_articolo>? products;
        public ObservableCollection<tab_articolo>? Products
        {
            get => products;
            set
            {
                products = value;
                if (!string.IsNullOrWhiteSpace(product_id))
                    Product = products?.Where(w => w.ID == product_id).FirstOrDefault();
                else
                    Product = null;
                NotifyPropertyChanged("Products");
            }
        }
        #endregion
    }
}
