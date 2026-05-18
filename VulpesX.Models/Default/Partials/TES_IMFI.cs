using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class TES_IMFI
    {
        public PDCGRUPPI? Group { get; set; }
        public PDCCONTI? Account { get; set; }
        public PDCSOTTO? Subaccount { get; set; }
        public int Month => ifdata.Month;
        public int Year => ifdata.Year;

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
                if (!string.IsNullOrWhiteSpace(ifgrup))
                    SelectedGroup = groupsList?.Where(w => w.P1GRUP == ifgrup).FirstOrDefault();
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
                if (!string.IsNullOrWhiteSpace(ifgrup) && !string.IsNullOrWhiteSpace(ifcont))
                    SelectedAccount = accountsList?.Where(w => w.P1GRUP == ifgrup && w.P2CONT == ifcont).FirstOrDefault();
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
                if (!string.IsNullOrWhiteSpace(ifgrup) && !string.IsNullOrWhiteSpace(ifcont) && !string.IsNullOrWhiteSpace(ifsott))
                    SelectedSubaccount = subaccountsList?.Where(w => w.P1GRUP == ifgrup && w.P2CONT == ifcont && w.P3SOTC == ifsott).FirstOrDefault();
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
                        SelectedSubaccount = null;
                    }

                    ifgrup = (value != null) ? value.P1GRUP : string.Empty;
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

                    if (value != null && SubaccountCache != null)
                    {
                        SubaccountsList = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        SubaccountsList = null;
                    }
                    if (selectedAccount != null)
                    {
                        SelectedSubaccount = null;
                    }
                    ifcont = (value != null) ? value.P2CONT : string.Empty;
                    selectedAccount = value;
                    NotifyPropertyChanged("SelectedAccount");
                }
            }
        }

        private PDCSOTTO? selectedSubaccount;

        public PDCSOTTO? SelectedSubaccount
        {
            get
            {
                return selectedSubaccount;
            }
            set
            {
                if (selectedSubaccount?.P1GRUP != value?.P1GRUP || selectedSubaccount?.P2CONT != value?.P2CONT || selectedSubaccount?.P3SOTC != value?.P3SOTC)
                {
                    ifsott = (value != null) ? value.P3SOTC : string.Empty;
                    selectedSubaccount = value;
                    NotifyPropertyChanged("SelectedSubaccount");
                }
            }
        }
        #endregion
    }
}
