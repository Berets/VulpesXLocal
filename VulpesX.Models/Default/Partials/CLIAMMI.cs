using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class CLIAMMI
    {
        public bool CLSPEBBool
        {
            get
            {
                return CLSPEB == "S";
            }
            set
            {
                if (value)
                    CLSPEB = "S";
                else
                    CLSPEB = "N";
            }
        }

        public bool CLASBOBool
        {
            get
            {
                return CLASBO == "S";
            }
            set
            {
                if (value)
                    CLASBO = "S";
                else
                    CLASBO = "N";
            }

        }

        public bool CLRAGRBool
        {
            get
            {
                return CLRAGR == "S";
            }
            set
            {
                if (value)
                    CLRAGR = "S";
                else
                    CLRAGR = "N";
            }

        }

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
                if (!string.IsNullOrWhiteSpace(clGRUP))
                    SelectedGroup = groupsList?.Where(w => w.P1GRUP == clGRUP).FirstOrDefault();
                else
                    SelectedGroup = null;
                NotifyPropertyChanged("GroupsList");
            }
        }
        // bank
        private ObservableCollection<PDCCONTI>? accountsList;
        public ObservableCollection<PDCCONTI>? AccountsList
        {
            get { return accountsList; }
            set
            {
                accountsList = value;
                if (!string.IsNullOrWhiteSpace(clGRUP) && !string.IsNullOrWhiteSpace(clcont))
                    SelectedAccount = accountsList?.Where(w => w.P1GRUP == clGRUP && w.P2CONT == clcont).FirstOrDefault();
                else
                    SelectedAccount = null;
                NotifyPropertyChanged("AccountsList");
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
                    if (value != null && AccountCache != null)
                    {
                        AccountsList = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsList = null;
                    }
                    if (selectedGroup != null)
                    {
                        SelectedAccount = null;
                    }
                    clGRUP = value?.P1GRUP;
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
                    clcont = value?.P2CONT;
                    selectedAccount = value;
                    NotifyPropertyChanged("SelectedAccount");
                }
            }
        }
        #endregion

        public string? clitop { get; set; }
        public bool clitopBool
        {
            get
            {
                return clitop == "S";
            }
            set
            {
                if (value)
                    clitop = "S";
                else
                    clitop = "N";
            }

        }

        public string? cliimballo { get; set; }
        public bool cliimballoBool
        {
            get
            {
                return cliimballo == "S";
            }
            set
            {
                if (value)
                    cliimballo = "S";
                else
                    cliimballo = "N";
            }

        }

        public string? cletiqr { get; set; }
        public bool cletiqrBool
        {
            get
            {
                return cletiqr == "S";
            }
            set
            {
                if (value)
                    cletiqr = "S";
                else
                    cletiqr = "N";
            }

        }

        public string? clibloccodisegno { get; set; }
        public bool clibloccodisegnoBool
        {
            get
            {
                return clibloccodisegno == "S";
            }
            set
            {
                if (value)
                    clibloccodisegno = "S";
                else
                    clibloccodisegno = "N";
            }

        }

        public string? clisister { get; set; }
        public bool clisisterBool
        {
            get
            {
                return clisister == "S";
            }
            set
            {
                if (value)
                    clisister = "S";
                else
                    clisister = "N";
            }

        }

        public string? climailfatture { get; set; }
        public bool climailfattureBool
        {
            get
            {
                return climailfatture == "S";
            }
            set
            {
                if (value)
                    climailfatture = "S";
                else
                    climailfatture = "N";
            }

        }

        public string? cliaasso { get; set; }
        public string? cliaaliq { get; set; }

        public string cliinv { get; set; } = "N";
        public bool cliinvBool
        {
            get
            {
                return cliinv == "S";
            }
            set
            {
                if (value)
                    cliinv = "S";
                else
                    cliinv = "N";
            }
        }

        public string? clipackingordine { get; set; }
        public bool clipackingordineBool
        {
            get
            {
                return clipackingordine == "S";
            }
            set
            {
                if (value)
                    clipackingordine = "S";
                else
                    clipackingordine = "N";
            }

        }

        public string? contqobb { get; set; }
        public bool contqobbBool
        {
            get
            {
                return contqobb == "S";
            }
            set
            {
                if (value)
                    contqobb = "S";
                else
                    contqobb = "N";
            }

        }

        public string? clobbcer { get; set; }
        public bool clobbcerBool
        {
            get
            {
                return clobbcer == "S";
            }
            set
            {
                if (value)
                    clobbcer = "S";
                else
                    clobbcer = "N";
            }

        }

        public string? cliinvbo { get; set; }
        public bool cliinvboBool
        {
            get
            {
                return cliinvbo == "S";
            }
            set
            {
                if (value)
                    cliinvbo = "S";
                else
                    cliinvbo = "N";
            }

        }

        public string clblospe { get; set; } = "N";
        public bool clblospeBool
        {
            get
            {
                return clblospe == "S";
            }
            set
            {
                if (value)
                    clblospe = "S";
                else
                    clblospe = "N";
            }

        }

        public short? clipezzicl { get; set; }
        public decimal? clivalore { get; set; }

        public string? clifattcau { get; set; }
        public bool clifattcauBool
        {
            get
            {
                return clifattcau == "S";
            }
            set
            {
                if (value)
                    clifattcau = "S";
                else
                    clifattcau = "N";
            }

        }

        public string? clicidi { get; set; }
        public string? cliiva { get; set; }
    }
}
