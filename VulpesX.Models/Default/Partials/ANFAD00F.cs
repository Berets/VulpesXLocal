using System.Collections.ObjectModel;
using System.ComponentModel;
 

namespace VulpesX.Models.Default
{
    public partial class ANFAD00F
    {
        #region Class
        public ANFAD00F()
        {
            PropertyChanged += ANFAD00F_PropertyChanged;
        }

        private void ANFAD00F_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AFDREDD" || e.PropertyName == "AFDTRED" ||
                e.PropertyName == "AFDTMAR")
            {
                ComputeValues();
                NotifyPropertyChanged("AFDPREZ");
                NotifyPropertyChanged("AFDCOST");
                NotifyPropertyChanged("MarginTypeVisibility");
                NotifyPropertyChanged("TotalMargin");
            }
        }
        #endregion

        public ObservableCollection<ANFADD00F>? DetailsRows { get; set; }
        public bool MarginTypeVisibility => AFDTRED == "P" ? true : false;
        public void ComputeValues()
        {
            // total cost from details
            AFDCOST = DetailsRows?.Sum(sum => sum.AFDDCOST) ?? 0;
            // compute profitability
            if (!string.IsNullOrWhiteSpace(AFDTRED) && AFDREDD.HasValue && AFDREDD.Value > 0)
            {
                if (AFDTRED == "P")
                {
                    // percentage
                    if (AFDTMAR == "U")
                        AFDPREZ = AFDCOST + AFDCOST * AFDREDD.Value / 100;
                    else
                        AFDPREZ = AFDCOST / (1 - AFDREDD.Value / 100);
                }
                else
                {
                    // value
                    AFDPREZ = AFDCOST + AFDREDD.Value;
                }
            }
            NotifyPropertyChanged("TotalMargin");
        }

        public void ComputeBack()
        {
            // total cost from details
            AFDCOST = DetailsRows?.Sum(sum => sum.AFDDCOST) ?? 0;
            // compute profitability
            if (!string.IsNullOrWhiteSpace(AFDTRED) && AFDREDD.HasValue && AFDREDD.Value > 0)
            {
                if (AFDTRED == "P")
                {
                    // percentage
                    if (AFDTMAR == "U")
                        AFDREDD = TotalMargin / AFDCOST * 100;
                    else
                        AFDREDD = TotalMargin / AFDPREZ * 100;
                }
            }
        }

        public decimal TotalMargin => AFDPREZ - AFDCOST;

        public ObservableCollection<GenericIDDescription> ProfitTypes => CommonsService.StandardValueTypes;
        public ObservableCollection<GenericIDDescription> MarginTypes => CommonsService.MarginTypes;

        #region Product and UM
        private ObservableCollection<tab_articolo>? products;
        public ObservableCollection<tab_articolo>? Products
        {
            get => products;
            set
            {
                products = value;
                if (!string.IsNullOrWhiteSpace(AFDCODA))
                    Product = products?.Where(w => w.ID == AFDCODA).FirstOrDefault();
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
                    product = value;

                    UMs = new ObservableCollection<GenericIDDescription>(UMsCache.Where(w => w.ID == value?.UnitaID || w.ID == value?.UnitaIDAlt));
                    AFDUNIM = product != null ? product.UnitaID ?? AFDUNIM : AFDUNIM;
                    AFDCODA = product != null ? product.ID : string.Empty;

                    NotifyPropertyChanged("Product");
                    NotifyPropertyChanged("AFDUNIM");
                    NotifyPropertyChanged("UMs");
                }
            }
        }

        public ObservableCollection<GenericIDDescription>? UMs { get; set; }
        public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }
        #endregion
    }
}
