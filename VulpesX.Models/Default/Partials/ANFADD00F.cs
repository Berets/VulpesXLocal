using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ANFADD00F
    {
        public event EventHandler? ProductChanged;
        protected void OnProductChanged(EventArgs e)
        {
            EventHandler? handler = ProductChanged;
            if (handler != null)
                handler(this, e);
        }

        #region DetailType
        private ObservableCollection<TAB_CRM_FEASABILITY>? detailTypes;
        public ObservableCollection<TAB_CRM_FEASABILITY>? DetailTypes
        {
            get => detailTypes;
            set
            {
                detailTypes = value;
                if (!string.IsNullOrWhiteSpace(AFDDTIPO))
                    DetailType = detailTypes?.Where(w => w.tdecod == AFDDTIPO).FirstOrDefault();
                else
                    DetailType = null;
                NotifyPropertyChanged("DetailType");
                NotifyPropertyChanged("DetailTypes");
            }
        }

        private TAB_CRM_FEASABILITY? detailType;
        public TAB_CRM_FEASABILITY? DetailType
        {
            get => detailType;
            set
            {
                if (detailType?.tdecod != value?.tdecod)
                {
                    AFDDTIPO = (value != null) ? value.tdecod : string.Empty;
                    detailType = value;
                    NotifyPropertyChanged("DetailType");
                }
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
                if (!string.IsNullOrWhiteSpace(AFDDCODA))
                    Product = products?.Where(w => w.ID == AFDDCODA).FirstOrDefault();
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
                    AFDDCODA = value?.ID;
                    product = value;
                    NotifyPropertyChanged("Product");
                    OnProductChanged(EventArgs.Empty);
                }
            }
        }
        #endregion
    }
}
