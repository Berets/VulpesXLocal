using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class CUSTOMER_GROUPS
    {
        public bool IsInsert { get; set; }

        #region Causal
        private ObservableCollection<CAUCONT>? causalsList;
        public ObservableCollection<CAUCONT>? CausalsList
        {
            get
            {
                return causalsList;
            }

            set
            {
                causalsList = value;
                if (!string.IsNullOrWhiteSpace(cccaus))
                    SelectedCausal = causalsList?.Where(w => w.caucod == cccaus).FirstOrDefault();
                else
                    SelectedCausal = null;
                NotifyPropertyChanged("CausalsList");
            }
        }
        public string? CausalDescription { get; set; }
        private CAUCONT? selectedCausal;
        public CAUCONT? SelectedCausal
        {
            get => selectedCausal;
            set
            {
                selectedCausal = value;
                CausalDescription = selectedCausal?.FullDescriptionSearchable;
                NotifyPropertyChanged("SelectedCausal");
                NotifyPropertyChanged("CausalDescription");
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
                if (!string.IsNullOrWhiteSpace(ccgrup))
                    SelectedGroup = groupsList?.Where(w => w.P1GRUP == ccgrup).FirstOrDefault();
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
                if (!string.IsNullOrWhiteSpace(ccgrup) && !string.IsNullOrWhiteSpace(cccont))
                    SelectedAccount = accountsList?.Where(w => w.P1GRUP == ccgrup && w.P2CONT == cccont).FirstOrDefault();
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
                if (!string.IsNullOrWhiteSpace(ccgrup) && !string.IsNullOrWhiteSpace(cccont) && !string.IsNullOrWhiteSpace(ccsott))
                    SelectedSubaccount = subaccountsList?.Where(w => w.P1GRUP == ccgrup && w.P2CONT == cccont && w.P3SOTC == ccsott).FirstOrDefault();
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
                SubaccountDescription = selectedSubaccount?.FullDescriptionSearchable;
                NotifyPropertyChanged("SelectedSubaccount");
                NotifyPropertyChanged("SubaccountDescription");
            }
        }
        public string? SubaccountDescription { get; set; }
        #endregion
    }
}
