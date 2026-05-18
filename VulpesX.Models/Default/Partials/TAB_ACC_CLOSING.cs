using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class TAB_ACC_CLOSING
    {
        public string? CompanyID { get; set; }
        public string FullDescriptionSearchable => $"{cchcod} {cchdes?.Trim()}";

        public bool cchpprBool
        {
            get
            {
                return cchppr == "S";
            }
            set
            {
                if (value)
                    cchppr = "S";
                else
                    cchppr = "N";
            }
        }

        #region Causal closing
        private ObservableCollection<CAUCONT>? causalsClosing;
        public ObservableCollection<CAUCONT>? CausalsClosing
        {
            get { return causalsClosing; }
            set
            {
                causalsClosing = value;
                if (!string.IsNullOrWhiteSpace(cchchi))
                    CausalClosing = causalsClosing?.Where(w => w.caucod == cchchi).FirstOrDefault();
                else
                    CausalClosing = null;
                NotifyPropertyChanged("CausalsClosing");
            }
        }

        private CAUCONT? causalClosing;

        public CAUCONT? CausalClosing
        {
            get => causalClosing;
            set
            {
                if (causalClosing?.caucod != value?.caucod)
                {
                    cchchi = value?.caucod;
                    causalClosing = value;
                    NotifyPropertyChanged("CausalClosing");
                }
            }
        }
        #endregion

        public List<PDCCONTI>? AccountCache { get; set; }
        public List<PDCSOTTO>? SubaccountCache { get; set; }

        #region PDC closing
        private ObservableCollection<PDCGRUPPI>? groupsClosing;
        public ObservableCollection<PDCGRUPPI>? GroupsClosing
        {
            get { return groupsClosing; }
            set
            {
                groupsClosing = value;
                if (!string.IsNullOrWhiteSpace(cchgrc))
                    GroupClosing = groupsClosing?.Where(w => w.P1GRUP == cchgrc).FirstOrDefault();
                else
                    GroupClosing = null;
                NotifyPropertyChanged("GroupsClosing");
            }
        }

        private ObservableCollection<PDCCONTI>? accountsClosing;
        public ObservableCollection<PDCCONTI>? AccountsClosing
        {
            get => accountsClosing;
            set
            {
                accountsClosing = value;
                if (!string.IsNullOrWhiteSpace(cchgrc) && !string.IsNullOrWhiteSpace(cchctc))
                    AccountClosing = accountsClosing?.Where(w => w.P1GRUP == cchgrc && w.P2CONT == cchctc).FirstOrDefault();
                else
                    AccountClosing = null;
                NotifyPropertyChanged("AccountsClosing");
            }
        }

        private ObservableCollection<PDCSOTTO>? subaccountsClosing;
        public ObservableCollection<PDCSOTTO>? SubaccountsClosing
        {
            get { return subaccountsClosing; }
            set
            {
                subaccountsClosing = value;
                if (!string.IsNullOrWhiteSpace(cchgrc) && !string.IsNullOrWhiteSpace(cchctc) && !string.IsNullOrWhiteSpace(cchstc))
                    SubaccountClosing = subaccountsClosing?.Where(w => w.P1GRUP == cchgrc && w.P2CONT == cchctc && w.P3SOTC == cchstc).FirstOrDefault();
                else
                    SubaccountClosing = null;
                NotifyPropertyChanged("SubaccountsClosing");
            }
        }

        private PDCGRUPPI? groupClosing;
        public PDCGRUPPI? GroupClosing
        {
            get => groupClosing;
            set
            {
                if (groupClosing?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        AccountsClosing = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsClosing = null;
                    }
                    if (groupClosing != null)
                    {
                        AccountClosing = null;
                        SubaccountClosing = null;
                    }
                    cchgrc = value?.P1GRUP;
                    groupClosing = value;
                    NotifyPropertyChanged("GroupClosing");
                }
            }
        }

        private PDCCONTI? accountClosing;
        public PDCCONTI? AccountClosing
        {
            get => accountClosing;
            set
            {
                if (accountClosing?.P1GRUP != value?.P1GRUP || accountClosing?.P2CONT != value?.P2CONT)
                {
                    if (value != null && SubaccountCache != null)
                    {
                        SubaccountsClosing = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        SubaccountsClosing = null;
                    }
                    if (accountClosing != null)
                    {
                        SubaccountClosing = null;
                    }
                    cchctc = value?.P2CONT;
                    accountClosing = value;
                    NotifyPropertyChanged("AccountClosing");
                }
            }
        }

        private PDCSOTTO? subaccountClosing;

        public PDCSOTTO? SubaccountClosing
        {
            get => subaccountClosing;
            set
            {
                if (subaccountClosing?.P1GRUP != value?.P1GRUP || subaccountClosing?.P2CONT != value?.P2CONT || subaccountClosing?.P3SOTC != value?.P3SOTC)
                {
                    cchstc = value?.P3SOTC;
                    subaccountClosing = value;
                    NotifyPropertyChanged("SubaccountClosing");
                }
            }
        }
        #endregion

        #region Causal reopen
        private ObservableCollection<CAUCONT>? causalsReopen;
        public ObservableCollection<CAUCONT>? CausalsReopen
        {
            get { return causalsReopen; }
            set
            {
                causalsReopen = value;
                if (!string.IsNullOrWhiteSpace(cchria))
                    CausalReopen = causalsReopen?.Where(w => w.caucod == cchria).FirstOrDefault();
                else
                    CausalReopen = null;
                NotifyPropertyChanged("CausalsReopen");
            }
        }

        private CAUCONT? causalReopen;

        public CAUCONT? CausalReopen
        {
            get => causalReopen;
            set
            {
                if (causalReopen?.caucod != value?.caucod)
                {
                    cchria = value?.caucod;
                    causalReopen = value;
                    NotifyPropertyChanged("CausalReopen");
                }
            }
        }
        #endregion

        #region PDC reopen
        private ObservableCollection<PDCGRUPPI>? groupsReopen;
        public ObservableCollection<PDCGRUPPI>? GroupsReopen
        {
            get { return groupsReopen; }
            set
            {
                groupsReopen = value;
                if (!string.IsNullOrWhiteSpace(cchgrr))
                    GroupReopen = groupsReopen?.Where(w => w.P1GRUP == cchgrr).FirstOrDefault();
                else
                    GroupReopen = null;
                NotifyPropertyChanged("GroupsReopen");
            }
        }
        private ObservableCollection<PDCCONTI>? accountsReopen;
        public ObservableCollection<PDCCONTI>? AccountsReopen
        {
            get => accountsReopen;
            set
            {
                accountsReopen = value;
                if (!string.IsNullOrWhiteSpace(cchgrr) && !string.IsNullOrWhiteSpace(cchctr))
                    AccountReopen = accountsReopen?.Where(w => w.P1GRUP == cchgrr && w.P2CONT == cchctr).FirstOrDefault();
                else
                    AccountReopen = null;
                NotifyPropertyChanged("AccountsReopen");
            }
        }
        private ObservableCollection<PDCSOTTO>? subaccountsReopen;
        public ObservableCollection<PDCSOTTO>? SubaccountsReopen
        {
            get { return subaccountsReopen; }
            set
            {
                subaccountsReopen = value;
                if (!string.IsNullOrWhiteSpace(cchgrr) && !string.IsNullOrWhiteSpace(cchctr) && !string.IsNullOrWhiteSpace(cchstr))
                    SubaccountReopen = subaccountsReopen?.Where(w => w.P1GRUP == cchgrr && w.P2CONT == cchctr && w.P3SOTC == cchstr).FirstOrDefault();
                else
                    SubaccountReopen = null;
                NotifyPropertyChanged("SubaccountsReopen");
            }
        }

        private PDCGRUPPI? groupReopen;
        public PDCGRUPPI? GroupReopen
        {
            get => groupReopen;
            set
            {
                if (groupReopen?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        AccountsReopen = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsReopen = null;
                    }
                    if (groupReopen != null)
                    {
                        AccountReopen = null;
                        SubaccountReopen = null;
                    }
                    cchgrr = value?.P1GRUP;
                    groupReopen = value;
                    NotifyPropertyChanged("GroupReopen");
                }
            }
        }

        private PDCCONTI? accountReopen;
        public PDCCONTI? AccountReopen
        {
            get => accountReopen;
            set
            {
                if (accountReopen?.P1GRUP != value?.P1GRUP || accountReopen?.P2CONT != value?.P2CONT)
                {
                    if (value != null && SubaccountCache != null)
                    {
                        SubaccountsReopen = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        SubaccountsReopen = null;
                    }
                    if (accountReopen != null)
                    {
                        SubaccountReopen = null;
                    }
                    cchctr = value?.P2CONT;
                    accountReopen = value;
                    NotifyPropertyChanged("AccountReopen");
                }
            }
        }

        private PDCSOTTO? subaccountReopen;

        public PDCSOTTO? SubaccountReopen
        {
            get => subaccountReopen;
            set
            {
                if (subaccountReopen?.P1GRUP != value?.P1GRUP || subaccountReopen?.P2CONT != value?.P2CONT || subaccountReopen?.P3SOTC != value?.P3SOTC)
                {
                    cchstr = value?.P3SOTC;
                    subaccountReopen = value;
                    NotifyPropertyChanged("SubaccountReopen");
                }
            }
        }
        #endregion

        #region Causal loss
        private ObservableCollection<CAUCONT>? causalsLoss;
        public ObservableCollection<CAUCONT>? CausalsLoss
        {
            get { return causalsLoss; }
            set
            {
                causalsLoss = value;
                if (!string.IsNullOrWhiteSpace(cchpes))
                    CausalLoss = causalsLoss?.Where(w => w.caucod == cchpes).FirstOrDefault();
                else
                    CausalLoss = null;
                NotifyPropertyChanged("CausalsLoss");
            }
        }

        private CAUCONT? causalLoss;

        public CAUCONT? CausalLoss
        {
            get => causalLoss;
            set
            {
                if (causalLoss?.caucod != value?.caucod)
                {
                    cchpes = value?.caucod;
                    causalLoss = value;
                    NotifyPropertyChanged("CausalLoss");
                }
            }
        }
        #endregion

        #region PDC loss
        private ObservableCollection<PDCGRUPPI>? groupsLoss;
        public ObservableCollection<PDCGRUPPI>? GroupsLoss
        {
            get { return groupsLoss; }
            set
            {
                groupsLoss = value;
                if (!string.IsNullOrWhiteSpace(cchgrp))
                    GroupLoss = groupsLoss?.Where(w => w.P1GRUP == cchgrp).FirstOrDefault();
                else
                    GroupLoss = null;
                NotifyPropertyChanged("GroupsLoss");
            }
        }
        private ObservableCollection<PDCCONTI>? accountsLoss;
        public ObservableCollection<PDCCONTI>? AccountsLoss
        {
            get => accountsLoss;
            set
            {
                accountsLoss = value;
                if (!string.IsNullOrWhiteSpace(cchgrp) && !string.IsNullOrWhiteSpace(cchctp))
                    AccountLoss = accountsLoss?.Where(w => w.P1GRUP == cchgrp && w.P2CONT == cchctp).FirstOrDefault();
                else
                    AccountLoss = null;
                NotifyPropertyChanged("AccountsLoss");
            }
        }
        private ObservableCollection<PDCSOTTO>? subaccountsLoss;
        public ObservableCollection<PDCSOTTO>? SubaccountsLoss
        {
            get { return subaccountsLoss; }
            set
            {
                subaccountsLoss = value;
                if (!string.IsNullOrWhiteSpace(cchgrp) && !string.IsNullOrWhiteSpace(cchctp) && !string.IsNullOrWhiteSpace(cchstp))
                    SubaccountLoss = subaccountsLoss?.Where(w => w.P1GRUP == cchgrp && w.P2CONT == cchctp && w.P3SOTC == cchstp).FirstOrDefault();
                else
                    SubaccountLoss = null;
                NotifyPropertyChanged("SubaccountsLoss");
            }
        }

        private PDCGRUPPI? groupLoss;
        public PDCGRUPPI? GroupLoss
        {
            get => groupLoss;
            set
            {
                if (groupLoss?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        AccountsLoss = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsLoss = null;
                    }
                    if (groupLoss != null)
                    {
                        AccountLoss = null;
                        SubaccountLoss = null;
                    }
                    cchgrp = value?.P1GRUP;
                    groupLoss = value;
                    NotifyPropertyChanged("GroupLoss");
                }
            }
        }

        private PDCCONTI? accountLoss;
        public PDCCONTI? AccountLoss
        {
            get => accountLoss;
            set
            {
                if (accountLoss?.P1GRUP != value?.P1GRUP || accountLoss?.P2CONT != value?.P2CONT)
                {
                    if (value != null && SubaccountCache != null)
                    {
                        SubaccountsLoss = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        SubaccountsLoss = null;
                    }
                    if (accountLoss != null)
                    {
                        SubaccountLoss = null;
                    }
                    cchctp = value?.P2CONT;
                    accountLoss = value;
                    NotifyPropertyChanged("AccountLoss");
                }
            }
        }

        private PDCSOTTO? subaccountLoss;

        public PDCSOTTO? SubaccountLoss
        {
            get => subaccountLoss;
            set
            {
                if (subaccountLoss?.P1GRUP != value?.P1GRUP || subaccountLoss?.P2CONT != value?.P2CONT || subaccountLoss?.P3SOTC != value?.P3SOTC)
                {
                    cchstp = value?.P3SOTC;
                    subaccountLoss = value;
                    NotifyPropertyChanged("SubaccountLoss");
                }
            }
        }
        #endregion

        #region Causal profit
        private ObservableCollection<CAUCONT>? causalsProfit;
        public ObservableCollection<CAUCONT>? CausalsProfit
        {
            get { return causalsProfit; }
            set
            {
                causalsProfit = value;
                if (!string.IsNullOrWhiteSpace(cchues))
                    CausalProfit = causalsProfit?.Where(w => w.caucod == cchues).FirstOrDefault();
                else
                    CausalProfit = null;
                NotifyPropertyChanged("CausalsProfit");
            }
        }

        private CAUCONT? causalProfit;

        public CAUCONT? CausalProfit
        {
            get => causalProfit;
            set
            {
                if (causalProfit?.caucod != value?.caucod)
                {
                    cchues = value?.caucod;
                    causalProfit = value;
                    NotifyPropertyChanged("CausalProfit");
                }
            }
        }
        #endregion

        #region PDC profit
        private ObservableCollection<PDCGRUPPI>? groupsProfit;
        public ObservableCollection<PDCGRUPPI>? GroupsProfit
        {
            get { return groupsProfit; }
            set
            {
                groupsProfit = value;
                if (!string.IsNullOrWhiteSpace(cchgru))
                    GroupProfit = groupsProfit?.Where(w => w.P1GRUP == cchgru).FirstOrDefault();
                else
                    GroupProfit = null;
                NotifyPropertyChanged("GroupsProfit");
            }
        }
        private ObservableCollection<PDCCONTI>? accountsProfit;
        public ObservableCollection<PDCCONTI>? AccountsProfit
        {
            get => accountsProfit;
            set
            {
                accountsProfit = value;
                if (!string.IsNullOrWhiteSpace(cchgru) && !string.IsNullOrWhiteSpace(cchctu))
                    AccountProfit = accountsProfit?.Where(w => w.P1GRUP == cchgru && w.P2CONT == cchctu).FirstOrDefault();
                else
                    AccountProfit = null;
                NotifyPropertyChanged("AccountsProfit");
            }
        }
        private ObservableCollection<PDCSOTTO>? subaccountsProfit;
        public ObservableCollection<PDCSOTTO>? SubaccountsProfit
        {
            get { return subaccountsProfit; }
            set
            {
                subaccountsProfit = value;
                if (!string.IsNullOrWhiteSpace(cchgru) && !string.IsNullOrWhiteSpace(cchctu) && !string.IsNullOrWhiteSpace(cchstu))
                    SubaccountProfit = subaccountsProfit?.Where(w => w.P1GRUP == cchgru && w.P2CONT == cchctu && w.P3SOTC == cchstu).FirstOrDefault();
                else
                    SubaccountProfit = null;
                NotifyPropertyChanged("SubaccountsProfit");
            }
        }

        private PDCGRUPPI? groupProfit;
        public PDCGRUPPI? GroupProfit
        {
            get => groupProfit;
            set
            {
                if (groupProfit?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        AccountsProfit = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsProfit = null;
                    }
                    if (groupProfit != null)
                    {
                        AccountProfit = null;
                        SubaccountProfit = null;
                    }
                    cchgru = value?.P1GRUP;
                    groupProfit = value;
                    NotifyPropertyChanged("GroupProfit");
                }
            }
        }

        private PDCCONTI? accountProfit;
        public PDCCONTI? AccountProfit
        {
            get => accountProfit;
            set
            {
                if (accountProfit?.P1GRUP != value?.P1GRUP || accountProfit?.P2CONT != value?.P2CONT)
                {
                    if (value != null && SubaccountCache != null)
                    {
                        SubaccountsProfit = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        SubaccountsProfit = null;
                    }
                    if (accountProfit != null)
                    {
                        SubaccountProfit = null;
                    }
                    cchctu = value?.P2CONT;
                    accountProfit = value;
                    NotifyPropertyChanged("AccountProfit");
                }
            }
        }

        private PDCSOTTO? subaccountProfit;

        public PDCSOTTO? SubaccountProfit
        {
            get => subaccountProfit;
            set
            {
                if (subaccountProfit?.P1GRUP != value?.P1GRUP || subaccountProfit?.P2CONT != value?.P2CONT || subaccountProfit?.P3SOTC != value?.P3SOTC)
                {
                    cchstu = value?.P3SOTC;
                    subaccountProfit = value;
                    NotifyPropertyChanged("SubaccountProfit");
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
