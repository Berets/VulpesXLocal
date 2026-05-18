using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ASSET00F
    {
        public int DetailsCount { get; set; }

        #region Attachments
        public ObservableCollection<ASSETAL00F>? Attachments { get; set; }
        public string AssetAttachmentsStatusColor => AttachmentsCount > 0 ? "G": "O";
        public string AssetAttachmentsStatusText => AttachmentsCount > 0 ? $"Visualizza gli allegati di questo asset" : "Clicca per aggiungere allegati a questo asset";
        public int AttachmentsCount { get; set; }
        #endregion

        #region Contacts
        public ObservableCollection<ASSETCO00F>? Contacts { get; set; }
        public string AssetContactsStatusColor => ContactsCount > 0 ? "G": "O";
        public string AssetContactsStatusText => ContactsCount > 0 ? $"Visualizza i contatti per questo asset" : "Clicca per aggiungere contatti a questo asset";
        public int ContactsCount { get; set; }
        #endregion

        #region Getters
        public string FullDescriptionSearchable => $"{id} {description}";
        public string PrintFilename => $"AssetInfo {id}.pdf";
        public string DetailsColor => DetailsCount > 0 ? "G" : "O";
        #endregion

        #region Location
        private TAB_AST_LOCATIONS? location;
        public TAB_AST_LOCATIONS? Location
        {
            get => location;
            set
            {
                if (location?.id != value?.id)
                {
                    location_id = value?.id;
                    location = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }

        private ObservableCollection<TAB_AST_LOCATIONS>? locations;
        public ObservableCollection<TAB_AST_LOCATIONS>? Locations
        {
            get => locations;
            set
            {
                locations = value;
                if (!string.IsNullOrWhiteSpace(location_id))
                    Location = locations?.Where(w => w.id == location_id).FirstOrDefault();
                else
                    Location = null;
                NotifyPropertyChanged("Locations");
            }
        }
        #endregion

        #region Product
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
                }
            }
        }
        #endregion

        #region Customer
        private ABE? customer;
        public ABE? Customer
        {
            get => customer;
            set
            {
                if (customer?.abecod != value?.abecod)
                {
                    customer_id = value?.abecod;
                    customer = value;
                    NotifyPropertyChanged("Customer");
                }
            }
        }

        private ObservableCollection<ABE>? customers;
        public ObservableCollection<ABE>? Customers
        {
            get => customers;
            set
            {
                customers = value;
                if (customer_id.HasValue && customer_id.Value > 0)
                    Customer = customers?.Where(w => w.abecod == customer_id).FirstOrDefault();
                else
                    Customer = null;
                NotifyPropertyChanged("Customers");
            }
        }
        #endregion

        #region ResourceItem
        private tab_produzione_risorsa? resourceItem;
        public tab_produzione_risorsa? ResourceItem
        {
            get => resourceItem;
            set
            {
                if (resourceItem?.ID != value?.ID)
                {
                    resource_id = value?.ID;
                    resourceItem = value;
                    NotifyPropertyChanged("ResourceItem");
                }
            }
        }

        private ObservableCollection<tab_produzione_risorsa>? resourceItems;
        public ObservableCollection<tab_produzione_risorsa>? ResourceItems
        {
            get => resourceItems;
            set
            {
                resourceItems = value;
                if (!string.IsNullOrWhiteSpace(resource_id))
                    ResourceItem = resourceItems?.Where(w => w.ID == resource_id).FirstOrDefault();
                else
                    ResourceItem = null;
                NotifyPropertyChanged("ResourceItems");
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
