using System.Collections.ObjectModel;
using System.ComponentModel;
 
using VulpesX.Shared.Utilities;

namespace VulpesX.Models.Default;

public partial class OFFED00F
{
    public OFFED00F()
    {
        PropertyChanged += OFFED00F_PropertyChanged;
    }

    private void OFFED00F_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "OFDQTAV" || e.PropertyName == "OFDTQTA" ||
            e.PropertyName == "OFDTPRE" || e.PropertyName == "OFDPREZ" ||
            e.PropertyName == "OFDSCO1" || e.PropertyName == "OFDSCO2" ||
            e.PropertyName == "OFDSCO3" || e.PropertyName == "OFDTSC1" ||
            e.PropertyName == "OFDTSC2" || e.PropertyName == "OFDTSC3" ||
            e.PropertyName == "OFDMAGG" || e.PropertyName == "OFDTMAG")
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

    public bool CanEdit => string.IsNullOrWhiteSpace(OFDSTA);
    public bool CanChangeProduct => IsHeadReadonly ? false : !OFDQTAEV.HasValue;
    public bool IsHeadReadonly { get; set; }
    public DateTime OfferDate { get; set; }
    public int CustomerID { get; set; }
    public int RecipientID { get; set; }

    #region QuantityValue
    public decimal QuantityValue
    {
        get => OFDQTAV;
        set
        {
            OFDQTAV = value;
            NotifyPropertyChanged("QuantityValue");
            OnQuantityValueChanged(EventArgs.Empty);
        }
    }
    #endregion

    public string? CustomerCode { get; set; }
    public decimal CustomerDiscount { get; set; }
    public bool PrintProductNote { get; set; }
    public bool PrintAgentsInDetails { get; set; }
    public bool HasDACO => OFDDACO.HasValue;
    public ObservableCollection<GenericIDDescription>? UMs { get; set; }
    public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }
    public ObservableCollection<GenericIDDescription> QuantityTypes => CommonsService.StandardQuantityTypes;
    public string? QuantityTypeDescription => QuantityTypes.Where(w => w.ID == OFDTQTA).FirstOrDefault()?.Description;
    public ObservableCollection<GenericIDDescription> PriceTypes => CommonsService.StandardPriceTypes;
    public string? PriceTypeDescription => PriceTypes.Where(w => w.ID == OFDTPRE).FirstOrDefault()?.Description;
    public ObservableCollection<GenericIDDescription> DiscountTypes => CommonsService.StandardValueTypes;
    public tab_articolo? Product { get; set; }
    public tab_articolo_unita? UM { get; set; }
    public string? CausalNotes { get; set; }

    #region Computed
    public string DiscountText => $"{(OFDSCO1.HasValue && OFDSCO1.Value > 0 ? OFDSCO1.Value.ToString("N2") + (OFDTSC1 == "V" ? "€" : "%") : " ")} {(OFDSCO2.HasValue && OFDSCO2.Value > 0 ? OFDSCO2.Value.ToString("N2") + (OFDTSC2 == "V" ? "€" : "%") : " ")} {(OFDSCO3.HasValue && OFDSCO3.Value > 0 ? OFDSCO3.Value.ToString("N2") + (OFDTSC3 == "V" ? "€" : "%") : " ")}";
    public string SurchargeText => $"{(OFDMAGG.HasValue && OFDMAGG.Value > 0 ? OFDMAGG.Value.ToString("N2") + (OFDTMAG == "V" ? "€" : "%") : " ")}";
    public decimal AmountDisplay => OFDTQTA == "O" ? 0 : Amount;
    public decimal NetPriceDisplay => OFDTQTA == "O" ? 0 : NetPrice;
    public decimal Amount
    {
        get
        {
            decimal result = 0;
            if (OFDTPRE == "U")
            {
                result = Math.Round(OFDPREZ * OFDQTAV, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                if (OFDTPRE == "V")
                {
                    result = OFDPREZ;
                }
            }
            if (OFDTQTA == "V" || OFDTQTA == "O")
            {
                return result;
            }
            else
            {
                if (OFDTQTA == "T")
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
            return AccountingHelper.ComputePrice(Amount, OFDSCO1, OFDTSC1, OFDSCO2, OFDTSC2, OFDSCO3, OFDTSC3, OFDMAGG, OFDTMAG).NetPrice;
        }
    }
    public decimal Discount
    {
        get
        {
            return AccountingHelper.ComputePrice(Amount, OFDSCO1, OFDTSC1, OFDSCO2, OFDTSC2, OFDSCO3, OFDTSC3, OFDMAGG, OFDTMAG).DiscountValue;
        }
    }
    public decimal Surcharge
    {
        get
        {
            return AccountingHelper.ComputePrice(Amount, OFDSCO1, OFDTSC1, OFDSCO2, OFDTSC2, OFDSCO3, OFDTSC3, OFDMAGG, OFDTMAG).SurchargeValue;
        }
    }
    public decimal VAT
    {
        get
        {
            decimal rate = 0;
            decimal.TryParse(OFDALIV, out rate);
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
            if (!string.IsNullOrWhiteSpace(OFDCODA) && products != null)
                SelectedProduct = products.Where(w => w.ID == OFDCODA).FirstOrDefault();
            else
                SelectedProduct = null;
            NotifyPropertyChanged("SelectedProduct");
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
                ofdunim = selectedProduct != null || string.IsNullOrWhiteSpace(ofdunim) ? value?.UnitaID : ofdunim;
                if (GroupsList != null && !string.IsNullOrWhiteSpace(value?.GroupID))
                {
                    SelectedGroup = GroupsList.Where(w => w.P1GRUP == value?.GroupID).FirstOrDefault();
                    SelectedAccount = AccountsList?.Where(w => w.P1GRUP == value?.GroupID && w.P2CONT == value?.AccountID).FirstOrDefault();
                    SelectedSubaccount = SubaccountsList?.Where(w => w.P1GRUP == value?.GroupID && w.P2CONT == value?.AccountID && w.P3SOTC == value?.SubaccountID).FirstOrDefault();
                }
                OFDCODA = (value != null) ? value.ID : string.Empty;
                selectedProduct = value;
                if (ratesList != null && !string.IsNullOrWhiteSpace(value?.asscod) && !string.IsNullOrWhiteSpace(value?.assali))
                {
                    if (string.IsNullOrWhiteSpace(OFDASSF) && string.IsNullOrWhiteSpace(OFDALIV))
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
            if (!string.IsNullOrWhiteSpace(OFDGRUP))
                SelectedGroup = groupsList?.Where(w => w.P1GRUP == OFDGRUP).FirstOrDefault();
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
            if (!string.IsNullOrWhiteSpace(OFDGRUP) && !string.IsNullOrWhiteSpace(OFDCONT))
                SelectedAccount = accountsList?.Where(w => w.P1GRUP == OFDGRUP && w.P2CONT == OFDCONT).FirstOrDefault();
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
            if (!string.IsNullOrWhiteSpace(OFDGRUP) && !string.IsNullOrWhiteSpace(OFDCONT) && !string.IsNullOrWhiteSpace(OFDSCTO))
                SelectedSubaccount = subaccountsList?.Where(w => w.P1GRUP == OFDGRUP && w.P2CONT == OFDCONT && w.P3SOTC == OFDSCTO).FirstOrDefault();
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
                OFDGRUP = value?.P1GRUP;
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
                    SubaccountsList = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (w.p3soci == null || w.p3soci == oftsoci)).ToList());
                }
                else
                {
                    SubaccountsList = null;
                }
                if (selectedAccount != null)
                {
                    SelectedSubaccount = null;
                }
                OFDCONT = value?.P2CONT;
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
                OFDSCTO = value?.P3SOTC;
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
            if (!string.IsNullOrWhiteSpace(OFDALIV) && !string.IsNullOrWhiteSpace(OFDASSF))
                SelectedRate = ratesList?.Where(w => w.assali == OFDALIV && w.asscod == OFDASSF).FirstOrDefault();
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
                OFDASSF = value?.asscod;
                OFDALIV = value?.assali;
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
            if (!string.IsNullOrWhiteSpace(OFDCOA1))
                SelectedFirstAgent = agentsList?.Where(w => w.agecod == OFDCOA1).FirstOrDefault();
            else
                SelectedFirstAgent = null;
            if (!string.IsNullOrWhiteSpace(OFDCOA2))
                SelectedSecondAgent = agentsList?.Where(w => w.agecod == OFDCOA2).FirstOrDefault();
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
                OFDCOA1 = value?.agecod;
                OFDCOA1P = !OFDCOA1P.HasValue || selectedFirstAgent != null ? value?.agepvg : OFDCOA1P;
                OFDCOA1PT = string.IsNullOrWhiteSpace(OFDCOA1PT) || selectedFirstAgent != null ? value?.agepvgt : OFDCOA1PT;
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
                OFDCOA2 = value?.agecod;
                OFDCOA2P = !OFDCOA2P.HasValue || selectedSecondAgent != null ? value?.agepvg : OFDCOA2P;
                OFDCOA2PT = string.IsNullOrWhiteSpace(OFDCOA2PT) || selectedSecondAgent != null ? value?.agepvgt : OFDCOA2PT;
                selectedSecondAgent = value;
                NotifyPropertyChanged("SelectedSecondAgent");
            }
        }
    }
    #endregion

    public bool IsReadOnlyRow => transformed.HasValue;
}
