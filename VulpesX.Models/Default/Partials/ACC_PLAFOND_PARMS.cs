using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ACC_PLAFOND_PARMS
    {
        public string? UM { get; set; }
        public string? IVANature { get; set; }

        #region Rates
        private ObservableCollection<ASSOGGETAMENTI>? ratesList;
        public ObservableCollection<ASSOGGETAMENTI>? RatesList
        {
            get { return ratesList; }
            set
            {
                ratesList = value;
                if (!string.IsNullOrWhiteSpace(rate_code) && !string.IsNullOrWhiteSpace(rate_value))
                    SelectedRate = ratesList?.Where(w => w.assali == rate_value && w.asscod == rate_code).FirstOrDefault();
                else
                    SelectedRate = null;
                NotifyPropertyChanged("SelectedRate");
            }
        }

        private ASSOGGETAMENTI? selectedRate;

        public ASSOGGETAMENTI? SelectedRate
        {
            get => selectedRate;
            set
            {
                if (value != null)
                {
                    if (selectedRate?.assali != value?.assali || selectedRate?.asscod != value?.asscod)
                    {
                        selectedRate = value;

                        if (value != null)
                        {
                            rate_code = value!.asscod;
                            rate_value = value!.assali;
                        }

                        NotifyPropertyChanged("SelectedRate");
                    }
                }
                else
                {
                    rate_code = string.Empty;
                    rate_value = string.Empty;
                }
            }
        }
        #endregion

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
                if (!string.IsNullOrWhiteSpace(group_id))
                    SelectedGroup = groupsList?.Where(w => w.P1GRUP == group_id).FirstOrDefault();
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
                if (!string.IsNullOrWhiteSpace(group_id) && !string.IsNullOrWhiteSpace(account_id))
                    SelectedAccount = accountsList?.Where(w => w.P1GRUP == group_id && w.P2CONT == account_id).FirstOrDefault();
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
                if (!string.IsNullOrWhiteSpace(group_id) && !string.IsNullOrWhiteSpace(account_id) && !string.IsNullOrWhiteSpace(subaccount_id))
                    SelectedSubaccount = subaccountsList?.Where(w => w.P1GRUP == group_id && w.P2CONT == account_id && w.P3SOTC == subaccount_id).FirstOrDefault();
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
                if (value == null || selectedGroup?.P1GRUP != value?.P1GRUP)
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
                if (value == null || selectedAccount?.P1GRUP != value?.P1GRUP && selectedAccount?.P2CONT != value?.P2CONT)
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
                selectedSubaccount = value;
                NotifyPropertyChanged("SelectedSubaccount");
            }
        }
        #endregion

        #region Rates
        private ObservableCollection<tab_articolo>? productsList;
        public ObservableCollection<tab_articolo>? ProductsList
        {
            get { return productsList; }
            set
            {
                productsList = value;
                if (!string.IsNullOrWhiteSpace(product_id))
                    SelectedProduct = productsList?.Where(w => w.ID == product_id).FirstOrDefault();
                else
                    SelectedProduct = null;
                NotifyPropertyChanged("SelectedProduct");
            }
        }

        private tab_articolo? selectedProduct;

        public tab_articolo? SelectedProduct
        {
            get => selectedProduct;
            set
            {
                if (value != null)
                {
                    if (selectedProduct?.ID != value?.assali)
                    {
                        selectedProduct = value;
                        product_id = value?.ID;
                        NotifyPropertyChanged("SelectedProduct");
                    }
                }
                else
                {
                    product_id = null;
                }
            }
        }
        #endregion

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        #endregion
    }
}
