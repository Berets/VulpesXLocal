using System.Collections.ObjectModel;
using System.Security.Principal;

namespace VulpesX.Models.Default
{
    public partial class BANAZIEN
    {
        public ABICAB? Bank { get; set; }
        public string? BankName { get; set; }
        public string? BankAgency { get; set; }

        public string FullDescriptionSearchable => $"{abiabi} {abicab} {BankName?.Trim()} {BankAgency?.Trim()} {abicon?.Trim()}";

        public bool scrittdettBool
        {
            get
            {
                return scrittdett == "S";
            }
            set
            {
                if (value)
                    scrittdett = "S";
                else
                    scrittdett = "N";
            }
        }

        #region PDC bank
        public ObservableCollection<PDCCONTI>? AllAccounts { get; set; }
        public ObservableCollection<PDCSOTTO>? AllSubccounts { get; set; }

        private ObservableCollection<PDCGRUPPI>? groupsList;
        public ObservableCollection<PDCGRUPPI>? GroupsList
        {
            get { return groupsList; }
            set
            {
                groupsList = value;
                if (!string.IsNullOrWhiteSpace(abigba))
                    SelectedGroupBank = groupsList?.Where(w => w.P1GRUP == abigba).FirstOrDefault();
                else
                    SelectedGroupBank = null;
                if (!string.IsNullOrWhiteSpace(abigru))
                    SelectedGroupAnticipation = groupsList?.Where(w => w.P1GRUP == abigru).FirstOrDefault();
                else
                    SelectedGroupAnticipation = null;
                if (!string.IsNullOrWhiteSpace(abigrb))
                    SelectedGroupCosts = groupsList?.Where(w => w.P1GRUP == abigrb).FirstOrDefault();
                else
                    SelectedGroupCosts = null;
                if (!string.IsNullOrWhiteSpace(abigru1))
                    SelectedGroupWallet = groupsList?.Where(w => w.P1GRUP == abigru1).FirstOrDefault();
                else
                    SelectedGroupWallet = null;
                if (!string.IsNullOrWhiteSpace(abigru2))
                    SelectedGroupEffects = groupsList?.Where(w => w.P1GRUP == abigru2).FirstOrDefault();
                else
                    SelectedGroupEffects = null;
                if (!string.IsNullOrWhiteSpace(abigru3))
                    SelectedGroupBankAnticipation = groupsList?.Where(w => w.P1GRUP == abigru3).FirstOrDefault();
                else
                    SelectedGroupBankAnticipation = null;
                if (!string.IsNullOrWhiteSpace(abigru4))
                    SelectedGroupItalyAnticipation = groupsList?.Where(w => w.P1GRUP == abigru4).FirstOrDefault();
                else
                    SelectedGroupItalyAnticipation = null;
                if (!string.IsNullOrWhiteSpace(abigru5))
                    SelectedGroupForeignAnticipation = groupsList?.Where(w => w.P1GRUP == abigru5).FirstOrDefault();
                else
                    SelectedGroupForeignAnticipation = null;
                NotifyPropertyChanged("GroupsList");
            }
        }

        #region Bank
        private ObservableCollection<PDCCONTI>? accountsBankList;
        public ObservableCollection<PDCCONTI>? AccountsBankList
        {
            get { return accountsBankList; }
            set
            {
                accountsBankList = value;
                if (!string.IsNullOrWhiteSpace(abigba) && !string.IsNullOrWhiteSpace(abicba))
                    SelectedAccountBank = accountsBankList?.Where(w => w.P1GRUP == abigba && w.P2CONT == abicba).FirstOrDefault();
                else
                    SelectedAccountBank = null;
                NotifyPropertyChanged("AccountsBankList");
            }
        }

        private ObservableCollection<PDCSOTTO>? subaccountsBankList;
        public ObservableCollection<PDCSOTTO>? SubaccountsBankList
        {
            get { return subaccountsBankList; }
            set
            {
                subaccountsBankList = value;
                if (!string.IsNullOrWhiteSpace(abigba) && !string.IsNullOrWhiteSpace(abicba) && !string.IsNullOrWhiteSpace(abisba))
                    SelectedSubaccountBank = subaccountsBankList?.Where(w => w.P1GRUP == abigba && w.P2CONT == abicba && w.P3SOTC == abisba).FirstOrDefault();
                else
                    SelectedSubaccountBank = null;
                NotifyPropertyChanged("SubaccountsBankList");
            }
        }

        private PDCGRUPPI? selectedGroupBank;
        public PDCGRUPPI? SelectedGroupBank
        {
            get
            {
                return selectedGroupBank;
            }
            set
            {
                if (selectedGroupBank?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        AccountsBankList = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsBankList = null;
                    }
                    if (selectedGroupBank != null)
                    {
                        SelectedAccountBank = null;
                        SelectedSubaccountBank = null;
                        SubaccountsBankList = null;
                    }
                    abigba = value?.P1GRUP;
                    selectedGroupBank = value;
                    NotifyPropertyChanged("SelectedGroupBank");
                }
            }
        }

        private PDCCONTI? selectedAccountBank;
        public PDCCONTI? SelectedAccountBank
        {
            get
            {
                return selectedAccountBank;
            }
            set
            {
                if (selectedAccountBank?.P1GRUP != value?.P1GRUP && selectedAccountBank?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        SubaccountsBankList = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (string.IsNullOrEmpty(w.p3soci?.Trim()) || w.p3soci == abisoc)).ToList());
                    }
                    else
                    {
                        SubaccountsBankList = null;
                    }
                    if (selectedAccountBank != null)
                    {
                        SelectedSubaccountBank = null;
                        SubaccountsBankList = null;
                    }
                    abicba = value?.P2CONT;
                    selectedAccountBank = value;
                    NotifyPropertyChanged("SelectedAccountBank");
                }
            }
        }

        private PDCSOTTO? selectedSubaccountBank;
        public PDCSOTTO? SelectedSubaccountBank
        {
            get
            {
                return selectedSubaccountBank;
            }
            set
            {
                if (selectedSubaccountBank?.P1GRUP != value?.P1GRUP && selectedSubaccountBank?.P2CONT != value?.P2CONT && selectedSubaccountBank?.P3SOTC != value?.P3SOTC)
                {
                    abisba = value?.P3SOTC;
                    selectedSubaccountBank = value;
                    NotifyPropertyChanged("SelectedSubaccountBank");
                }
            }
        }
        #endregion
        #region Anticipation
        private ObservableCollection<PDCCONTI>? accountsAnticipationList;
        public ObservableCollection<PDCCONTI>? AccountsAnticipationList
        {
            get { return accountsAnticipationList; }
            set
            {
                accountsAnticipationList = value;
                if (!string.IsNullOrWhiteSpace(abigru) && !string.IsNullOrWhiteSpace(abicot))
                    SelectedAccountAnticipation = accountsAnticipationList?.Where(w => w.P1GRUP == abigru && w.P2CONT == abicot).FirstOrDefault();
                else
                    SelectedAccountAnticipation = null;
                NotifyPropertyChanged("AccountsAnticipationList");
            }
        }

        private ObservableCollection<PDCSOTTO>? subaccountsAnticipationList;
        public ObservableCollection<PDCSOTTO>? SubaccountsAnticipationList
        {
            get { return subaccountsAnticipationList; }
            set
            {
                subaccountsAnticipationList = value;
                if (!string.IsNullOrWhiteSpace(abigru) && !string.IsNullOrWhiteSpace(abicot) && !string.IsNullOrWhiteSpace(abisot))
                    SelectedSubaccountAnticipation = subaccountsAnticipationList?.Where(w => w.P1GRUP == abigru && w.P2CONT == abicot && w.P3SOTC == abisot).FirstOrDefault();
                else
                    SelectedSubaccountAnticipation = null;
                NotifyPropertyChanged("SubaccountsAnticipationList");
            }
        }

        private PDCGRUPPI? selectedGroupAnticipation;
        public PDCGRUPPI? SelectedGroupAnticipation
        {
            get
            {
                return selectedGroupAnticipation;
            }
            set
            {
                if (selectedGroupAnticipation?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        AccountsAnticipationList = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsAnticipationList = null;
                    }
                    if (selectedGroupAnticipation != null)
                    {
                        SelectedAccountAnticipation = null;
                        SelectedSubaccountAnticipation = null;
                        SubaccountsAnticipationList = null;
                    }
                    abigru = value?.P1GRUP;
                    selectedGroupAnticipation = value;
                    NotifyPropertyChanged("SelectedGroupAnticipation");
                }
            }
        }

        private PDCCONTI? selectedAccountAnticipation;
        public PDCCONTI? SelectedAccountAnticipation
        {
            get
            {
                return selectedAccountAnticipation;
            }
            set
            {
                if (selectedAccountAnticipation?.P1GRUP != value?.P1GRUP && selectedAccountAnticipation?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        SubaccountsAnticipationList = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (string.IsNullOrEmpty(w.p3soci?.Trim()) || w.p3soci == abisoc)).ToList());
                    }
                    else
                    {
                        SubaccountsAnticipationList = null;
                    }
                    if (selectedAccountAnticipation != null)
                    {
                        SelectedSubaccountAnticipation = null;
                        SubaccountsAnticipationList = null;
                    }
                    abicot = value?.P2CONT;
                    selectedAccountAnticipation = value;
                    NotifyPropertyChanged("SelectedAccountAnticipation");
                }
            }
        }

        private PDCSOTTO? selectedSubaccountAnticipation;
        public PDCSOTTO? SelectedSubaccountAnticipation
        {
            get
            {
                return selectedSubaccountAnticipation;
            }
            set
            {
                if (selectedSubaccountAnticipation?.P1GRUP != value?.P1GRUP && selectedSubaccountAnticipation?.P2CONT != value?.P2CONT && selectedSubaccountAnticipation?.P3SOTC != value?.P3SOTC)
                {
                    abisot = value?.P3SOTC;
                    selectedSubaccountAnticipation = value;
                    NotifyPropertyChanged("SelectedSubaccountAnticipation");
                }
            }
        }
        #endregion
        #region Costs
        private ObservableCollection<PDCCONTI>? accountsCostsList;
        public ObservableCollection<PDCCONTI>? AccountsCostsList
        {
            get { return accountsCostsList; }
            set
            {
                accountsCostsList = value;
                if (!string.IsNullOrWhiteSpace(abigrb) && !string.IsNullOrWhiteSpace(abicob))
                    SelectedAccountCosts = accountsCostsList?.Where(w => w.P1GRUP == abigrb && w.P2CONT == abicob).FirstOrDefault();
                else
                    SelectedAccountCosts = null;
                NotifyPropertyChanged("AccountsCostsList");
            }
        }

        private ObservableCollection<PDCSOTTO>? subaccountsCostsList;
        public ObservableCollection<PDCSOTTO>? SubaccountsCostsList
        {
            get { return subaccountsCostsList; }
            set
            {
                subaccountsCostsList = value;
                if (!string.IsNullOrWhiteSpace(abigrb) && !string.IsNullOrWhiteSpace(abicob) && !string.IsNullOrWhiteSpace(abisob))
                    SelectedSubaccountCosts = subaccountsCostsList?.Where(w => w.P1GRUP == abigrb && w.P2CONT == abicob && w.P3SOTC == abisob).FirstOrDefault();
                else
                    SelectedSubaccountCosts = null;
                NotifyPropertyChanged("SubaccountsCostsList");
            }
        }

        private PDCGRUPPI? selectedGroupCosts;
        public PDCGRUPPI? SelectedGroupCosts
        {
            get
            {
                return selectedGroupCosts;
            }
            set
            {
                if (selectedGroupCosts?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        AccountsCostsList = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsCostsList = null;
                    }
                    if (selectedGroupCosts != null)
                    {
                        SelectedAccountCosts = null;
                        SelectedSubaccountCosts = null;
                        SubaccountsCostsList = null;
                    }
                    abigrb = value?.P1GRUP;
                    selectedGroupCosts = value;
                    NotifyPropertyChanged("SelectedGroupCosts");
                }
            }
        }

        private PDCCONTI? selectedAccountCosts;
        public PDCCONTI? SelectedAccountCosts
        {
            get
            {
                return selectedAccountCosts;
            }
            set
            {
                if (selectedAccountCosts?.P1GRUP != value?.P1GRUP && selectedAccountCosts?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        SubaccountsCostsList = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (string.IsNullOrEmpty(w.p3soci?.Trim()) || w.p3soci == abisoc)).ToList());
                    }
                    else
                    {
                        SubaccountsCostsList = null;
                    }
                    if (selectedAccountCosts != null)
                    {
                        SelectedSubaccountCosts = null;
                        SubaccountsCostsList = null;
                    }
                    abicob = value?.P2CONT;
                    selectedAccountCosts = value;
                    NotifyPropertyChanged("SelectedAccountCosts");
                }
            }
        }

        private PDCSOTTO? selectedSubaccountCosts;
        public PDCSOTTO? SelectedSubaccountCosts
        {
            get
            {
                return selectedSubaccountCosts;
            }
            set
            {
                if (selectedSubaccountCosts?.P1GRUP != value?.P1GRUP && selectedSubaccountCosts?.P2CONT != value?.P2CONT && selectedSubaccountCosts?.P3SOTC != value?.P3SOTC)
                {
                    abisob = value?.P3SOTC;
                    selectedSubaccountCosts = value;
                    NotifyPropertyChanged("SelectedSubaccountCosts");
                }
            }
        }
        #endregion
        #region Wallet
        private ObservableCollection<PDCCONTI>? accountsWalletList;
        public ObservableCollection<PDCCONTI>? AccountsWalletList
        {
            get { return accountsWalletList; }
            set
            {
                accountsWalletList = value;
                if (!string.IsNullOrWhiteSpace(abigru1) && !string.IsNullOrWhiteSpace(abicon1))
                    SelectedAccountWallet = accountsWalletList?.Where(w => w.P1GRUP == abigru1 && w.P2CONT == abicon1).FirstOrDefault();
                else
                    SelectedAccountWallet = null;
                NotifyPropertyChanged("AccountsWalletList");
            }
        }

        private ObservableCollection<PDCSOTTO>? subaccountsWalletList;
        public ObservableCollection<PDCSOTTO>? SubaccountsWalletList
        {
            get { return subaccountsWalletList; }
            set
            {
                subaccountsWalletList = value;
                if (!string.IsNullOrWhiteSpace(abigru1) && !string.IsNullOrWhiteSpace(abicon1) && !string.IsNullOrWhiteSpace(abisot1))
                    SelectedSubaccountWallet = subaccountsWalletList?.Where(w => w.P1GRUP == abigru1 && w.P2CONT == abicon1 && w.P3SOTC == abisot1).FirstOrDefault();
                else
                    SelectedSubaccountWallet = null;
                NotifyPropertyChanged("SubaccountsWalletList");
            }
        }

        private PDCGRUPPI? selectedGroupWallet;
        public PDCGRUPPI? SelectedGroupWallet
        {
            get
            {
                return selectedGroupWallet;
            }
            set
            {
                if (selectedGroupWallet?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        AccountsWalletList = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsWalletList = null;
                    }
                    if (selectedGroupWallet != null)
                    {
                        SelectedAccountWallet = null;
                        SelectedSubaccountWallet = null;
                        SubaccountsWalletList = null;
                    }
                    abigru1 = value?.P1GRUP;
                    selectedGroupWallet = value;
                    NotifyPropertyChanged("SelectedGroupWallet");
                }
            }
        }


        private PDCCONTI? selectedAccountWallet;
        public PDCCONTI? SelectedAccountWallet
        {
            get
            {
                return selectedAccountWallet;
            }
            set
            {
                if (selectedAccountWallet?.P1GRUP != value?.P1GRUP && selectedAccountWallet?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        SubaccountsWalletList = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (string.IsNullOrEmpty(w.p3soci?.Trim()) || w.p3soci == abisoc)).ToList());
                    }
                    else
                    {
                        SubaccountsWalletList = null;
                    }
                    if (selectedAccountWallet != null)
                    {
                        SelectedSubaccountWallet = null;
                        SubaccountsWalletList = null;
                    }
                    abicon1 = value?.P2CONT;
                    selectedAccountWallet = value;
                    NotifyPropertyChanged("SelectedAccountWallet");
                }
            }
        }

        private PDCSOTTO? selectedSubaccountWallet;
        public PDCSOTTO? SelectedSubaccountWallet
        {
            get
            {
                return selectedSubaccountWallet;
            }
            set
            {
                if (selectedSubaccountWallet?.P1GRUP != value?.P1GRUP && selectedSubaccountWallet?.P2CONT != value?.P2CONT && selectedSubaccountWallet?.P3SOTC != value?.P3SOTC)
                {
                    abisot1 = value?.P3SOTC;
                    selectedSubaccountWallet = value;
                    NotifyPropertyChanged("SelectedSubaccountWallet");
                }
            }
        }
        #endregion
        #region Effects
        private ObservableCollection<PDCCONTI>? accountsEffectsList;
        public ObservableCollection<PDCCONTI>? AccountsEffectsList
        {
            get { return accountsEffectsList; }
            set
            {
                accountsEffectsList = value;
                if (!string.IsNullOrWhiteSpace(abigru2) && !string.IsNullOrWhiteSpace(abicon2))
                    SelectedAccountEffects = accountsEffectsList?.Where(w => w.P1GRUP == abigru2 && w.P2CONT == abicon2).FirstOrDefault();
                else
                    SelectedAccountEffects = null;
                NotifyPropertyChanged("AccountsEffectsList");
            }
        }

        private ObservableCollection<PDCSOTTO>? subaccountsEffectsList;
        public ObservableCollection<PDCSOTTO>? SubaccountsEffectsList
        {
            get { return subaccountsEffectsList; }
            set
            {
                subaccountsEffectsList = value;
                if (!string.IsNullOrWhiteSpace(abigru2) && !string.IsNullOrWhiteSpace(abicon2) && !string.IsNullOrWhiteSpace(abisot2))
                    SelectedSubaccountEffects = subaccountsEffectsList?.Where(w => w.P1GRUP == abigru2 && w.P2CONT == abicon2 && w.P3SOTC == abisot2).FirstOrDefault();
                else
                    SelectedSubaccountEffects = null;
                NotifyPropertyChanged("SubaccountsEffectsList");
            }
        }

        private PDCGRUPPI? selectedGroupEffects;
        public PDCGRUPPI? SelectedGroupEffects
        {
            get
            {
                return selectedGroupEffects;
            }
            set
            {
                if (selectedGroupEffects?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        AccountsEffectsList = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsEffectsList = null;
                    }
                    if (selectedGroupEffects != null)
                    {
                        SelectedAccountEffects = null;
                        SelectedSubaccountEffects = null;
                        SubaccountsEffectsList = null;
                    }
                    abigru2 = value?.P1GRUP;
                    selectedGroupEffects = value;
                    NotifyPropertyChanged("SelectedGroupEffects");
                }
            }
        }

        private PDCCONTI? selectedAccountEffects;
        public PDCCONTI? SelectedAccountEffects
        {
            get
            {
                return selectedAccountEffects;
            }
            set
            {
                if (selectedAccountEffects?.P1GRUP != value?.P1GRUP && selectedAccountEffects?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        SubaccountsEffectsList = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (string.IsNullOrEmpty(w.p3soci?.Trim()) || w.p3soci == abisoc)).ToList());
                    }
                    else
                    {
                        SubaccountsEffectsList = null;
                    }
                    if (selectedAccountEffects != null)
                    {
                        SelectedSubaccountEffects = null;
                        SubaccountsEffectsList = null;
                    }
                    abicon2 = value?.P2CONT;
                    selectedAccountEffects = value;
                    NotifyPropertyChanged("SelectedAccountEffects");
                }
            }
        }

        private PDCSOTTO? selectedSubaccountEffects;
        public PDCSOTTO? SelectedSubaccountEffects
        {
            get
            {
                return selectedSubaccountEffects;
            }
            set
            {
                if (selectedSubaccountEffects?.P1GRUP != value?.P1GRUP && selectedSubaccountEffects?.P2CONT != value?.P2CONT && selectedSubaccountEffects?.P3SOTC != value?.P3SOTC)
                {
                    abisot2 = value?.P3SOTC;
                    selectedSubaccountEffects = value;
                    NotifyPropertyChanged("SelectedSubaccountEffects");
                }
            }
        }
        #endregion
        #region Bank anticipation
        private ObservableCollection<PDCCONTI>? accountsBankAnticipationList;
        public ObservableCollection<PDCCONTI>? AccountsBankAnticipationList
        {
            get { return accountsBankAnticipationList; }
            set
            {
                accountsBankAnticipationList = value;
                if (!string.IsNullOrWhiteSpace(abigru3) && !string.IsNullOrWhiteSpace(abicon3))
                    SelectedAccountBankAnticipation = accountsBankAnticipationList?.Where(w => w.P1GRUP == abigru3 && w.P2CONT == abicon3).FirstOrDefault();
                else
                    SelectedAccountBankAnticipation = null;
                NotifyPropertyChanged("AccountsBankAnticipationList");
            }
        }

        private ObservableCollection<PDCSOTTO>? subaccountsBankAnticipationList;
        public ObservableCollection<PDCSOTTO>? SubaccountsBankAnticipationList
        {
            get { return subaccountsBankAnticipationList; }
            set
            {
                subaccountsBankAnticipationList = value;
                if (!string.IsNullOrWhiteSpace(abigru3) && !string.IsNullOrWhiteSpace(abicon3) && !string.IsNullOrWhiteSpace(abisot3))
                    SelectedSubaccountBankAnticipation = subaccountsBankAnticipationList?.Where(w => w.P1GRUP == abigru3 && w.P2CONT == abicon3 && w.P3SOTC == abisot3).FirstOrDefault();
                else
                    SelectedSubaccountBankAnticipation = null;
                NotifyPropertyChanged("SubaccountsBankAnticipationList");
            }
        }

        private PDCGRUPPI? selectedGroupBankAnticipation;
        public PDCGRUPPI? SelectedGroupBankAnticipation
        {
            get
            {
                return selectedGroupBankAnticipation;
            }
            set
            {
                if (selectedGroupBankAnticipation?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        AccountsBankAnticipationList = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsBankAnticipationList = null;
                    }
                    if (selectedGroupBankAnticipation != null)
                    {
                        SelectedAccountBankAnticipation = null;
                        SelectedSubaccountBankAnticipation = null;
                        SubaccountsBankAnticipationList = null;
                    }
                    abigru3 = value?.P1GRUP;
                    selectedGroupBankAnticipation = value;
                    NotifyPropertyChanged("SelectedGroupBankAnticipation");
                }
            }
        }

        private PDCCONTI? selectedAccountBankAnticipation;
        public PDCCONTI? SelectedAccountBankAnticipation
        {
            get
            {
                return selectedAccountBankAnticipation;
            }
            set
            {
                if (selectedAccountBankAnticipation?.P1GRUP != value?.P1GRUP && selectedAccountBankAnticipation?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        SubaccountsBankAnticipationList = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (string.IsNullOrEmpty(w.p3soci?.Trim()) || w.p3soci == abisoc)).ToList());
                    }
                    else
                    {
                        SubaccountsBankAnticipationList = null;
                    }
                    if (selectedAccountBankAnticipation != null)
                    {
                        SelectedSubaccountBankAnticipation = null;
                        SubaccountsBankAnticipationList = null;
                    }
                    abicon3 = value?.P2CONT;
                    selectedAccountBankAnticipation = value;
                    NotifyPropertyChanged("SelectedAccountBankAnticipation");
                }
            }
        }

        private PDCSOTTO? selectedSubaccountBankAnticipation;
        public PDCSOTTO? SelectedSubaccountBankAnticipation
        {
            get
            {
                return selectedSubaccountBankAnticipation;
            }
            set
            {
                if (selectedSubaccountBankAnticipation?.P1GRUP != value?.P1GRUP && selectedSubaccountBankAnticipation?.P2CONT != value?.P2CONT && selectedSubaccountBankAnticipation?.P3SOTC != value?.P3SOTC)
                {
                    abisot3 = value?.P3SOTC;
                    selectedSubaccountBankAnticipation = value;
                    NotifyPropertyChanged("SelectedSubaccountBankAnticipation");
                }
            }
        }
        #endregion
        #region Italy anticipation
        private ObservableCollection<PDCCONTI>? accountsItalyAnticipationList;
        public ObservableCollection<PDCCONTI>? AccountsItalyAnticipationList
        {
            get { return accountsItalyAnticipationList; }
            set
            {
                accountsItalyAnticipationList = value;
                if (!string.IsNullOrWhiteSpace(abigru4) && !string.IsNullOrWhiteSpace(abicon4))
                    SelectedAccountItalyAnticipation = accountsItalyAnticipationList?.Where(w => w.P1GRUP == abigru4 && w.P2CONT == abicon4).FirstOrDefault();
                else
                    SelectedAccountItalyAnticipation = null;
                NotifyPropertyChanged("AccountsItalyAnticipationList");
            }
        }

        private ObservableCollection<PDCSOTTO>? subaccountsItalyAnticipationList;
        public ObservableCollection<PDCSOTTO>? SubaccountsItalyAnticipationList
        {
            get { return subaccountsItalyAnticipationList; }
            set
            {
                subaccountsItalyAnticipationList = value;
                if (!string.IsNullOrWhiteSpace(abigru4) && !string.IsNullOrWhiteSpace(abicon4) && !string.IsNullOrWhiteSpace(abisot4))
                    SelectedSubaccountItalyAnticipation = subaccountsItalyAnticipationList?.Where(w => w.P1GRUP == abigru4 && w.P2CONT == abicon4 && w.P3SOTC == abisot4).FirstOrDefault();
                else
                    SelectedSubaccountItalyAnticipation = null;
                NotifyPropertyChanged("SubaccountsItalyAnticipationList");
            }
        }

        private PDCGRUPPI? selectedGroupItalyAnticipation;
        public PDCGRUPPI? SelectedGroupItalyAnticipation
        {
            get
            {
                return selectedGroupItalyAnticipation;
            }
            set
            {
                if (selectedGroupItalyAnticipation?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        AccountsItalyAnticipationList = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsItalyAnticipationList = null;
                    }
                    if (selectedGroupItalyAnticipation != null)
                    {
                        SelectedAccountItalyAnticipation = null;
                        SelectedSubaccountItalyAnticipation = null;
                        SubaccountsItalyAnticipationList = null;
                    }
                    abigru4 = value?.P1GRUP;
                    selectedGroupItalyAnticipation = value;
                    NotifyPropertyChanged("SelectedGroupItalyAnticipation");
                }
            }
        }

        private PDCCONTI? selectedAccountItalyAnticipation;
        public PDCCONTI? SelectedAccountItalyAnticipation
        {
            get
            {
                return selectedAccountItalyAnticipation;
            }
            set
            {
                if (selectedAccountItalyAnticipation?.P1GRUP != value?.P1GRUP && selectedAccountItalyAnticipation?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        SubaccountsItalyAnticipationList = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (string.IsNullOrEmpty(w.p3soci?.Trim()) || w.p3soci == abisoc)).ToList());
                    }
                    else
                    {
                        SubaccountsItalyAnticipationList = null;
                    }
                    if (selectedAccountItalyAnticipation != null)
                    {
                        SelectedSubaccountItalyAnticipation = null;
                        SubaccountsItalyAnticipationList = null;
                    }
                    abicon4 = value?.P2CONT;
                    selectedAccountItalyAnticipation = value;
                    NotifyPropertyChanged("SelectedAccountItalyAnticipation");
                }
            }
        }

        private PDCSOTTO? selectedSubaccountItalyAnticipation;
        public PDCSOTTO? SelectedSubaccountItalyAnticipation
        {
            get
            {
                return selectedSubaccountItalyAnticipation;
            }
            set
            {
                if (selectedSubaccountItalyAnticipation?.P1GRUP != value?.P1GRUP && selectedSubaccountItalyAnticipation?.P2CONT != value?.P2CONT && selectedSubaccountItalyAnticipation?.P3SOTC != value?.P3SOTC)
                {
                    abisot4 = value?.P3SOTC;
                    selectedSubaccountItalyAnticipation = value;
                    NotifyPropertyChanged("SelectedSubaccountItalyAnticipation");
                }
            }
        }
        #endregion
        #region Foreign anticipation
        private ObservableCollection<PDCCONTI>? accountsForeignAnticipationList;
        public ObservableCollection<PDCCONTI>? AccountsForeignAnticipationList
        {
            get { return accountsForeignAnticipationList; }
            set
            {
                accountsForeignAnticipationList = value;
                if (!string.IsNullOrWhiteSpace(abigru5) && !string.IsNullOrWhiteSpace(abicon5))
                    SelectedAccountForeignAnticipation = accountsForeignAnticipationList?.Where(w => w.P1GRUP == abigru5 && w.P2CONT == abicon5).FirstOrDefault();
                else
                    SelectedAccountForeignAnticipation = null;
                NotifyPropertyChanged("AccountsForeignAnticipationList");
            }
        }

        private ObservableCollection<PDCSOTTO>? subaccountsForeignAnticipationList;
        public ObservableCollection<PDCSOTTO>? SubaccountsForeignAnticipationList
        {
            get { return subaccountsForeignAnticipationList; }
            set
            {
                subaccountsForeignAnticipationList = value;
                if (!string.IsNullOrWhiteSpace(abigru5) && !string.IsNullOrWhiteSpace(abicon5) && !string.IsNullOrWhiteSpace(abisot5))
                    SelectedSubaccountForeignAnticipation = subaccountsForeignAnticipationList?.Where(w => w.P1GRUP == abigru5 && w.P2CONT == abicon5 && w.P3SOTC == abisot5).FirstOrDefault();
                else
                    SelectedSubaccountForeignAnticipation = null;
                NotifyPropertyChanged("SubaccountsForeignAnticipationList");
            }
        }

        private PDCGRUPPI? selectedGroupForeignAnticipation;
        public PDCGRUPPI? SelectedGroupForeignAnticipation
        {
            get
            {
                return selectedGroupForeignAnticipation;
            }
            set
            {
                if (selectedGroupForeignAnticipation?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AllAccounts != null)
                    {
                        AccountsForeignAnticipationList = new ObservableCollection<PDCCONTI>(AllAccounts.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsForeignAnticipationList = null;
                    }
                    if (selectedGroupForeignAnticipation != null)
                    {
                        SelectedAccountForeignAnticipation = null;
                        SelectedSubaccountForeignAnticipation = null;
                        SubaccountsForeignAnticipationList = null;
                    }
                    abigru5 = value?.P1GRUP;
                    selectedGroupForeignAnticipation = value;
                    NotifyPropertyChanged("SelectedGroupForeignAnticipation");
                }
            }
        }

        private PDCCONTI? selectedAccountForeignAnticipation;
        public PDCCONTI? SelectedAccountForeignAnticipation
        {
            get
            {
                return selectedAccountForeignAnticipation;
            }
            set
            {
                if (selectedAccountForeignAnticipation?.P1GRUP != value?.P1GRUP && selectedAccountForeignAnticipation?.P2CONT != value?.P2CONT)
                {

                    if (value != null && AllSubccounts != null)
                    {
                        SubaccountsForeignAnticipationList = new ObservableCollection<PDCSOTTO>(AllSubccounts.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT && (string.IsNullOrEmpty(w.p3soci?.Trim()) || w.p3soci == abisoc)).ToList());
                    }
                    else
                    {
                        SubaccountsForeignAnticipationList = null;
                    }
                    if (selectedAccountForeignAnticipation != null)
                    {
                        SelectedSubaccountForeignAnticipation = null;
                        SubaccountsForeignAnticipationList = null;
                    }
                    abicon5 = value?.P2CONT;
                    selectedAccountForeignAnticipation = value;
                    NotifyPropertyChanged("SelectedAccountForeignAnticipation");
                }
            }
        }

        private PDCSOTTO? selectedSubaccountForeignAnticipation;
        public PDCSOTTO? SelectedSubaccountForeignAnticipation
        {
            get
            {
                return selectedSubaccountForeignAnticipation;
            }
            set
            {
                if (selectedSubaccountForeignAnticipation?.P1GRUP != value?.P1GRUP && selectedSubaccountForeignAnticipation?.P2CONT != value?.P2CONT && selectedSubaccountForeignAnticipation?.P3SOTC != value?.P3SOTC)
                {
                    abisot5 = value?.P3SOTC;
                    selectedSubaccountForeignAnticipation = value;
                    NotifyPropertyChanged("SelectedSubaccountForeignAnticipation");
                }
            }
        }
        #endregion
        #endregion

        public int? abisepa { get; set; }
        public bool abisepaBool
        {
            get
            {
                return abisepa == 1;
            }
            set
            {
                if (value)
                    abisepa = 1;
                else
                    abisepa = 0;
            }
        }

        public string? abiest { get; set; }
        public bool abiestBool
        {
            get
            {
                return abiest == "";
            }
            set
            {
                if (value)
                    abiest = "";
                else
                    abiest = "*";
            }
        }

        public DateTime? abidaa { get; set; }
        public DateTime? abidad { get; set; }

        public string? abiswift { get; set; }
        public int? abinud { get; set; }
    }
}
