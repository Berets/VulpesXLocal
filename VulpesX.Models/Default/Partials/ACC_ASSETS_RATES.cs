using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ACC_ASSETS_RATES
    {
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
                if (!string.IsNullOrWhiteSpace(tgrupp))
                    Group = groups?.Where(w => w.P1GRUP == tgrupp).FirstOrDefault();
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
                if (!string.IsNullOrWhiteSpace(tgrupp) && !string.IsNullOrWhiteSpace(gconto))
                    Account = accounts?.Where(w => w.P1GRUP == tgrupp && w.P2CONT == gconto).FirstOrDefault();
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
                if (!string.IsNullOrWhiteSpace(tgrupp) && !string.IsNullOrWhiteSpace(gconto) && !string.IsNullOrWhiteSpace(tsotco))
                    Subaccount = subaccounts?.Where(w => w.P1GRUP == tgrupp && w.P2CONT == gconto && w.P3SOTC == tsotco).FirstOrDefault();
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
                        Subaccounts = null;
                    }
                    tgrupp = (value != null) ? value.P1GRUP : string.Empty;
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
                if (account?.P1GRUP != value?.P1GRUP && account?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        Subaccounts = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (w.p3soci == null || w.p3soci == ammsoc)).ToList());
                    }
                    else
                    {
                        Subaccounts = null;
                    }
                    if (account != null)
                    {
                        Subaccount = null;
                        Subaccounts = null;
                    }
                    gconto = (value != null) ? value.P2CONT : string.Empty;
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
                if (subaccount?.P1GRUP != value?.P1GRUP && subaccount?.P2CONT != value?.P2CONT && subaccount?.P3SOTC != value?.P3SOTC)
                {
                    tsotco = (value != null) ? value.P3SOTC : string.Empty;
                    subaccount = value;
                    NotifyPropertyChanged("Subaccount");
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
