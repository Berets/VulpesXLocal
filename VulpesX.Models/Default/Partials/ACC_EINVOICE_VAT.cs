using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class ACC_EINVOICE_VAT
    {
        public string? CollectabilityDescription => CommonsService.FECollectabilities.Where(w => w.ID == fattesi).FirstOrDefault()?.FullDescriptionNotSearchable;
        public bool AlreadyCreatedPNRow { get; set; }

        #region Rates
        public string? RateCode { get; set; }
        public string? RateValue { get; set; }
        private ObservableCollection<ASSOGGETAMENTI>? rates;
        public ObservableCollection<ASSOGGETAMENTI>? Rates
        {
            get { return rates; }
            set
            {
                rates = value;
                if (!string.IsNullOrWhiteSpace(RateCode) && !string.IsNullOrWhiteSpace(RateValue))
                    SelectedRate = rates?.Where(w => w.assali == RateValue && w.asscod == RateCode).FirstOrDefault();
                else
                    SelectedRate = null;
                NotifyPropertyChanged("Rates");
            }
        }

        private ASSOGGETAMENTI? selectedRate;

        public ASSOGGETAMENTI? SelectedRate
        {
            get => selectedRate;
            set
            {
                if (selectedRate?.asscod != value?.asscod || selectedRate?.assali != value?.assali)
                {
                    RateCode = value?.asscod;
                    RateValue = value?.assali;
                    selectedRate = value;
                    NotifyPropertyChanged("SelectedRate");
                }
            }
        }
        #endregion

        #region Natura
        private ObservableCollection<FE_IVADOC>? natures;
        public ObservableCollection<FE_IVADOC>? Natures
        {
            get { return natures; }
            set
            {
                natures = value;
                if (!string.IsNullOrWhiteSpace(fattnatu))
                    SelectedNature = natures?.Where(w => w.FETICod == fattnatu).FirstOrDefault();
                else
                    SelectedNature = null;
                NotifyPropertyChanged("Natures");
            }
        }

        private FE_IVADOC? selectedNature;

        public FE_IVADOC? SelectedNature
        {
            get => selectedNature;
            set
            {
                if (selectedNature?.FETICod != value?.FETICod)
                {
                    fattnatu = value?.FETICod;
                    selectedNature = value;
                    NotifyPropertyChanged("SelectedNature");
                }
            }
        }
        #endregion
    }
}
