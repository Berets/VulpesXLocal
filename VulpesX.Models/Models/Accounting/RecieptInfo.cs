using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Accounting
{
    public class ReceiptInfo : Base
    {
        public required string CompanyID { get; set; }
        public decimal Amount { get; set; }
        public ASSOGGETAMENTI? Rate { get; set; }
        public ObservableCollection<ASSOGGETAMENTI>? Rates { get; set; }

        public TCECO00F? CostCenter { get; set; }
        public ObservableCollection<TCECO00F>? CostCenters { get; set; }

        #region PDC

        private ObservableCollection<PDCGRUPPI>? groupsList;
        public ObservableCollection<PDCGRUPPI>? GroupsList
        {
            get { return groupsList; }
            set
            {
                groupsList = value;
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
                    //if (value != null)
                    //{
                    //    AccountsList = new PDCCONTIService().GetList(value.P1GRUP);
                    //}
                    //else
                    //{
                    //    AccountsList = null;
                    //}
                    if (selectedGroup != null)
                    {
                        SelectedAccount = null;
                        SelectedSubaccount = null;
                        SubaccountsList = null;
                    }
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
                if (selectedAccount?.P1GRUP != value?.P1GRUP && selectedAccount?.P2CONT != value?.P2CONT)
                {

                    //if (value != null)
                    //{
                    //    SubaccountsList = new PDCSOTTOService().GetList(value.P1GRUP, value.P2CONT, CompanyID);
                    //}
                    //else
                    //{
                    //    SubaccountsList = null;
                    //}
                    if (selectedAccount != null)
                    {
                        SelectedSubaccount = null;
                        SubaccountsList = null;
                    }
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
                if (selectedSubaccount?.P1GRUP != value?.P1GRUP && selectedSubaccount?.P2CONT != value?.P2CONT && selectedSubaccount?.P3SOTC != value?.P3SOTC)
                {
                    selectedSubaccount = value;
                    NotifyPropertyChanged("SelectedSubaccount");
                }
            }
        }
        #endregion

        #region Methods
        public string? Validate()
        {
            if (Amount > 0)
            {
                if (Rate != null)
                {
                    if (SelectedGroup != null && SelectedAccount != null && SelectedSubaccount != null)
                    {
                        return null;
                    }
                    else
                    { return "Il conto del ricavo e' obbligatorio"; }
                }
                else
                {
                    return "Selezionare un'aliquota valida";
                }
            }
            else
            {
                return "L'importo è obbligatorio e deve essere maggiore di 0";
            }
        }
        #endregion

        #region Computed
        public decimal Taxable
        {
            get
            {
                if (Rate != null)
                {
                    int rateValue = 0;
                    if (int.TryParse(Rate.assali, out rateValue))
                    {
                        decimal coeff = decimal.Parse($"1.{(rateValue < 10 ? $"0{rateValue}" : $"{rateValue}")}", new CultureInfo("en-US"));
                        return Math.Round((Amount / coeff), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    { return Amount; }
                }
                else
                { return Amount; }
            }
        }
        public decimal Tax => Math.Round(Amount - Taxable, 2, MidpointRounding.AwayFromZero);
        #endregion
    }
}
