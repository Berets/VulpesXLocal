using System.Collections.ObjectModel;
using System.ComponentModel;
 
using VulpesX.Shared.Utilities;

namespace VulpesX.Models.Default
{
    public partial class ORDID00F
    {
        public ORDID00F()
        {
            PropertyChanged += ORDID00F_PropertyChanged;
            FulfillmentID = "S";
        }

        private void ORDID00F_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ODQTAV" || e.PropertyName == "ODTQTA" ||
                e.PropertyName == "ODTPRE" || e.PropertyName == "ODPREZ" ||
                e.PropertyName == "ODSCO1" || e.PropertyName == "ODSCO2" ||
                e.PropertyName == "ODSCO3" || e.PropertyName == "ODTSC1" ||
                e.PropertyName == "ODTSC2" || e.PropertyName == "ODTSC3" ||
                e.PropertyName == "ODMAGG" || e.PropertyName == "ODTMAG")
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

        public ObservableCollection<GenericIDDescription> Fulfillments => CommonsService.StandardFulfillments;
        public int DDTCount { get; set; }
        public bool HasDDT => DDTCount > 0;
        public bool IsHeadReadonly { get; set; }
        public bool CanEdit { get; set; }
        public bool CanChangeProduct => IsHeadReadonly || !CanEdit ? false : !ODQTAEV.HasValue;
        public string? PaymentID { get; set; }
        public string? BankFullID { get; set; }
        public string? HeadStatusDescription { get; set; }
        public ABE? Customer { get; set; }
        public DESTINATARI? Recipient { get; set; }
        public TAB_CRM_CAUORD? Causal { get; set; }
        public pro_ordine? LinkedProductionOrder { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerID { get; set; }
        public int RecipientID { get; set; }

        #region QuantityValue
        public decimal QuantityValue
        {
            get => ODQTAV;
            set
            {
                ODQTAV = value;
                NotifyPropertyChanged("QuantityValue");
                OnQuantityValueChanged(EventArgs.Empty);
            }
        }
        #endregion

        public string? CustomerCode { get; set; }
        public decimal CustomerDiscount { get; set; }
        public bool PrintProductNote { get; set; }
        public bool PrintAgentsInDetails { get; set; }
        public bool HasDACO => ODDACO.HasValue;
        public ObservableCollection<GenericIDDescription>? UMs { get; set; }
        public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }
        public ObservableCollection<GenericIDDescription> QuantityTypes => CommonsService.StandardQuantityTypes;
        public ObservableCollection<GenericIDDescription> PriceTypes => CommonsService.StandardPriceTypes;
        public ObservableCollection<GenericIDDescription> DiscountTypes => CommonsService.StandardValueTypes;

        public tab_articolo? Product { get; set; }
        public tab_articolo_unita? UM { get; set; }
        public string? CausalNotes { get; set; }

        #region Computed
        public string DiscountText => $"{(ODSCO1.HasValue && ODSCO1.Value > 0 ? ODSCO1.Value.ToString("N2") + (ODTSC1 == "V" ? "€" : "%") : " ")} {(ODSCO2.HasValue && ODSCO2.Value > 0 ? ODSCO2.Value.ToString("N2") + (ODTSC2 == "V" ? "€" : "%") : " ")} {(ODSCO3.HasValue && ODSCO3.Value > 0 ? ODSCO3.Value.ToString("N2") + (ODTSC3 == "V" ? "€" : "%") : " ")}";
        public string SurchargeText => $"{(ODMAGG.HasValue && ODMAGG.Value > 0 ? ODMAGG.Value.ToString("N2") + (ODTMAG == "V" ? "€" : "%") : " ")}";
        public decimal AmountDisplay => ODTQTA == "O" ? 0 : Amount;
        public decimal NetPriceDisplay => ODTQTA == "O" ? 0 : NetPrice;
        public decimal Amount
        {
            get
            {
                decimal result = 0;
                if (ODTPRE == "U")
                {
                    result = Math.Round(ODPREZ * ODQTAV, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    if (ODTPRE == "V")
                    {
                        result = ODPREZ;
                    }
                }
                if (ODTQTA == "V" || ODTQTA == "O")
                {
                    return result;
                }
                else
                {
                    if (ODTQTA == "T")
                    {
                        return result * -1;
                    }
                }
                return 0;
            }
        }
        public decimal NetPrice
        {
            get
            {
                return AccountingHelper.ComputePrice(Amount, ODSCO1, ODTSC1, ODSCO2, ODTSC2, ODSCO3, ODTSC3, ODMAGG, ODTMAG).NetPrice;
            }
        }
        public decimal Discount
        {
            get
            {
                return AccountingHelper.ComputePrice(Amount, ODSCO1, ODTSC1, ODSCO2, ODTSC2, ODSCO3, ODTSC3, ODMAGG, ODTMAG).DiscountValue;
            }
        }
        public decimal Surcharge
        {
            get
            {
                return AccountingHelper.ComputePrice(Amount, ODSCO1, ODTSC1, ODSCO2, ODTSC2, ODSCO3, ODTSC3, ODMAGG, ODTMAG).SurchargeValue;
            }
        }
        public decimal VAT
        {
            get
            {
                decimal rate = 0;
                decimal.TryParse(ODALIV, out rate);
                return Math.Round((NetPrice - (NetPrice * CustomerDiscount / 100)) * rate / 100, 2, MidpointRounding.AwayFromZero);
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
                if (!string.IsNullOrWhiteSpace(ODCODA))
                    SelectedProduct = products?.Where(w => w.ID == ODCODA).FirstOrDefault();
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
                    odunit = selectedProduct != null || string.IsNullOrWhiteSpace(odunit) ? value?.UnitaID : odunit;
                    if (GroupsList != null && !string.IsNullOrWhiteSpace(value?.GroupID))
                    {
                        SelectedGroup = GroupsList.Where(w => w.P1GRUP == value?.GroupID).FirstOrDefault();
                        SelectedAccount = AccountsList?.Where(w => w.P1GRUP == value?.GroupID && w.P2CONT == value?.AccountID).FirstOrDefault();
                        SelectedSubaccount = SubaccountsList?.Where(w => w.P1GRUP == value?.GroupID && w.P2CONT == value?.AccountID && w.P3SOTC == value?.SubaccountID).FirstOrDefault();
                    }
                    ODCODA = (value != null) ? value.ID : string.Empty;
                    selectedProduct = value;
                    if (ratesList != null && !string.IsNullOrWhiteSpace(value?.asscod) && !string.IsNullOrWhiteSpace(value?.assali))
                    {
                        if (string.IsNullOrWhiteSpace(ODASSF) && string.IsNullOrWhiteSpace(ODALIV))
                            SelectedRate = ratesList.Where(w => w.asscod == value?.asscod && w.assali == value?.assali).FirstOrDefault();
                    }
                    NotifyPropertyChanged("SelectedProduct");
                    NotifyPropertyChanged("UMs");
                }
            }
        }
        #endregion

        #region PDC
        public ObservableCollection<PDCCONTI>? AllAccounts { get; set; }
        public ObservableCollection<PDCSOTTO>? AllSubccounts { get; set; }
        private ObservableCollection<PDCGRUPPI>? groupsList;
        public ObservableCollection<PDCGRUPPI>? GroupsList
        {
            get { return groupsList; }
            set
            {
                groupsList = value;
                if (!string.IsNullOrWhiteSpace(ODGRUP))
                    SelectedGroup = groupsList?.Where(w => w.P1GRUP == ODGRUP).FirstOrDefault();
                else
                    SelectedGroup = null;
                NotifyPropertyChanged("SelectedGroup");
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
                if (!string.IsNullOrWhiteSpace(ODGRUP) && !string.IsNullOrWhiteSpace(ODCONT))
                    SelectedAccount = accountsList?.Where(w => w.P1GRUP == ODGRUP && w.P2CONT == ODCONT).FirstOrDefault();
                else
                    SelectedAccount = null;
                NotifyPropertyChanged("SelectedAccount");
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
                if (!string.IsNullOrWhiteSpace(ODGRUP) && !string.IsNullOrWhiteSpace(ODCONT) && !string.IsNullOrWhiteSpace(ODSCTO))
                    SelectedSubaccount = subaccountsList?.Where(w => w.P1GRUP == ODGRUP && w.P2CONT == ODCONT && w.P3SOTC == ODSCTO).FirstOrDefault();
                else
                    SelectedSubaccount = null;
                NotifyPropertyChanged("SelectedSubaccount");
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
                    if (value != null && AllAccounts != null)
                    {
                        AccountsList = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
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
                    ODGRUP = value?.P1GRUP;
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

                    if (value != null && AllSubccounts != null)
                    {
                        SubaccountsList = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (w.p3soci == null || w.p3soci == otsoci)).ToList());
                    }
                    else
                    {
                        SubaccountsList = null;
                    }
                    if (selectedAccount != null)
                    {
                        SelectedSubaccount = null;
                    }
                    ODCONT = value?.P2CONT;
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
                    ODSCTO = value?.P3SOTC;
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
                if (!string.IsNullOrWhiteSpace(ODALIV) && !string.IsNullOrWhiteSpace(ODASSF))
                    SelectedRate = ratesList?.Where(w => w.assali == ODALIV && w.asscod == ODASSF).FirstOrDefault();
                else
                    SelectedRate = null;
                NotifyPropertyChanged("SelectedRate");
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
                    ODASSF = value?.asscod;
                    ODALIV = value?.assali;
                    selectedRate = value;
                    NotifyPropertyChanged("SelectedRate");
                }
            }
        }
        #endregion

        #region Agents
        private ObservableCollection<AGENTI>? agentsList;
        public ObservableCollection<AGENTI>? AgentsList
        {
            get { return agentsList; }
            set
            {
                agentsList = value;
                if (!string.IsNullOrWhiteSpace(ODCOA1))
                    SelectedFirstAgent = agentsList?.Where(w => w.agecod == ODCOA1).FirstOrDefault();
                else
                    SelectedFirstAgent = null;
                if (!string.IsNullOrWhiteSpace(ODCOA2))
                    SelectedSecondAgent = agentsList?.Where(w => w.agecod == ODCOA2).FirstOrDefault();
                else
                    SelectedSecondAgent = null;
                NotifyPropertyChanged("AgentsList");
                NotifyPropertyChanged("SelectedFirstAgent");
                NotifyPropertyChanged("SelectedSecondAgent");
            }
        }

        private AGENTI? selectedFirstAgent;

        public AGENTI? SelectedFirstAgent
        {
            get => selectedFirstAgent;
            set
            {
                if (selectedFirstAgent?.agecod != value?.agecod)
                {
                    ODCOA1 = value?.agecod;
                    ODCOA1P = !ODCOA1P.HasValue || selectedFirstAgent != null ? value?.agepvg : ODCOA1P;
                    ODCOA1PT = string.IsNullOrWhiteSpace(ODCOA1PT) || selectedFirstAgent != null ? value?.agepvgt : ODCOA1PT;
                    selectedFirstAgent = value;
                    NotifyPropertyChanged("SelectedFirstAgent");
                }
            }
        }
        private AGENTI? selectedSecondAgent;

        public AGENTI? SelectedSecondAgent
        {
            get => selectedSecondAgent;
            set
            {
                if (selectedSecondAgent?.agecod != value?.agecod)
                {
                    ODCOA2 = value?.agecod;
                    ODCOA2P = !ODCOA2P.HasValue || selectedSecondAgent != null ? value?.agepvg : ODCOA2P;
                    ODCOA2PT = string.IsNullOrWhiteSpace(ODCOA2PT) || selectedSecondAgent != null ? value?.agepvgt : ODCOA2PT;
                    selectedSecondAgent = value;
                    NotifyPropertyChanged("SelectedSecondAgent");
                }
            }
        }
        #endregion

        public decimal QuantityToProduce
        {
            get
            {
                if (UM?.ID == Product?.UnitaID)
                {
                    return ODQTAV;
                }
                else
                {
                    return ODQTAV * ((Product != null) ? (Product.QuantitaDefault ?? 1) : 1);
                }
            }
        }
        public decimal OriginalQuantity { get; set; }
        public string FulfillmentID { get; set; }
        public string? FulfillmentDescription => CommonsService.StandardFulfillments.Where(w => w.ID == FulfillmentID).FirstOrDefault()?.Description;
        public decimal QuantityToSend { get; set; }
    }
}