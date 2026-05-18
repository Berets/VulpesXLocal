using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Ufp
{
    public partial class COMTIPREGLEVEL1
    {
        public bool IsInsert { get; set; }
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
                if (!string.IsNullOrWhiteSpace(Cogrup))
                    SelectedGroup = groupsList?.Where(w => w.P1GRUP == Cogrup).FirstOrDefault();
                else
                    SelectedGroup = null;
                NotifyPropertyChanged("GroupsList");
                NotifyPropertyChanged("GroupDescription");
            }
        }
        private ObservableCollection<PDCCONTI>? accountsList;
        public ObservableCollection<PDCCONTI>? AccountsList
        {
            get { return accountsList; }
            set
            {
                accountsList = value;
                if (!string.IsNullOrWhiteSpace(Cogrup) && !string.IsNullOrWhiteSpace(Cocont))
                    SelectedAccount = accountsList?.Where(w => w.P1GRUP == Cogrup && w.P2CONT == Cocont).FirstOrDefault();
                else
                    SelectedAccount = null;
                NotifyPropertyChanged("AccountsList");
                NotifyPropertyChanged("AccountDescription");
            }
        }
        private ObservableCollection<PDCSOTTO>? subaccountsList;
        public ObservableCollection<PDCSOTTO>? SubaccountsList
        {
            get { return subaccountsList; }
            set
            {
                subaccountsList = value;
                if (!string.IsNullOrWhiteSpace(Cogrup) && !string.IsNullOrWhiteSpace(Cocont) && !string.IsNullOrWhiteSpace(CoSotc))
                    SelectedSubaccount = subaccountsList?.Where(w => w.P1GRUP == Cogrup && w.P2CONT == Cocont && w.P3SOTC == CoSotc).FirstOrDefault();
                else
                    SelectedSubaccount = null;
                NotifyPropertyChanged("SubaccountsList");
                NotifyPropertyChanged("SubaccountDescription");
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
