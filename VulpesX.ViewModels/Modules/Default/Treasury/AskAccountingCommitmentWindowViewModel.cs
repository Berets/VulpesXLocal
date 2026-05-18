using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Treasury;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Treasury
{
    public class AskAccountingCommitmentWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public AskAccountingCommitmentWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public int? AccountingYear { get; set; }
        public DateTime? SelectedDate { get; set; }

        public required TES_IMFI Item { get; set; }

        public CAUCONT? SelectedCausal { get; set; }
        public ObservableCollection<CAUCONT>? Causals { get; set; }
        public ObservableCollection<PDCGRUPPI>? Groups { get; set; }

        private PDCGRUPPI? selectedGroup;
        public PDCGRUPPI? SelectedGroup { get => selectedGroup; set { selectedGroup = value; NotifyPropertyChanged("SelectedGroup"); } }

        private ObservableCollection<PDCCONTI>? accounts;
        public ObservableCollection<PDCCONTI>? Accounts { get => accounts; set { accounts = value; NotifyPropertyChanged("Accounts"); } }

        private PDCCONTI? selectedAccount;
        public PDCCONTI? SelectedAccount { get => selectedAccount; set { selectedAccount = value; NotifyPropertyChanged("SelectedAccount"); } }

        private ObservableCollection<PDCSOTTO>? subaccounts;
        public ObservableCollection<PDCSOTTO>? Subaccounts { get => subaccounts; set { subaccounts = value; NotifyPropertyChanged("Subaccounts"); } }

        private PDCSOTTO? selectedSubaccount;
        public PDCSOTTO? SelectedSubaccount
        {
            get => selectedSubaccount; set { selectedSubaccount = value; NotifyPropertyChanged("SelectedSubaccount"); }
        }

        public void LoadDetails()
        {
            SelectedDate = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
            Groups = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public ObservableCollection<ESERCIZIO>? GetESERCIZIO()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public ObservableCollection<PDCCONTI>? GetPDCCONTIs(string GroupID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetList(GroupID);
        }

        public ObservableCollection<PDCSOTTO>? GetPDCSOTTOs(string GroupID, string AccountID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(GroupID, AccountID, CompanyID);
        }

        public bool Accounting()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITES_IMFIRepository>().Accounting(Item, AccountingYear!.Value, SelectedDate!.Value, SelectedCausal!, SelectedGroup!.P1GRUP, SelectedAccount!.P2CONT, SelectedSubaccount!.P3SOTC, UserID);
        }
    }
}
