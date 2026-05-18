using System.Collections.ObjectModel;
using System.ComponentModel;
 
using VulpesX.Shared.Utilities;

namespace VulpesX.Models.Default
{
    public partial class BOLLD00F
    {
        public BOLLD00F()
        {
            PropertyChanged += BOLLD00F_PropertyChanged;
        }
        private void BOLLD00F_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BOQTAV" || e.PropertyName == "BOTQTA" ||
                e.PropertyName == "boprez" || e.PropertyName == "botpre" ||
                e.PropertyName == "bosco1" || e.PropertyName == "bosco2" ||
                e.PropertyName == "bosco3" || e.PropertyName == "botsc1" ||
                e.PropertyName == "botsc2" || e.PropertyName == "botsc3" ||
                e.PropertyName == "bomagg" || e.PropertyName == "botmag")
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

        public ObservableCollection<BOLLD00F1>? EngagesRows { get; set; }
        public bool HasLotsDetails => EngagesRows != null && EngagesRows.Count > 0;
        public DateTime? DDTDate { get; set; }
        public int CustomerID { get; set; }
        public decimal CustomerDiscount { get; set; }
        public int RecipientID { get; set; }
        public string? CustomerProductID { get; set; }
        public string? CustomerProductDescription { get; set; }
        public string? SupplierProductID { get; set; }
        public string? SupplierProductDescription { get; set; }
        public ORDIT00F? LinkedOrder { get; set; }

        #region QuantityValue
        public decimal QuantityValue
        {
            get => BOQTAV;
            set
            {
                BOQTAV = value;
                NotifyPropertyChanged("QuantityValue");
                OnQuantityValueChanged(EventArgs.Empty);
            }
        }
        #endregion

        #region Simple lists
        public ObservableCollection<GenericIDDescription>? DiscountTypes => CommonsService.StandardValueTypes;
        public ObservableCollection<GenericIDDescription>? QuantityTypes => CommonsService.StandardQuantityTypes;
        public ObservableCollection<GenericIDDescription>? PriceTypes => CommonsService.StandardPriceTypes;
        #endregion

        public bool PrintProductNote { get; set; }
        public bool PrintAgentsInDetails { get; set; }
        public string? CustomerCode { get; set; }
        public string? OrderReferenceText { get; set; }
        public string? HeaderFootNote { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal ResidualQuantity { get; set; }

        #region Computed
        public decimal AmountDisplay => BOTQTA == "O" ? 0 : Amount;
        public decimal NetPriceDisplay => BOTQTA == "O" ? 0 : NetPrice;
        public decimal Amount
        {
            get
            {
                decimal result = 0;
                if (botpre == "U")
                {
                    result = Math.Round(boprez * BOQTAV, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    if (botpre == "V")
                    {
                        result = boprez;
                    }
                }
                if (BOTQTA == "V" || BOTQTA == "O")
                {
                    return result;
                }
                else
                {
                    if (BOTQTA == "T")
                    {
                        return result * -1;
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
                decimal.TryParse(boaliq, out rate);
                return Math.Round((NetPrice - NetPrice * CustomerDiscount / 100) * rate / 100, 2, MidpointRounding.AwayFromZero);
            }
        }
        public decimal NetPrice
        {
            get
            {
                return AccountingHelper.ComputePrice(Amount, bosco1, botsc1, bosco2, botsc2, bosco3, botsc3, bomagg, botmag).NetPrice;
            }
        }
        public decimal Discount
        {
            get
            {
                return AccountingHelper.ComputePrice(Amount, bosco1, botsc1, bosco2, botsc2, bosco3, botsc3, bomagg, botmag).DiscountValue;
            }
        }
        public decimal Surcharge
        {
            get
            {
                return AccountingHelper.ComputePrice(Amount, bosco1, botsc1, bosco2, botsc2, bosco3, botsc3, bomagg, botmag).SurchargeValue;
            }
        }
        public string DiscountText => $"{(bosco1.HasValue && bosco1.Value > 0 ? bosco1.Value.ToString("N2") + (botsc1 == "V" ? "€" : "%") : " ")} {(bosco2.HasValue && bosco2.Value > 0 ? bosco2.Value.ToString("N2") + (botsc2 == "V" ? "€" : "%") : " ")} {(bosco3.HasValue && bosco3.Value > 0 ? bosco3.Value.ToString("N2") + (botsc3 == "V" ? "€" : "%") : " ")}";
        public string SurchargeText => $"{(bomagg.HasValue && bomagg.Value > 0 ? bomagg.Value.ToString("N2") + (botmag == "V" ? "€" : "%") : " ")}";
        #endregion

        #region Product
        private ObservableCollection<tab_articolo>? products;
        public ObservableCollection<tab_articolo>? Products
        {
            get { return products; }
            set
            {
                products = value;
                if (!string.IsNullOrWhiteSpace(BOCODA))
                    Product = products?.Where(w => w.ID == BOCODA).FirstOrDefault();
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
                    UMs = new ObservableCollection<GenericIDDescription>(UMsCache.Where(w => w.ID == value?.UnitaID || w.ID == value?.UnitaIDAlt));
                    BOUNIM = product != null || string.IsNullOrWhiteSpace(BOUNIM) ? value?.UnitaID : BOUNIM;
                    if (Groups != null && !string.IsNullOrWhiteSpace(value?.GroupID))
                    {
                        Group = Groups.Where(w => w.P1GRUP == value?.GroupID).FirstOrDefault();
                        Account = Accounts?.Where(w => w.P1GRUP == value?.GroupID && w.P2CONT == value?.AccountID).FirstOrDefault();
                        Subaccount = Subaccounts?.Where(w => w.P1GRUP == value?.GroupID && w.P2CONT == value?.AccountID && w.P3SOTC == value?.SubaccountID).FirstOrDefault();
                    }
                    BOCODA = value?.ID;
                    product = value;
                    if (rates != null && !string.IsNullOrWhiteSpace(value?.asscod) && !string.IsNullOrWhiteSpace(value?.assali))
                    {
                        if (string.IsNullOrWhiteSpace(boasso) && string.IsNullOrWhiteSpace(boaliq))
                            Rate = rates.Where(w => w.asscod == value?.asscod && w.assali == value?.assali).FirstOrDefault();
                    }
                    NotifyPropertyChanged("Product");
                    NotifyPropertyChanged("UMs");
                }
            }
        }

        public tab_articolo_unita? UM { get; set; }
        public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }
        public ObservableCollection<GenericIDDescription>? UMs { get; set; }
        #endregion

        #region Rates
        private ObservableCollection<ASSOGGETAMENTI>? rates;
        public ObservableCollection<ASSOGGETAMENTI>? Rates
        {
            get { return rates; }
            set
            {
                rates = value;
                if (!string.IsNullOrWhiteSpace(boaliq) && !string.IsNullOrWhiteSpace(boasso))
                    Rate = rates?.Where(w => w.assali == boaliq && w.asscod == boasso).FirstOrDefault();
                else
                    Rate = null;
                NotifyPropertyChanged("Rates");
            }
        }

        private ASSOGGETAMENTI? rate;

        public ASSOGGETAMENTI? Rate
        {
            get => rate;
            set
            {
                if (rate?.asscod != value?.asscod || rate?.assali != value?.assali)
                {
                    boasso = value?.asscod;
                    boaliq = value?.assali;
                    rate = value;
                    NotifyPropertyChanged("Rate");
                }
            }
        }
        #endregion

        #region Stores
        private ObservableCollection<store_stores>? stores;
        public ObservableCollection<store_stores>? Stores
        {
            get { return stores; }
            set
            {
                stores = value;
                if (!string.IsNullOrWhiteSpace(BOMAGA))
                    SelectedStore = stores?.Where(w => w.id == BOMAGA).FirstOrDefault();
                else
                    SelectedStore = null;
                NotifyPropertyChanged("StoreDescription");
            }
        }

        private store_stores? selectedStore;

        public store_stores? SelectedStore
        {
            get => selectedStore;
            set
            {
                if (selectedStore?.id != value?.id)
                {
                    BOMAGA = value?.id;
                    selectedStore = value;
                    NotifyPropertyChanged("SelectedStore");
                }
            }
        }

        #endregion

        #region PDC
        public ObservableCollection<PDCCONTI>? AllAccounts { get; set; }
        public ObservableCollection<PDCSOTTO>? AllSubccounts { get; set; }

        private ObservableCollection<PDCGRUPPI>? groups;
        public ObservableCollection<PDCGRUPPI>? Groups
        {
            get { return groups; }
            set
            {
                groups = value;
                if (!string.IsNullOrWhiteSpace(bogrup))
                    Group = groups?.Where(w => w.P1GRUP == bogrup).FirstOrDefault();
                else
                    Group = null;
                NotifyPropertyChanged("Groups");
            }
        }
        private ObservableCollection<PDCCONTI>? accounts;
        public ObservableCollection<PDCCONTI>? Accounts
        {
            get => accounts;
            set
            {
                accounts = value;
                if (!string.IsNullOrWhiteSpace(bogrup) && !string.IsNullOrWhiteSpace(bocont))
                    Account = accounts?.Where(w => w.P1GRUP == bogrup && w.P2CONT == bocont).FirstOrDefault();
                else
                    Account = null;
                NotifyPropertyChanged("Accounts");
            }
        }
        private ObservableCollection<PDCSOTTO>? subaccounts;
        public ObservableCollection<PDCSOTTO>? Subaccounts
        {
            get { return subaccounts; }
            set
            {
                subaccounts = value;
                if (!string.IsNullOrWhiteSpace(bogrup) && !string.IsNullOrWhiteSpace(bocont) && !string.IsNullOrWhiteSpace(bosotc))
                    Subaccount = subaccounts?.Where(w => w.P1GRUP == bogrup && w.P2CONT == bocont && w.P3SOTC == bosotc).FirstOrDefault();
                else
                    Subaccount = null;
                NotifyPropertyChanged("Subaccounts");
            }
        }

        private PDCGRUPPI? group;
        public PDCGRUPPI? Group
        {
            get => group;
            set
            {
                if (group?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        Accounts = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        Accounts = null;
                    }
                    if (group != null)
                    {
                        Account = null;
                        Subaccount = null;
                    }
                    bogrup = value?.P1GRUP;
                    group = value;
                    NotifyPropertyChanged("Group");
                }
            }
        }

        private PDCCONTI? account;
        public PDCCONTI? Account
        {
            get => account;
            set
            {
                if (account?.P1GRUP != value?.P1GRUP || account?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        Subaccounts = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (w.p3soci == null || w.p3soci == bolsoc)).ToList());
                    }
                    else
                    {
                        Subaccounts = null;
                    }
                    if (account != null)
                    {
                        Subaccount = null;
                    }
                    bocont = value?.P2CONT;
                    account = value;
                    NotifyPropertyChanged("Account");
                }
            }
        }

        private PDCSOTTO? subaccount;

        public PDCSOTTO? Subaccount
        {
            get => subaccount;
            set
            {
                if (subaccount?.P1GRUP != value?.P1GRUP || subaccount?.P2CONT != value?.P2CONT || subaccount?.P3SOTC != value?.P3SOTC)
                {
                    bosotc = value?.P3SOTC;
                    subaccount = value;
                    NotifyPropertyChanged("Subaccount");
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
                if (!string.IsNullOrWhiteSpace(BOCOA1))
                    FirstAgent = agents?.Where(w => w.agecod == BOCOA1).FirstOrDefault();
                else
                    FirstAgent = null;
                if (!string.IsNullOrWhiteSpace(BOCOA2))
                    SecondAgent = agents?.Where(w => w.agecod == BOCOA2).FirstOrDefault();
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
                    BOCOA1 = value?.agecod;
                    BOCOA1P = !BOCOA1P.HasValue || firstAgent != null ? value?.agepvg : BOCOA1P;
                    BOCOA1PT = string.IsNullOrWhiteSpace(BOCOA1PT) || firstAgent != null ? value?.agepvgt : BOCOA1PT;
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
                    BOCOA2 = value?.agecod;
                    BOCOA2P = !BOCOA2P.HasValue || secondAgent != null ? value?.agepvg : BOCOA2P;
                    BOCOA2PT = string.IsNullOrWhiteSpace(BOCOA2PT) || secondAgent != null ? value?.agepvgt : BOCOA2PT;
                    secondAgent = value;
                    NotifyPropertyChanged("SecondAgent");
                }
            }
        }
        #endregion
    }
}
