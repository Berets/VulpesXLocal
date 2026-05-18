using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class tab_articolo_tipo
    {
        public string FullDescriptionSearchable => $"{ID} {Descrizione}";


        #region DefaultStore
        private store_stores? defaultStore;
        public store_stores? DefaultStore
        {
            get => defaultStore;
            set
            {
                if (defaultStore?.id != value?.id)
                {
                    DefaultMagazzinoID = value?.id;
                    defaultStore = value;
                    NotifyPropertyChanged("DefaultStore");
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
                if (!string.IsNullOrWhiteSpace(DefaultMagazzinoID))
                    DefaultStore = stores?.Where(w => w.id == DefaultMagazzinoID).FirstOrDefault();
                else
                    DefaultStore = null;
                NotifyPropertyChanged("Stores");
            }
        }
        #endregion
    }
}
