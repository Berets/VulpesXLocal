using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class acq_goods_receipts
    {
        public string? ProductDescription { get; set; }
        public string? SupplierDescription { get; set; }
        public string? StoreDescription { get; set; }
        public string? CausalDescription { get; set; }
        public decimal OrderedQuantity { get; set; }
        public string? UM { get; set; }

        #region Store
        private store_stores? store;
        public store_stores? Store
        {
            get => store;
            set
            {
                if (store?.id != value?.id)
                {
                    store_id = value?.id;
                    store = value;
                    NotifyPropertyChanged("Store");
                }
            }
        }

        private ObservableCollection<store_stores>? stores;
        public ObservableCollection<store_stores>? Stores
        {
            get => stores;
            set
            {
                stores = value;
                if (!string.IsNullOrWhiteSpace(store_id))
                    Store = stores?.Where(w => w.id == store_id).FirstOrDefault();
                else
                    Store = null;
                NotifyPropertyChanged("Stores");
            }
        }
        #endregion

        #region Causal
        private store_causals? causal;
        public store_causals? Causal
        {
            get => causal;
            set
            {
                if (causal?.id != value?.id)
                {
                    causal_id = value?.id;
                    causal = value;
                    NotifyPropertyChanged("Causal");
                }
            }
        }

        private ObservableCollection<store_causals>? causals;
        public ObservableCollection<store_causals>? Causals
        {
            get => causals;
            set
            {
                causals = value;
                if (!string.IsNullOrWhiteSpace(causal_id))
                    Causal = causals?.Where(w => w.id == causal_id).FirstOrDefault();
                else
                    Causal = null;
                NotifyPropertyChanged("Causals");
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
