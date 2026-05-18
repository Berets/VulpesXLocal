using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class FORNAMMI
    {
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
                if (!string.IsNullOrWhiteSpace(foGRUP))
                    SelectedGroup = groupsList?.Where(w => w.P1GRUP == foGRUP).FirstOrDefault();
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
                if (!string.IsNullOrWhiteSpace(foGRUP) && !string.IsNullOrWhiteSpace(foCONT))
                    SelectedAccount = accountsList?.Where(w => w.P1GRUP == foGRUP && w.P2CONT == foCONT).FirstOrDefault();
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
                    foGRUP = value?.P1GRUP;
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
                selectedAccount = value;
                if (selectedAccount != null)
                {
                    foCONT = selectedAccount.P2CONT;
                }
                else
                {
                    foCONT = null;
                }
                NotifyPropertyChanged("SelectedAccount");
            }
        }
        #endregion

        public string? focidi { get; set; }
        public string? foiva { get; set; }
        public string? focodcliedi { get; set; }
        public string? focodconsedi { get; set; }

        public string? foaccobb { get; set; }
        public bool foaccobbBool
        {
            get
            {
                return foaccobb == "S";
            }
            set
            {
                if (value)
                    foaccobb = "S";
                else
                    foaccobb = "N";
            }

        }

        public string? focauacq { get; set; }
    }
}
