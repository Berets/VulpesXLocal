using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class ACC_ASSETS_CARDS
    {
        public int DetailsCount { get; set; }
        public bool HasDetails => DetailsCount > 0;
        public string? TypeDescription { get; set; }
        public string? CategoryDescription { get; set; }
        public string? FullAccountDescription { get; set; }

        #region StartValue
        public decimal StartValuePlus { get; set; }
        public decimal StartValueMinus { get; set; }
        public decimal StartValueBalance => StartValuePlus - StartValueMinus;
        #endregion
        #region FiscalValue
        public decimal FiscalValuePlus { get; set; }
        public decimal FiscalValueMinus { get; set; }
        public decimal FiscalHistoryValue { get; set; }
        public decimal FiscalValueBalance => FiscalHistoryValue + (FiscalValuePlus - FiscalValueMinus);
        #endregion
        #region CivilValue
        public decimal CivilValuePlus { get; set; }
        public decimal CivilValueMinus { get; set; }
        public decimal CivilHistoryValue { get; set; }
        public decimal CivilValueBalance => CivilHistoryValue + (CivilValuePlus - CivilValueMinus);
        #endregion

        public string? SuspensionDescription => CommonsService.AccountingAssetsSuspensions.Where(w => w.ID == besosp).First()?.Description;

        #region Asset type
        private ObservableCollection<ACC_ASSETS_TYPES>? typesList;
        public ObservableCollection<ACC_ASSETS_TYPES>? TypesList
        {
            get { return typesList; }
            set
            {
                typesList = value;
                if (!string.IsNullOrWhiteSpace(betice))
                    SelectedType = typesList?.Where(w => w.JTICE == betice).FirstOrDefault();
                else
                    SelectedType = null;
                NotifyPropertyChanged("TypesList");
            }
        }

        private ACC_ASSETS_TYPES? selectedType;

        public ACC_ASSETS_TYPES? SelectedType
        {
            get => selectedType;
            set
            {
                if (selectedType?.JTICE != value?.JTICE)
                {
                    betice = value?.JTICE;
                    selectedType = value;
                    NotifyPropertyChanged("SelectedType");
                }
            }
        }
        #endregion
        #region Cost center
        private ObservableCollection<TCECO00F>? costCentersList;
        public ObservableCollection<TCECO00F>? CostCentersList
        {
            get { return costCentersList; }
            set
            {
                costCentersList = value;
                if (!string.IsNullOrWhiteSpace(becdf))
                    SelectedCostCenter = costCentersList?.Where(w => w.cecodc == becdf).FirstOrDefault();
                else
                    SelectedCostCenter = null;
                NotifyPropertyChanged("CostCentersList");
            }
        }

        private TCECO00F? selectedCostCenter;

        public TCECO00F? SelectedCostCenter
        {
            get => selectedCostCenter;
            set
            {
                if (selectedCostCenter?.cecodc != value?.cecodc)
                {
                    becdf = value?.cecodc;
                    selectedCostCenter = value;
                    NotifyPropertyChanged("SelectedCostCenter");
                }
            }
        }
        #endregion
        #region Supplier
        private ObservableCollection<ABE>? suppliers;
        public ObservableCollection<ABE>? Suppliers
        {
            get { return suppliers; }
            set
            {
                suppliers = value;
                if (beprfo.HasValue && beprfo.Value > 0)
                    SelectedSupplier = suppliers?.Where(w => w.abecod == beprfo).FirstOrDefault();
                else
                    SelectedSupplier = null;
                NotifyPropertyChanged("Suppliers");
            }
        }

        private ABE? selectedSupplier;

        public ABE? SelectedSupplier
        {
            get => selectedSupplier;
            set
            {
                if (selectedSupplier?.abecod != value?.abecod)
                {
                    beprfo = value?.abecod;
                    selectedSupplier = value;
                    NotifyPropertyChanged("SelectedSupplier");
                }
            }
        }
        #endregion
        #region Asset category
        private ObservableCollection<ACC_ASSETS_CATEGORIES>? categoriesList;
        public ObservableCollection<ACC_ASSETS_CATEGORIES>? CategoriesList
        {
            get { return categoriesList; }
            set
            {
                categoriesList = value;
                if (!string.IsNullOrWhiteSpace(becat))
                    SelectedCategory = categoriesList?.Where(w => w.jcateg == becat).FirstOrDefault();
                else
                    SelectedCategory = null;
                NotifyPropertyChanged("CategoriesList");
            }
        }

        private ACC_ASSETS_CATEGORIES? selectedCategory;

        public ACC_ASSETS_CATEGORIES? SelectedCategory
        {
            get => selectedCategory;
            set
            {
                if (selectedCategory?.jcateg != value?.jcateg)
                {
                    becat = value?.jcateg;
                    selectedCategory = value;
                    NotifyPropertyChanged("SelectedCategory");
                }
            }
        }
        #endregion
        #region Branch
        private ObservableCollection<FILIALI>? branches;
        public ObservableCollection<FILIALI>? Branches
        {
            get { return branches; }
            set
            {
                branches = value;
                if (bztras.HasValue && bztras.Value > 0)
                    SelectedBranch = branches?.Where(w => w.filcod == bztras).FirstOrDefault();
                else
                    SelectedBranch = null;
                NotifyPropertyChanged("Branches");
            }
        }

        private FILIALI? selectedBranch;

        public FILIALI? SelectedBranch
        {
            get => selectedBranch;
            set
            {
                if (selectedBranch?.filcod != value?.filcod)
                {
                    bztras = value?.filcod;
                    selectedBranch = value;
                    NotifyPropertyChanged("SelectedBranch");
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
