using System.Collections.ObjectModel;
using System.ComponentModel;
 
using VulpesX.Shared.Utilities;

namespace VulpesX.Models.Default
{
    public partial class FATTD00F
    {
        public FATTD00F()
        {
            PropertyChanged += FATTD00F_PropertyChanged;
        }

        private void FATTD00F_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FDQTAV" || e.PropertyName == "FDTQTA" ||
                e.PropertyName == "FDTPRE" || e.PropertyName == "FDPREZ" ||
                e.PropertyName == "FDSCO1" || e.PropertyName == "FDSCO2" ||
                e.PropertyName == "FDSCO3" || e.PropertyName == "FDTSC1" ||
                e.PropertyName == "FDTSC2" || e.PropertyName == "FDTSC3" ||
                e.PropertyName == "FDMAGG" || e.PropertyName == "FDTMAG")
            {
                NotifyPropertyChanged("Amount");
                NotifyPropertyChanged("AmountDisplay");
                NotifyPropertyChanged("Discount");
                NotifyPropertyChanged("Surcharge");
            }
        }

        public event EventHandler? QuantityValueChanged;
        protected void OnQuantityValueChanged(EventArgs e)
        {
            EventHandler? handler = QuantityValueChanged;
            if (handler != null)
                handler(this, e);
        }

        public DateTime InvoiceDate { get; set; }
        public int CustomerID { get; set; }
        public int RecipientID { get; set; }

        #region QuantityValue
        public decimal QuantityValue
        {
            get => FDQTAV ?? 0;
            set
            {
                FDQTAV = value;
                NotifyPropertyChanged("QuantityValue");
                OnQuantityValueChanged(EventArgs.Empty);
            }
        }
        #endregion

        public FATTT00F? LinkedInvoiceHead { get; set; }
        public BOLLT00F? LinkedDDT { get; set; }
        public ORDIT00F? LinkedOrder { get; set; }
        public string? RecipientDescription { get; set; }
        public string? CustomerProductID { get; set; }
        public string? CustomerProductDescription { get; set; }
        public bool HasPlafond { get; set; }
        public bool PrintProductNote { get; set; }
        public bool PrintAgentsInDetails { get; set; }
        public bool HasDACO => FDDACO.HasValue;
        public string? CustomerCode { get; set; }
        public ASSOGGETAMENTI? Rate { get; set; }
        public tab_articolo? Product { get; set; }
        public tab_articolo_unita? UM { get; set; }
        public decimal CustomerDiscount { get; set; }
        public string? HeaderFootNote { get; set; }
        public string? DDTReferenceText { get; set; }
        public string? ODAReferenceText { get; set; }
        public string DiscountText => $"{(FDSCO1.HasValue && FDSCO1.Value > 0 ? FDSCO1.Value.ToString("N2") + (FDTSC1 == "V" ? "€" : "%") : " ")} {(FDSCO2.HasValue && FDSCO2.Value > 0 ? FDSCO2.Value.ToString("N2") + (FDTSC2 == "V" ? "€" : "%") : " ")} {(FDSCO3.HasValue && FDSCO3.Value > 0 ? FDSCO3.Value.ToString("N2") + (FDTSC3 == "V" ? "€" : "%") : " ")}";
        public string SurchargeText => $"{(FDMAGG.HasValue && FDMAGG.Value > 0 ? FDMAGG.Value.ToString("N2") + (FDTMAG == "V" ? "€" : "%") : " ")}";

        public ObservableCollection<GenericIDDescription>? UMs { get; set; }
        public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }
        public ObservableCollection<GenericIDDescription>? DiscountTypes => CommonsService.StandardValueTypes;
        public ObservableCollection<GenericIDDescription>? QuantityTypes => CommonsService.StandardQuantityTypesWithStamp;
        public ObservableCollection<GenericIntIDDescription>? StampTypes => CommonsService.StampTypes;
        public ObservableCollection<GenericIDDescription>? PriceTypes => CommonsService.StandardPriceTypes;

        public PAGCLI? PayementType { get; set; }

        #region Computed
        public decimal AmountDisplay => FDTQTA == "O" ? 0 : Amount;
        public decimal NetPriceDisplay => FDTQTA == "O" ? 0 : NetPrice;
        public decimal Amount
        {
            get
            {
                decimal result = 0;
                if (FDTPRE == "U")
                {
                    result = Math.Round((FDPREZ ?? 0) * (FDQTAV ?? 0), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    if (FDTPRE == "V")
                    {
                        result = FDPREZ ?? 0;
                    }
                }
                if (FDTQTA == "V" || FDTQTA == "O")
                {
                    return result;
                }
                else
                {
                    if (FDTQTA == "T")
                    {
                        return result * -1;
                    }
                    if (FDTQTA == "B")
                    {
                        if (FDSTAMP == 1)
                            return result;
                        else
                            return 0;
                    }
                }
                return 0;
            }
        }
        public decimal VAT
        {
            get
            {
                decimal rate = 0;
                decimal.TryParse(FDALIV, out rate);
                return Math.Round((NetPrice - NetPrice * CustomerDiscount / 100) * rate / 100, 2, MidpointRounding.AwayFromZero);
            }
        }
        public decimal NetPrice
        {
            get
            {
                return AccountingHelper.ComputePrice(Amount, FDSCO1, FDTSC1, FDSCO2, FDTSC2, FDSCO3, FDTSC3, FDMAGG, FDTMAG).NetPrice;
            }
        }
        public decimal NetPriceUnit
        {
            get
            {
                return AccountingHelper.ComputePrice(Amount, FDSCO1, FDTSC1, FDSCO2, FDTSC2, FDSCO3, FDTSC3, FDMAGG, FDTMAG).NetPrice / (FDQTAV ?? 0);
            }
        }
        public decimal Discount
        {
            get
            {
                return AccountingHelper.ComputePrice(Amount, FDSCO1, FDTSC1, FDSCO2, FDTSC2, FDSCO3, FDTSC3, FDMAGG, FDTMAG).DiscountValue;
            }
        }
        public decimal Surcharge
        {
            get
            {
                return AccountingHelper.ComputePrice(Amount, FDSCO1, FDTSC1, FDSCO2, FDTSC2, FDSCO3, FDTSC3, FDMAGG, FDTMAG).SurchargeValue;
            }
        }
        #endregion

        #region Product
        private ObservableCollection<tab_articolo>? products;
        public ObservableCollection<tab_articolo>? Products
        {
            get { return products; }
            set
            {
                products = value;
                if (!string.IsNullOrWhiteSpace(FDCODA))
                    SelectedProduct = products?.Where(w => w.ID == FDCODA).FirstOrDefault();
                else
                    SelectedProduct = null;
                NotifyPropertyChanged("Products");
            }
        }

        private tab_articolo? selectedProduct;
        public tab_articolo? SelectedProduct
        {
            get => selectedProduct;
            set
            {
                if (selectedProduct?.ID != value?.ID && UMsCache != null)
                {
                    // load UMs
                    UMs = new ObservableCollection<GenericIDDescription>(UMsCache.Where(w => w.ID == value?.UnitaID || w.ID == value?.UnitaIDAlt));
                    FDUNIM = selectedProduct != null || string.IsNullOrWhiteSpace(FDUNIM) ? value?.UnitaID : FDUNIM;
                    if (GroupsList != null && !string.IsNullOrWhiteSpace(value?.GroupID))
                    {
                        SelectedGroup = GroupsList.Where(w => w.P1GRUP == value?.GroupID).FirstOrDefault();
                        SelectedAccount = AccountsList?.Where(w => w.P1GRUP == value?.GroupID && w.P2CONT == value?.AccountID).FirstOrDefault();
                        SelectedSubaccount = SubaccountsList?.Where(w => w.P1GRUP == value?.GroupID && w.P2CONT == value?.AccountID && w.P3SOTC == value?.SubaccountID).FirstOrDefault();
                    }
                    FDCODA = value?.ID;
                    selectedProduct = value;
                    if (ratesList != null && !string.IsNullOrWhiteSpace(value?.asscod) && !string.IsNullOrWhiteSpace(value?.assali))
                    {
                        if (string.IsNullOrWhiteSpace(FDASSF) && string.IsNullOrWhiteSpace(FDALIV))
                            SelectedRate = ratesList.Where(w => w.asscod == value?.asscod && w.assali == value?.assali).FirstOrDefault();
                    }
                    if (costCentersList != null && !string.IsNullOrWhiteSpace(value?.costcenter_id))
                    {
                        if (string.IsNullOrWhiteSpace(FDCECO))
                            SelectedCostCenter = costCentersList.Where(w => w.cecodc == value?.costcenter_id).FirstOrDefault();
                    }
                    NotifyPropertyChanged("SelectedProduct");
                    NotifyPropertyChanged("UMs");
                }
            }
        }
        #endregion

        #region PDC
        public List<PDCCONTI>? AccountCache { get; set; }
        public List<PDCSOTTO>? SubaccountCache { get; set; }

        private ObservableCollection<PDCGRUPPI>? groupsList;
        public ObservableCollection<PDCGRUPPI>? GroupsList
        {
            get { return groupsList; }
            set
            {
                groupsList = value;
                if (!string.IsNullOrWhiteSpace(FDGRUP))
                    SelectedGroup = groupsList?.Where(w => w.P1GRUP == FDGRUP).FirstOrDefault();
                else
                    SelectedGroup = null;
                NotifyPropertyChanged("GroupsList");
            }
        }
        private ObservableCollection<PDCCONTI>? accountsList;
        public ObservableCollection<PDCCONTI>? AccountsList
        {
            get { return accountsList; }
            set
            {
                accountsList = value;
                if (!string.IsNullOrWhiteSpace(FDGRUP) && !string.IsNullOrWhiteSpace(FDCONT))
                    SelectedAccount = accountsList?.Where(w => w.P1GRUP == FDGRUP && w.P2CONT == FDCONT).FirstOrDefault();
                else
                    SelectedAccount = null;
                NotifyPropertyChanged("AccountsList");
            }
        }
        private ObservableCollection<PDCSOTTO>? subaccountsList;
        public ObservableCollection<PDCSOTTO>? SubaccountsList
        {
            get { return subaccountsList; }
            set
            {
                subaccountsList = value;
                if (!string.IsNullOrWhiteSpace(FDGRUP) && !string.IsNullOrWhiteSpace(FDCONT) && !string.IsNullOrWhiteSpace(FDSCTO))
                    SelectedSubaccount = subaccountsList?.Where(w => w.P1GRUP == FDGRUP && w.P2CONT == FDCONT && w.P3SOTC == FDSCTO).FirstOrDefault();
                else
                    SelectedSubaccount = null;
                NotifyPropertyChanged("SubaccountsList");
            }
        }

        private PDCGRUPPI? selectedGroup;
        public PDCGRUPPI? SelectedGroup
        {
            get
            {
                return selectedGroup;
            }
            set
            {
                if (selectedGroup?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        AccountsList = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsList = null;
                    }
                    if (selectedGroup != null)
                    {
                        SelectedAccount = null;
                        SelectedSubaccount = null;
                    }
                    FDGRUP = value?.P1GRUP;
                    selectedGroup = value;
                    NotifyPropertyChanged("SelectedGroup");
                }
            }
        }

        private PDCCONTI? selectedAccount;
        public PDCCONTI? SelectedAccount
        {
            get
            {
                return selectedAccount;
            }
            set
            {
                if (selectedAccount?.P1GRUP != value?.P1GRUP || selectedAccount?.P2CONT != value?.P2CONT)
                {

                    if (value != null && SubaccountCache != null)
                    {
                        SubaccountsList = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        SubaccountsList = null;
                    }
                    if (selectedAccount != null)
                    {
                        SelectedSubaccount = null;
                    }
                    FDCONT = value?.P2CONT;
                    selectedAccount = value;
                    NotifyPropertyChanged("SelectedAccount");
                }
            }
        }

        private PDCSOTTO? selectedSubaccount;
        public PDCSOTTO? SelectedSubaccount
        {
            get
            {
                return selectedSubaccount;
            }
            set
            {
                if (selectedSubaccount?.P1GRUP != value?.P1GRUP || selectedSubaccount?.P2CONT != value?.P2CONT || selectedSubaccount?.P3SOTC != value?.P3SOTC)
                {
                    FDSCTO = value?.P3SOTC;
                    selectedSubaccount = value;
                    NotifyPropertyChanged("SelectedSubaccount");
                }
            }
        }
        #endregion

        #region Rates
        private ObservableCollection<ASSOGGETAMENTI>? ratesList;
        public ObservableCollection<ASSOGGETAMENTI>? RatesList
        {
            get { return ratesList; }
            set
            {
                ratesList = value;
                if (!string.IsNullOrWhiteSpace(FDALIV) && !string.IsNullOrWhiteSpace(FDASSF))
                    SelectedRate = ratesList?.Where(w => w.assali == FDALIV && w.asscod == FDASSF).FirstOrDefault();
                else
                    SelectedRate = null;
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
                    FDASSF = value?.asscod;
                    FDALIV = value?.assali;
                    selectedRate = value;
                    if (!string.IsNullOrWhiteSpace(value?.assnatufe))
                        SelectedNatura = naturaList?.Where(w => w.FETICod == value?.assnatufe).FirstOrDefault();
                    NotifyPropertyChanged("SelectedRate");
                }
            }
        }
        #endregion

        #region Natura
        private ObservableCollection<FE_IVADOC>? naturaList;
        public ObservableCollection<FE_IVADOC>? NaturaList
        {
            get { return naturaList; }
            set
            {
                naturaList = value;
                if (!string.IsNullOrWhiteSpace(fdtiva))
                    SelectedNatura = naturaList?.Where(w => w.FETICod == fdtiva).FirstOrDefault();
                else
                    SelectedNatura = null;
                NotifyPropertyChanged("NaturaList");
            }
        }

        private FE_IVADOC? selectedNatura;

        public FE_IVADOC? SelectedNatura
        {
            get => selectedNatura;
            set
            {
                if (selectedNatura?.FETICod != value?.FETICod)
                {
                    fdtiva = value?.FETICod;
                    selectedNatura = value;
                    NotifyPropertyChanged("SelectedNatura");
                }
            }
        }
        #endregion

        #region Agents
        private ObservableCollection<AGENTI>? agents;
        public ObservableCollection<AGENTI>? Agents
        {
            get => agents;
            set
            {
                agents = value;
                if (!string.IsNullOrWhiteSpace(FDCOAG1))
                    FirstAgent = agents?.Where(w => w.agecod == FDCOAG1).FirstOrDefault();
                else
                    FirstAgent = null;
                if (!string.IsNullOrWhiteSpace(FDCOAG2))
                    SecondAgent = agents?.Where(w => w.agecod == FDCOAG2).FirstOrDefault();
                else
                    SecondAgent = null;
                NotifyPropertyChanged("Agents");
            }
        }

        private AGENTI? firstAgent;

        public AGENTI? FirstAgent
        {
            get => firstAgent;
            set
            {
                if (firstAgent?.agecod != value?.agecod)
                {
                    FDCOAG1 = value?.agecod;
                    FDPROV = !FDPROV.HasValue || firstAgent != null ? value?.agepvg : FDPROV;
                    FDCOAG1PT = string.IsNullOrWhiteSpace(FDCOAG1PT) || firstAgent != null ? value?.agepvgt : FDCOAG1PT;
                    firstAgent = value;
                    NotifyPropertyChanged("FirstAgent");
                }
            }
        }
        private AGENTI? secondAgent;

        public AGENTI? SecondAgent
        {
            get => secondAgent;
            set
            {
                if (secondAgent?.agecod != value?.agecod)
                {
                    FDCOAG2 = value?.agecod;
                    fdpro2 = !fdpro2.HasValue || secondAgent != null ? value?.agepvg : fdpro2;
                    FDCOAG2PT = string.IsNullOrWhiteSpace(FDCOAG2PT) || secondAgent != null ? value?.agepvgt : FDCOAG2PT;
                    secondAgent = value;
                    NotifyPropertyChanged("SecondAgent");
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
                if (!string.IsNullOrWhiteSpace(FDCECO))
                    SelectedCostCenter = costCentersList?.Where(w => w.cecodc == FDCECO).FirstOrDefault();
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
                    FDCECO = value?.cecodc;
                    selectedCostCenter = value;
                    NotifyPropertyChanged("SelectedCostCenter");
                }
            }
        }
        #endregion

        public DateTime? fddaor { get; set; }
        public int? fdcodc { get; set; }
        public string? fdartdise { get; set; }
        public string? FDTVEn { get; set; }
    }
}
