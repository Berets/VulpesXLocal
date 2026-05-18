using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class store_causals
    {
        public string FullDescriptionSearchable => $"{id} {description}";
        public string PrintFullDescription => $"[{sign}] {id} - {description}";
        public string? signDescription => CausalTypes.Where(w => w.ID == sign).First().Description;
        public ObservableCollection<GenericIDDescription> CausalTypes => CommonsService.StoreCausalTypes;

        #region Cost center
        private ObservableCollection<TCECO00F>? costCentersList;
        public ObservableCollection<TCECO00F>? CostCentersList
        {
            get { return costCentersList; }
            set
            {
                costCentersList = value;
                if (!string.IsNullOrWhiteSpace(cost_center_id))
                    SelectedCostCenter = costCentersList?.Where(w => w.cecodc == cost_center_id).FirstOrDefault();
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
                    cost_center_id = value?.cecodc;
                    selectedCostCenter = value;
                    NotifyPropertyChanged("SelectedCostCenter");
                }
            }
        }
        #endregion

        #region Linked causal
        private ObservableCollection<store_causals>? causalsList;
        public ObservableCollection<store_causals>? CausalsList
        {
            get { return causalsList; }
            set
            {
                causalsList = value;
                if (!string.IsNullOrWhiteSpace(link_causal_id))
                    SelectedLinkedCausal = causalsList?.Where(w => w.id == link_causal_id).FirstOrDefault();
                else
                    SelectedLinkedCausal = null;
                NotifyPropertyChanged("CausalsList");
            }
        }

        private store_causals? selectedLinkedCausal;

        public store_causals? SelectedLinkedCausal
        {
            get => selectedLinkedCausal;
            set
            {
                if (selectedLinkedCausal?.id != value?.id)
                {
                    link_causal_id = value?.id;
                    selectedLinkedCausal = value;
                    NotifyPropertyChanged("SelectedLinkedCausal");
                }
            }
        }
        #endregion

        #region Linked store
        private ObservableCollection<store_stores>? storesList;
        public ObservableCollection<store_stores>? StoresList
        {
            get { return storesList; }
            set
            {
                storesList = value;
                if (!string.IsNullOrWhiteSpace(link_store_id))
                    SelectedLinkedStore = storesList?.Where(w => w.id == link_store_id).FirstOrDefault();
                else
                    SelectedLinkedStore = null;
                NotifyPropertyChanged("StoresList");
            }
        }

        private store_stores? selectedLinkedStore;

        public store_stores? SelectedLinkedStore
        {
            get => selectedLinkedStore;
            set
            {
                if (selectedLinkedStore?.id != value?.id)
                {
                    link_store_id = value?.id;
                    selectedLinkedStore = value;
                    NotifyPropertyChanged("SelectedLinkedStore");
                }
            }
        }
        #endregion
    }
}
