using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ACC_ASSETS_TYPOLOGIES
    {
        #region Asset category
        private ObservableCollection<ACC_ASSETS_CATEGORIES>? assetCategories;
        public ObservableCollection<ACC_ASSETS_CATEGORIES>? AssetCategories
        {
            get { return assetCategories; }
            set
            {
                assetCategories = value;
                if (!string.IsNullOrWhiteSpace(jcateg))
                    SelectedAssetCategory = assetCategories?.Where(w => w.jcateg == jcateg).FirstOrDefault();
                else
                    SelectedAssetCategory = null;
                NotifyPropertyChanged("AssetCategories");
            }
        }

        private ACC_ASSETS_CATEGORIES? selectedAssetCategory;

        public ACC_ASSETS_CATEGORIES? SelectedAssetCategory
        {
            get => selectedAssetCategory;
            set
            {
                if (selectedAssetCategory?.jcateg != value?.jcateg)
                {
                    jcateg = value?.jcateg;
                    selectedAssetCategory = value;
                    NotifyPropertyChanged("SelectedAssetCategory");
                }
            }
        }
        #endregion

        #region PDC Main
        public ObservableCollection<PDCCONTI>? AllAccounts { get; set; }
        public ObservableCollection<PDCSOTTO>? AllSubccounts { get; set; }

        #region PDC
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
                        Subaccounts = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (w.p3soci == null || w.p3soci == tsoci)).ToList());
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

        #region PDC Ordinary
        private ObservableCollection<PDCGRUPPI>? ordinaryGroups;
        public ObservableCollection<PDCGRUPPI>? OrdinaryGroups
        {
            get { return ordinaryGroups; }
            set
            {
                ordinaryGroups = value;
                if (!string.IsNullOrWhiteSpace(grupp2))
                    OrdinaryGroup = ordinaryGroups?.Where(w => w.P1GRUP == grupp2).FirstOrDefault();
                else
                    OrdinaryGroup = null;
                NotifyPropertyChanged("OrdinaryGroups");
            }
        }
        private ObservableCollection<PDCCONTI>? ordinaryAccounts;
        public ObservableCollection<PDCCONTI>? OrdinaryAccounts
        {
            get => ordinaryAccounts;
            set
            {
                ordinaryAccounts = value;
                if (!string.IsNullOrWhiteSpace(grupp2) && !string.IsNullOrWhiteSpace(conto2))
                    OrdinaryAccount = ordinaryAccounts?.Where(w => w.P1GRUP == grupp2 && w.P2CONT == conto2).FirstOrDefault();
                else
                    OrdinaryAccount = null;
                NotifyPropertyChanged("OrdinaryAccounts");
            }
        }
        private ObservableCollection<PDCSOTTO>? ordinarySubaccounts;
        public ObservableCollection<PDCSOTTO>? OrdinarySubaccounts
        {
            get { return ordinarySubaccounts; }
            set
            {
                ordinarySubaccounts = value;
                if (!string.IsNullOrWhiteSpace(grupp2) && !string.IsNullOrWhiteSpace(conto2) && !string.IsNullOrWhiteSpace(sotto2))
                    OrdinarySubaccount = ordinarySubaccounts?.Where(w => w.P1GRUP == grupp2 && w.P2CONT == conto2 && w.P3SOTC == sotto2).FirstOrDefault();
                else
                    OrdinarySubaccount = null;
                NotifyPropertyChanged("OrdinarySubaccounts");
            }
        }

        private PDCGRUPPI? ordinaryGroup;
        public PDCGRUPPI? OrdinaryGroup
        {
            get => ordinaryGroup;
            set
            {
                if (ordinaryGroup?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        OrdinaryAccounts = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        OrdinaryAccounts = null;
                    }
                    if (ordinaryGroup != null)
                    {
                        OrdinaryAccount = null;
                        OrdinarySubaccount = null;
                        OrdinarySubaccounts = null;
                    }
                    grupp2 = value?.P1GRUP;
                    ordinaryGroup = value;
                    NotifyPropertyChanged("OrdinaryGroup");
                }
            }
        }

        private PDCCONTI? ordinaryAccount;
        public PDCCONTI? OrdinaryAccount
        {
            get => ordinaryAccount;
            set
            {
                if (ordinaryAccount?.P1GRUP != value?.P1GRUP && ordinaryAccount?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        OrdinarySubaccounts = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (w.p3soci == null || w.p3soci == tsoci)).ToList());
                    }
                    else
                    {
                        OrdinarySubaccounts = null;
                    }
                    if (ordinaryAccount != null)
                    {
                        OrdinarySubaccount = null;
                        OrdinarySubaccounts = null;
                    }
                    conto2 = value?.P2CONT;
                    ordinaryAccount = value;
                    NotifyPropertyChanged("OrdinaryAccount");
                }
            }
        }

        private PDCSOTTO? ordinarySubaccount;

        public PDCSOTTO? OrdinarySubaccount
        {
            get => ordinarySubaccount;
            set
            {
                if (ordinarySubaccount?.P1GRUP != value?.P1GRUP && ordinarySubaccount?.P2CONT != value?.P2CONT && ordinarySubaccount?.P3SOTC != value?.P3SOTC)
                {
                    sotto2 = value?.P3SOTC;
                    ordinarySubaccount = value;
                    NotifyPropertyChanged("OrdinarySubaccount");
                }
            }
        }
        #endregion

        #region PDC Anticipated
        private ObservableCollection<PDCGRUPPI>? anticipatedGroups;
        public ObservableCollection<PDCGRUPPI>? AnticipatedGroups
        {
            get { return anticipatedGroups; }
            set
            {
                anticipatedGroups = value;
                if (!string.IsNullOrWhiteSpace(grupp3))
                    AnticipatedGroup = anticipatedGroups?.Where(w => w.P1GRUP == grupp3).FirstOrDefault();
                else
                    AnticipatedGroup = null;
                NotifyPropertyChanged("AnticipatedGroups");
            }
        }
        private ObservableCollection<PDCCONTI>? anticipatedAccounts;
        public ObservableCollection<PDCCONTI>? AnticipatedAccounts
        {
            get => anticipatedAccounts;
            set
            {
                anticipatedAccounts = value;
                if (!string.IsNullOrWhiteSpace(grupp3) && !string.IsNullOrWhiteSpace(conto3))
                    AnticipatedAccount = anticipatedAccounts?.Where(w => w.P1GRUP == grupp3 && w.P2CONT == conto3).FirstOrDefault();
                else
                    AnticipatedAccount = null;
                NotifyPropertyChanged("AnticipatedAccounts");
            }
        }
        private ObservableCollection<PDCSOTTO>? anticipatedSubaccounts;
        public ObservableCollection<PDCSOTTO>? AnticipatedSubaccounts
        {
            get { return anticipatedSubaccounts; }
            set
            {
                anticipatedSubaccounts = value;
                if (!string.IsNullOrWhiteSpace(grupp3) && !string.IsNullOrWhiteSpace(conto3) && !string.IsNullOrWhiteSpace(sotto3))
                    AnticipatedSubaccount = anticipatedSubaccounts?.Where(w => w.P1GRUP == grupp3 && w.P2CONT == conto3 && w.P3SOTC == sotto3).FirstOrDefault();
                else
                    AnticipatedSubaccount = null;
                NotifyPropertyChanged("AnticipatedSubaccounts");
            }
        }

        private PDCGRUPPI? anticipatedGroup;
        public PDCGRUPPI? AnticipatedGroup
        {
            get => anticipatedGroup;
            set
            {
                if (anticipatedGroup?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        AnticipatedAccounts = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AnticipatedAccounts = null;
                    }
                    if (anticipatedGroup != null)
                    {
                        AnticipatedAccount = null;
                        AnticipatedSubaccount = null;
                        AnticipatedSubaccounts = null;
                    }
                    grupp3 = value?.P1GRUP;
                    anticipatedGroup = value;
                    NotifyPropertyChanged("AnticipatedGroup");
                }
            }
        }

        private PDCCONTI? anticipatedAccount;
        public PDCCONTI? AnticipatedAccount
        {
            get => anticipatedAccount;
            set
            {
                if (anticipatedAccount?.P1GRUP != value?.P1GRUP && anticipatedAccount?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        AnticipatedSubaccounts = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (w.p3soci == null || w.p3soci == tsoci)).ToList());
                    }
                    else
                    {
                        AnticipatedSubaccounts = null;
                    }
                    if (anticipatedAccount != null)
                    {
                        AnticipatedSubaccount = null;
                        AnticipatedSubaccounts = null;
                    }
                    conto3 = value?.P2CONT;
                    anticipatedAccount = value;
                    NotifyPropertyChanged("AnticipatedAccount");
                }
            }
        }

        private PDCSOTTO? anticipatedSubaccount;

        public PDCSOTTO? AnticipatedSubaccount
        {
            get => anticipatedSubaccount;
            set
            {
                if (anticipatedSubaccount?.P1GRUP != value?.P1GRUP && anticipatedSubaccount?.P2CONT != value?.P2CONT && anticipatedSubaccount?.P3SOTC != value?.P3SOTC)
                {
                    sotto3 = value?.P3SOTC;
                    anticipatedSubaccount = value;
                    NotifyPropertyChanged("AnticipatedSubaccount");
                }
            }
        }
        #endregion

        #region PDC Fund
        private ObservableCollection<PDCGRUPPI>? fundGroups;
        public ObservableCollection<PDCGRUPPI>? FundGroups
        {
            get { return fundGroups; }
            set
            {
                fundGroups = value;
                if (!string.IsNullOrWhiteSpace(grupp1))
                    FundGroup = fundGroups?.Where(w => w.P1GRUP == grupp1).FirstOrDefault();
                else
                    FundGroup = null;
                NotifyPropertyChanged("FundGroups");
            }
        }
        private ObservableCollection<PDCCONTI>? fundAccounts;
        public ObservableCollection<PDCCONTI>? FundAccounts
        {
            get => fundAccounts;
            set
            {
                fundAccounts = value;
                if (!string.IsNullOrWhiteSpace(grupp1) && !string.IsNullOrWhiteSpace(cont1))
                    FundAccount = fundAccounts?.Where(w => w.P1GRUP == grupp1 && w.P2CONT == cont1).FirstOrDefault();
                else
                    FundAccount = null;
                NotifyPropertyChanged("FundAccounts");
            }
        }
        private ObservableCollection<PDCSOTTO>? fundSubaccounts;
        public ObservableCollection<PDCSOTTO>? FundSubaccounts
        {
            get { return fundSubaccounts; }
            set
            {
                fundSubaccounts = value;
                if (!string.IsNullOrWhiteSpace(grupp1) && !string.IsNullOrWhiteSpace(cont1) && !string.IsNullOrWhiteSpace(sotto1))
                    FundSubaccount = fundSubaccounts?.Where(w => w.P1GRUP == grupp1 && w.P2CONT == cont1 && w.P3SOTC == sotto1).FirstOrDefault();
                else
                    FundSubaccount = null;
                NotifyPropertyChanged("FundSubaccounts");
            }
        }

        private PDCGRUPPI? fundGroup;
        public PDCGRUPPI? FundGroup
        {
            get => fundGroup;
            set
            {
                if (fundGroup?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        FundAccounts = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        FundAccounts = null;
                    }
                    if (fundGroup != null)
                    {
                        FundAccount = null;
                        FundSubaccount = null;
                        FundSubaccounts = null;
                    }
                    grupp1 = value?.P1GRUP;
                    fundGroup = value;
                    NotifyPropertyChanged("FundGroup");
                }
            }
        }

        private PDCCONTI? fundAccount;
        public PDCCONTI? FundAccount
        {
            get => fundAccount;
            set
            {
                if (fundAccount?.P1GRUP != value?.P1GRUP && fundAccount?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        FundSubaccounts = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (w.p3soci == null || w.p3soci == tsoci)).ToList());
                    }
                    else
                    {
                        FundSubaccounts = null;
                    }
                    if (fundAccount != null)
                    {
                        FundSubaccount = null;
                        FundSubaccounts = null;
                    }
                    cont1 = value?.P2CONT;
                    fundAccount = value;
                    NotifyPropertyChanged("FundAccount");
                }
            }
        }

        private PDCSOTTO? fundSubaccount;

        public PDCSOTTO? FundSubaccount
        {
            get => fundSubaccount;
            set
            {
                if (fundSubaccount?.P1GRUP != value?.P1GRUP && fundSubaccount?.P2CONT != value?.P2CONT && fundSubaccount?.P3SOTC != value?.P3SOTC)
                {
                    sotto1 = value?.P3SOTC;
                    fundSubaccount = value;
                    NotifyPropertyChanged("FundSubaccount");
                }
            }
        }
        #endregion
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
