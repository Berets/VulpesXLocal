using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ASSOGGETAMENTI
    {
        public string FullDescriptionSearchable => $"{asscod?.Trim()} {assali} {assdes?.Trim()}";

        public string? NaturaFullDescriptionSearchable { get; set; }

        public bool asscliBool
        {
            get
            {
                return asscli == "S";
            }
            set
            {
                if (value)
                    asscli = "S";
                else
                    asscli = "N";
            }
        }

        public bool assforBool
        {
            get
            {
                return assfor == "S";
            }
            set
            {
                if (value)
                    assfor = "S";
                else
                    assfor = "N";
            }
        }

        public bool assomaBool
        {
            get
            {
                return assoma == "O";
            }
            set
            {
                if (value)
                    assoma = "O";
                else
                    assoma = null;
            }
        }

        public bool assplaBool
        {
            get
            {
                return asspla == "S";
            }
            set
            {
                if (value)
                    asspla = "S";
                else
                    asspla = "N";
            }
        }

        public bool asselcBool
        {
            get
            {
                return asselc == "S";
            }
            set
            {
                if (value)
                    asselc = "S";
                else
                    asselc = "N";
            }
        }

        public bool asselfBool
        {
            get
            {
                return asself == "S";
            }
            set
            {
                if (value)
                    asself = "S";
                else
                    asself = "N";
            }
        }

        public bool asscomivaBool
        {
            get
            {
                return asscomiva == "S";
            }
            set
            {
                if (value)
                    asscomiva = "S";
                else
                    asscomiva = "N";
            }
        }

        public bool asssplpayBool
        {
            get
            {
                return asssplpay == "S";
            }
            set
            {
                if (value)
                    asssplpay = "S";
                else
                    asssplpay = "N";
            }
        }

        #region Rates
        private ObservableCollection<ASSOGGETAMENTI>? ratesList;
        public ObservableCollection<ASSOGGETAMENTI>? RatesList
        {
            get { return ratesList; }
            set
            {
                ratesList = value;
                if (!string.IsNullOrWhiteSpace(assomacod) && !string.IsNullOrWhiteSpace(assomaali))
                    SelectedRate = ratesList?.Where(w => w.assali == assomaali && w.asscod == assomacod).FirstOrDefault();
                else
                    SelectedRate = null;
                if (!string.IsNullOrWhiteSpace(assventcod) && !string.IsNullOrWhiteSpace(assventali))
                    SelectedVentilatedRate = ratesList?.Where(w => w.assali == assventali && w.asscod == assventcod).FirstOrDefault();
                else
                    SelectedVentilatedRate = null;
                NotifyPropertyChanged("RatesList");
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
                    assomacod = value?.asscod;
                    assomaali = value?.assali;
                    selectedRate = value;
                    NotifyPropertyChanged("SelectedRate");
                }
            }
        }

        private ASSOGGETAMENTI? selectedVentilatedRate;
        public ASSOGGETAMENTI? SelectedVentilatedRate
        {
            get => selectedVentilatedRate;
            set
            {
                if (selectedVentilatedRate?.asscod != value?.asscod || selectedVentilatedRate?.assali != value?.assali)
                {
                    assventcod = value?.asscod;
                    assventali = value?.assali;
                    selectedVentilatedRate = value;
                    NotifyPropertyChanged("SelectedVentilatedRate");
                }
            }
        }
        #endregion
    }
}
