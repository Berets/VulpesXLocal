using System.Collections.ObjectModel;
using VulpesX.Models.Default.Partials;


namespace VulpesX.Models.Default
{
    public partial class PNRIGHE
    {
        public event EventHandler? SubaccountChanged;
        protected void OnSubaccountChanged(EventArgs e)
        {
            SubaccountChanged?.Invoke(this, e);
        }

        public string? CurrencyID { get; set; }
        public PNTESTATA? Testata { get; set; }
        public ABE? BasicRegistry { get; set; }

        public decimal? Dare => N1SEGN == "D" ? N1IMEU : 0;
        public decimal? Avere => N1SEGN == "A" ? N1IMEU : 0;
        public decimal SaldoMastrino { get; set; }
        public string? SegnoSaldoMastrino { get; set; }
        public string? PDCExternalCode { get; set; }
        public string? PDCAlternativeCode { get; set; }

        public PDCGRUPPI? Group { get; set; }
        public PDCCONTI? Account { get; set; }
        public PDCSOTTO? Subaccount { get; set; }

        private ObservableCollection<SectionalItem>? expireRows;
        public ObservableCollection<SectionalItem>? ExpireRows
        {
            get { return expireRows; }
            set
            {
                expireRows = value ?? new ObservableCollection<SectionalItem>();
                NotifyPropertyChanged("ExpireRows");
            }
        }

        public bool SelectECVisibility => selectedEntity != null && SelectedAccount != null && !string.IsNullOrWhiteSpace(SelectedAccount.p2flcf) && (SelectedAccount.p2flcf == "C" || SelectedAccount.p2flcf == "F") ? true : false;

        public string? TreasuryProgressiveSign { get; set; }
        public decimal TreasuryProgressive { get; set; }
        public bool NotPair { get; set; } = false;

        #region Entity
        private ObservableCollection<ABE>? entitiesList;
        public ObservableCollection<ABE>? EntitiesList
        {
            get { return entitiesList; }
            set
            {
                entitiesList = value;
                if (n1clie.HasValue)
                    SelectedEntity = entitiesList?.Where(w => w.abecod == n1clie.Value).FirstOrDefault();
                else
                    SelectedEntity = null;
                NotifyPropertyChanged("EntitiesList");
            }
        }

        public string? EntityDescription { get; set; }
        private ABE? selectedEntity;
        public ABE? SelectedEntity
        {
            get => selectedEntity;
            set
            {
                selectedEntity = value;
                EntityDescription = selectedEntity?.FullDescriptionSearchable;
                NotifyPropertyChanged("SelectedEntity");
                NotifyPropertyChanged("EntityDescription");
                NotifyPropertyChanged("SelectECVisibility");
            }
        }
        public bool IsEntityReadonly { get; set; }
        #endregion

        #region Payment

        public ObservableCollection<PAGCLI>? CustomerPaymentsList { get; set; }
        public ObservableCollection<PAGFOR>? SupplierPaymentsList { get; set; }
        private ObservableCollection<GenericIDDescription>? paymentsList;
        public ObservableCollection<GenericIDDescription>? PaymentsList
        {
            get
            {
                return paymentsList;
            }

            set
            {
                paymentsList = value;
                if (!string.IsNullOrWhiteSpace(n1paga))
                    SelectedPayment = paymentsList?.Where(w => w.ID == n1paga).FirstOrDefault();
                else
                    SelectedPayment = null;
                NotifyPropertyChanged("SelectedPayment");
                NotifyPropertyChanged("PaymentDescription");
            }
        }
        public string? PaymentDescription { get; set; }
        private GenericIDDescription? selectedPayment;
        public GenericIDDescription? SelectedPayment
        {
            get => selectedPayment;
            set
            {
                selectedPayment = value;
                PaymentDescription = selectedPayment?.FullDescription;
                NotifyPropertyChanged("PaymentDescription");
            }
        }
        public bool IsPaymentReadonly { get; set; }
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
                if (!string.IsNullOrWhiteSpace(pngrup))
                    SelectedGroup = groupsList?.Where(w => w.P1GRUP == pngrup).FirstOrDefault();
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
                if (!string.IsNullOrWhiteSpace(pngrup) && !string.IsNullOrWhiteSpace(pncont))
                    SelectedAccount = accountsList?.Where(w => w.P1GRUP == pngrup && w.P2CONT == pncont).FirstOrDefault();
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
                if (!string.IsNullOrWhiteSpace(pngrup) && !string.IsNullOrWhiteSpace(pncont) && !string.IsNullOrWhiteSpace(pnsott))
                    SelectedSubaccount = subaccountsList?.Where(w => w.P1GRUP == pngrup && w.P2CONT == pncont && w.P3SOTC == pnsott).FirstOrDefault();
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
                selectedGroup = value;
                SelectedAccount = null;
                SelectedSubaccount = null;
                if (selectedGroup != null && AccountCache != null)
                {
                    AccountsList = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == selectedGroup.P1GRUP).ToList());
                }
                else
                {
                    AccountsList = null;
                }
                GroupDescription = selectedGroup?.FullDescriptionSearchable;
                NotifyPropertyChanged("SelectedGroup");
                NotifyPropertyChanged("GroupDescription");
            }
        }
        public string? GroupDescription { get; set; }

        private PDCCONTI? selectedAccount;
        public PDCCONTI? SelectedAccount
        {
            get
            {
                return selectedAccount;
            }
            set
            {
                selectedAccount = value;
                SelectedSubaccount = null;
                if (selectedAccount != null && SubaccountCache != null)
                {
                    SubaccountsList = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == selectedAccount.P1GRUP && w.P2CONT == selectedAccount.P2CONT).ToList());
                }
                else
                {
                    SubaccountsList = null;
                }
                AccountDescription = selectedAccount?.FullDescriptionSearchable;
                NotifyPropertyChanged("SelectedAccount");
                NotifyPropertyChanged("AccountDescription");
            }
        }
        public string? AccountDescription { get; set; }

        private PDCSOTTO? selectedSubaccount;

        public PDCSOTTO? SelectedSubaccount
        {
            get
            {
                return selectedSubaccount;
            }
            set
            {
                selectedSubaccount = value;
                SelectedPayment = null;
                SelectedEntity = null;
                EntitiesList = null;
                IsPaymentReadonly = true;
                IsEntityReadonly = true;
                if (!string.IsNullOrWhiteSpace(selectedSubaccount?.P3CLFO))
                {
                    IsPaymentReadonly = false;
                    IsEntityReadonly = false;
                }
                SubaccountDescription = selectedSubaccount?.FullDescriptionSearchable;
                NotifyPropertyChanged("IsPaymentEnabled");
                NotifyPropertyChanged("IsEntityEnabled");
                NotifyPropertyChanged("SelectedSubaccount");
                NotifyPropertyChanged("SubaccountDescription");
                OnSubaccountChanged(EventArgs.Empty);
            }
        }
        public string? SubaccountDescription { get; set; }

        public string? DIVADescription { get; set; }
        #endregion

        #region Cost center
        private ObservableCollection<TCECO00F>? costCentersList;
        public ObservableCollection<TCECO00F>? CostCentersList
        {
            get => costCentersList;
            set
            {
                costCentersList = value;
                if (!string.IsNullOrWhiteSpace(N1CCCC))
                    SelectedCostCenter = costCentersList?.Where(w => w.cecodc == N1CCCC).FirstOrDefault();
                else
                    SelectedCostCenter = null;
                NotifyPropertyChanged("SelectedCostCenter");
                NotifyPropertyChanged("CostCenterDescription");
            }
        }

        public string? CostCenterDescription { get; set; }
        private TCECO00F? selectedCostCenter;
        public TCECO00F? SelectedCostCenter
        {
            get => selectedCostCenter;
            set
            {
                selectedCostCenter = value;
                CostCenterDescription = selectedCostCenter?.FullDescriptionSearchable;
                NotifyPropertyChanged("CostCenterDescription");
            }
        }
        public bool IsCostCenterReadonly { get; set; }
        #endregion
    }
}
