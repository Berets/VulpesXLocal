using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class CRM_LISCLI
    {
        public ABE? Customer { get; set; }
        public string? ProductDescription { get; set; }
        public ObservableCollection<GenericIDDescription>? UMs { get; set; }
        public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }

        #region Product
        private ObservableCollection<tab_articolo>? products;
        public ObservableCollection<tab_articolo>? Products
        {
            get { return products; }
            set
            {
                products = value;
                if (!string.IsNullOrWhiteSpace(productID))
                    Product = products?.Where(w => w.ID == productID).FirstOrDefault();
                else
                    Product = null;
                NotifyPropertyChanged("Products");
            }
        }

        private tab_articolo? product;
        public tab_articolo? Product
        {
            get => product;
            set
            {
                if (product?.ID != value?.ID && UMsCache != null)
                {
                    // load UMs
                    UMs = new ObservableCollection<GenericIDDescription>(UMsCache.Where(w => w.ID == value?.UnitaID || w.ID == value?.UnitaIDAlt));
                    unit_id = product != null || string.IsNullOrWhiteSpace(unit_id) ? value?.UnitaID : unit_id;
                    productID = (value != null) ? value.ID : string.Empty;
                    product = value;
                    NotifyPropertyChanged("Product");
                    NotifyPropertyChanged("UMs");
                }
            }
        }
        #endregion

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion
    }
}
