using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class tab_articolo
    {
        private bool _IsSelected;
        public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; NotifyPropertyChanged(); } }

        public string FullDescriptionSearchable => $"{ID} {Descrizione?.TrimEnd()}";
        public string FullDescriptionSearchableUfp => $"{ID} {artdise?.TrimEnd()}";
        public string? RevisioneID { get; set; }
        public string? UltimaRevisioneID { get; set; }
        public bool HaComposizione { get; set; } = false;
        public bool HaDipendenze { get; set; } = false;
        public bool HaAllegato { get; set; } = false;
        public bool HaImmagine { get; set; } = false;
        public string? TipoDescrizione { get; set; }
        public string? UnitaDescrizione { get; set; }
        public string? CategoriaDescrizione { get; set; }

        #region Cost center
        private ObservableCollection<TCECO00F>? costCentersList;
        public ObservableCollection<TCECO00F>? CostCentersList
        {
            get => costCentersList;
            set
            {
                costCentersList = value;
                if (!string.IsNullOrWhiteSpace(costcenter_id))
                    CostCenter = costCentersList?.Where(w => w.cecodc == costcenter_id).FirstOrDefault();
                else
                    CostCenter = null;
                NotifyPropertyChanged("CostCentersList");
            }
        }

        private TCECO00F? costCenter;
        public TCECO00F? CostCenter
        {
            get => costCenter;
            set
            {
                costcenter_id = value?.cecodc;
                costCenter = value;
                NotifyPropertyChanged("CostCenter");
            }
        }
        #endregion

        #region PDC - revenue
        public List<PDCCONTI>? AccountCache { get; set; }
        public List<PDCSOTTO>? SubaccountCache { get; set; }

        private ObservableCollection<PDCGRUPPI>? revenueGroupsList;
        public ObservableCollection<PDCGRUPPI>? RevenueGroupsList
        {
            get { return revenueGroupsList; }
            set
            {
                revenueGroupsList = value;
                if (!string.IsNullOrWhiteSpace(GroupID))
                    RevenueGroup = revenueGroupsList?.Where(w => w.P1GRUP == GroupID).FirstOrDefault();
                else
                    RevenueGroup = null;
                NotifyPropertyChanged("RevenueGroupsList");
            }
        }
        private ObservableCollection<PDCCONTI>? revenueAccountsList;
        public ObservableCollection<PDCCONTI>? RevenueAccountsList
        {
            get { return revenueAccountsList; }
            set
            {
                revenueAccountsList = value;
                if (!string.IsNullOrWhiteSpace(GroupID) && !string.IsNullOrWhiteSpace(AccountID))
                    RevenueAccount = revenueAccountsList?.Where(w => w.P1GRUP == GroupID && w.P2CONT == AccountID).FirstOrDefault();
                else
                    RevenueAccount = null;
                NotifyPropertyChanged("RevenueAccountsList");
            }
        }
        private ObservableCollection<PDCSOTTO>? revenueSubaccountsList;
        public ObservableCollection<PDCSOTTO>? RevenueSubaccountsList
        {
            get { return revenueSubaccountsList; }
            set
            {
                revenueSubaccountsList = value;
                if (!string.IsNullOrWhiteSpace(GroupID) && !string.IsNullOrWhiteSpace(AccountID) && !string.IsNullOrWhiteSpace(SubaccountID))
                    RevenueSubaccount = revenueSubaccountsList?.Where(w => w.P1GRUP == GroupID && w.P2CONT == AccountID && w.P3SOTC == SubaccountID).FirstOrDefault();
                else
                    RevenueSubaccount = null;
                NotifyPropertyChanged("RevenueSubaccountsList");
            }
        }

        private PDCGRUPPI? revenueGroup;
        public PDCGRUPPI? RevenueGroup
        {
            get
            {
                return revenueGroup;
            }
            set
            {
                if (revenueGroup?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        RevenueAccountsList = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        RevenueAccountsList = null;
                    }
                    if (revenueGroup != null)
                    {
                        RevenueAccount = null;
                        RevenueSubaccount = null;
                    }
                    GroupID = value?.P1GRUP;
                    revenueGroup = value;
                    NotifyPropertyChanged("RevenueGroup");
                }
            }
        }

        private PDCCONTI? revenueAccount;
        public PDCCONTI? RevenueAccount
        {
            get
            {
                return revenueAccount;
            }
            set
            {
                if (revenueAccount?.P1GRUP != value?.P1GRUP || revenueAccount?.P2CONT != value?.P2CONT)
                {

                    if (value != null && SubaccountCache != null)
                    {
                        RevenueSubaccountsList = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        RevenueSubaccountsList = null;
                    }
                    if (revenueAccount != null)
                    {
                        RevenueSubaccount = null;
                    }
                    AccountID = value?.P2CONT;
                    revenueAccount = value;
                    NotifyPropertyChanged("RevenueAccount");
                }
            }
        }

        private PDCSOTTO? revenueSubaccount;

        public PDCSOTTO? RevenueSubaccount
        {
            get
            {
                return revenueSubaccount;
            }
            set
            {
                if (revenueSubaccount?.P1GRUP != value?.P1GRUP && revenueSubaccount?.P2CONT != value?.P2CONT && revenueSubaccount?.P3SOTC != value?.P3SOTC)
                {
                    SubaccountID = value?.P3SOTC;
                    revenueSubaccount = value;
                    NotifyPropertyChanged("RevenueSubaccount");
                }
            }
        }
        #endregion

        #region PDC - cost
        private ObservableCollection<PDCGRUPPI>? costGroupsList;
        public ObservableCollection<PDCGRUPPI>? CostGroupsList
        {
            get { return costGroupsList; }
            set
            {
                costGroupsList = value;
                if (!string.IsNullOrWhiteSpace(cost_group_id))
                    CostGroup = costGroupsList?.Where(w => w.P1GRUP == cost_group_id).FirstOrDefault();
                else
                    CostGroup = null;
                NotifyPropertyChanged("CostGroupsList");
            }
        }
        private ObservableCollection<PDCCONTI>? costAccountsList;
        public ObservableCollection<PDCCONTI>? CostAccountsList
        {
            get { return costAccountsList; }
            set
            {
                costAccountsList = value;
                if (!string.IsNullOrWhiteSpace(cost_group_id) && !string.IsNullOrWhiteSpace(cost_account_id))
                    CostAccount = costAccountsList?.Where(w => w.P1GRUP == cost_group_id && w.P2CONT == cost_account_id).FirstOrDefault();
                else
                    CostAccount = null;
                NotifyPropertyChanged("CostAccountsList");
            }
        }
        private ObservableCollection<PDCSOTTO>? costSubaccountsList;
        public ObservableCollection<PDCSOTTO>? CostSubaccountsList
        {
            get { return costSubaccountsList; }
            set
            {
                costSubaccountsList = value;
                if (!string.IsNullOrWhiteSpace(cost_group_id) && !string.IsNullOrWhiteSpace(cost_account_id) && !string.IsNullOrWhiteSpace(cost_subaccount_id))
                    CostSubaccount = costSubaccountsList?.Where(w => w.P1GRUP == cost_group_id && w.P2CONT == cost_account_id && w.P3SOTC == cost_subaccount_id).FirstOrDefault();
                else
                    CostSubaccount = null;
                NotifyPropertyChanged("CostSubaccountsList");
            }
        }

        private PDCGRUPPI? costGroup;
        public PDCGRUPPI? CostGroup
        {
            get
            {
                return costGroup;
            }
            set
            {
                if (costGroup?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        CostAccountsList = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        CostAccountsList = null;
                    }
                    if (costGroup != null)
                    {
                        CostAccount = null;
                        CostSubaccount = null;
                    }
                    cost_group_id = value?.P1GRUP;
                    costGroup = value;
                    NotifyPropertyChanged("CostGroup");
                }
            }
        }

        private PDCCONTI? costAccount;
        public PDCCONTI? CostAccount
        {
            get
            {
                return costAccount;
            }
            set
            {
                if (costAccount?.P1GRUP != value?.P1GRUP || costAccount?.P2CONT != value?.P2CONT)
                {

                    if (value != null && SubaccountCache != null)
                    {
                        CostSubaccountsList = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        CostSubaccountsList = null;
                    }
                    if (costAccount != null)
                    {
                        CostSubaccount = null;
                    }
                    cost_account_id = value?.P2CONT;
                    costAccount = value;
                    NotifyPropertyChanged("CostAccount");
                }
            }
        }

        private PDCSOTTO? costSubaccount;

        public PDCSOTTO? CostSubaccount
        {
            get
            {
                return costSubaccount;
            }
            set
            {
                if (costSubaccount?.P1GRUP != value?.P1GRUP || costSubaccount?.P2CONT != value?.P2CONT || costSubaccount?.P3SOTC != value?.P3SOTC)
                {
                    cost_subaccount_id = value?.P3SOTC;
                    costSubaccount = value;
                    NotifyPropertyChanged("CostSubaccount");
                }
            }
        }
        #endregion

        #region Info
        public string AddedText => LogAdded.HasValue ? LogAdded.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(LogAddedUserID) ? LogAddedUserID : "---";
        public string UpdatedText => LogUpdated.HasValue ? LogUpdated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(LogUpdatedUserID) ? LogUpdatedUserID : "---";
        public string CanceledText => LogCanceled.HasValue ? LogCanceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(LogCanceledUserID) ? LogCanceledUserID : "---";
        #endregion

        public string? artdise { get; set; }
        public string? artven { get; set; }

        public string? artmp1 { get; set; }
        public string? artmp1Descrizione { get; set; }
        public string? artmp1Full { get { return $"{artmp1?.TrimEnd()} - {artmp1Descrizione}"; } }

        public int? artfor1m { get; set; }
        public string? artfor1mDescrizione { get; set; }
        public string? artfor1mFull { get { return $"{artfor1m?.ToString()} - {artfor1mDescrizione}"; } }


        public string? arttip { get; set; }
        public string? arttipDescrizione { get; set; }
        public string? arttipFull { get { return $"{arttip?.TrimEnd()} - {arttipDescrizione}"; } }

        public string? artfam { get; set; }
        public string? artfamDescrizione { get; set; }
        public string? artfamFull { get { return $"{artfam?.TrimEnd()} - {artfamDescrizione}"; } }

        public string? artcor { get; set; }
        public string? artcorDescrizione { get; set; }
        public string? artcorFull { get { return $"{artcor?.TrimEnd()} - {artcorDescrizione}"; } }

        public string? arttipmat { get; set; }
        public string? arttipmatDescrizione { get; set; }
        public string? arttipmatFull { get { return $"{arttipmat?.TrimEnd()} - {arttipmatDescrizione}"; } }

        public string? artmap { get; set; }
        public string? artmapDescrizione { get; set; }
        public string? artmapFull { get { return $"{artmap?.TrimEnd()} - {artmapDescrizione}"; } }

        public string? artden { get; set; }
        public string? artdenDescrizione { get; set; }
        public string? artdenFull { get { return $"{artden?.TrimEnd()} - {artdenDescrizione}"; } }

        public string? artdiam { get; set; }
        public string? artdiamDescrizione { get; set; }
        public string? artdiamFull { get { return $"{artdiam?.TrimEnd()} - {artdiamDescrizione}"; } }

        public string? artld { get; set; }
        public string? artldDescrizione { get; set; }
        public string? artldFull { get { return $"{artld?.TrimEnd()} - {artldDescrizione}"; } }

        public string? artfori { get; set; }
        public string? artforiDescrizione { get; set; }
        public string? artforiFull { get { return $"{artfori?.TrimEnd()} - {artforiDescrizione}"; } }

        public string? artatco { get; set; }
        public string? artatcoDescrizione { get; set; }
        public string? artatcoFull { get { return $"{artatco?.TrimEnd()} - {artatcoDescrizione}"; } }

        public decimal? artdi1 { get; set; }
        public decimal? artdi2 { get; set; }
        public decimal? artlun { get; set; }
        public decimal? artdi3 { get; set; }
        public decimal? artluncod { get; set; }
        public decimal? artlinuta { get; set; }
        public decimal? artlar { get; set; }
        public decimal? artalt { get; set; }

        public int HaTempiMedi { get; set; }
        public int HaTempiMediCNC { get; set; }
        public string TempiMediBackground
        {
            get
            {
                if (HaTempiMediCNC > 0)
                    return "G";
                if (HaTempiMedi > 0)
                    return "O";
               

                return "R";
            }
        }
    }
}
