using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class TAB_CRM_CAUORD
    {
        public string? CompanyID { get; set; }
        public CAUSBOLL? CausalDDT { get; set; }
        public string FullDescriptionSearchable => $"{cauacq} {caudec?.Trim()}";
        public TAB_GEN_TEXTS? Text { get; set; }
        public bool cauflbBool
        {
            get
            {
                return cauflb == "S";
            }
            set
            {
                if (value)
                    cauflb = "S";
                else
                    cauflb = "N";
            }
        }

        public string? cauflaDescription => OrderTypes.Where(w => w.ID == caufla).FirstOrDefault()?.Description;
        public ObservableCollection<GenericIDDescription> OrderTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "V", Description = "Vendita" },
            new GenericIDDescription(){ ID = null, Description = "Nessuno" }
        };

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
                if (!string.IsNullOrWhiteSpace(caugrp))
                    SelectedGroup = groupsList?.Where(w => w.P1GRUP == caugrp).FirstOrDefault();
                else
                    SelectedGroup = null;
                NotifyPropertyChanged("GroupsList");
                NotifyPropertyChanged("GroupDescription");
            }
        }
        private ObservableCollection<PDCCONTI>? accountList;
        public ObservableCollection<PDCCONTI>? AccountList
        {
            get { return accountList; }
            set
            {
                accountList = value;
                if (!string.IsNullOrWhiteSpace(caugrp) && !string.IsNullOrWhiteSpace(caucon))
                    SelectedAccount = accountList?.Where(w => w.P1GRUP == caugrp && w.P2CONT == caucon).FirstOrDefault();
                else
                    SelectedAccount = null;
                NotifyPropertyChanged("AccountList");
                NotifyPropertyChanged("AccountDescription");
            }
        }
        private ObservableCollection<PDCSOTTO>? subaccountList;
        public ObservableCollection<PDCSOTTO>? SubaccountList
        {
            get { return subaccountList; }
            set
            {
                subaccountList = value;
                if (!string.IsNullOrWhiteSpace(caugrp) && !string.IsNullOrWhiteSpace(caucon) && !string.IsNullOrWhiteSpace(causoc))
                    SelectedSubaccount = subaccountList?.Where(w => w.P1GRUP == caugrp && w.P2CONT == caucon && w.P3SOTC == causoc).FirstOrDefault();
                else
                    SelectedSubaccount = null;
                NotifyPropertyChanged("SubaccountList");
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
                    AccountList = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == selectedGroup.P1GRUP).ToList());
                }
                else
                {
                    AccountList = null;
                }
                GroupDescription = selectedGroup?.FullDescriptionSearchable;
                NotifyPropertyChanged("SelectedGroup");
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
                    SubaccountList = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == selectedAccount.P1GRUP && w.P2CONT == selectedAccount.P2CONT).ToList());
                }
                else
                {
                    SubaccountList = null;
                }
                AccountDescription = selectedAccount?.FullDescriptionSearchable;
                NotifyPropertyChanged("SelectedAccount");
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
            }
        }
        public string? SubaccountDescription { get; set; }

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
