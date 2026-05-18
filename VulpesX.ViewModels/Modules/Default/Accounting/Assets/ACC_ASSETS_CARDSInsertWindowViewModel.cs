using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Assets
{
    public class ACC_ASSETS_CARDSInsertWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ACC_ASSETS_CARDSInsertWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public int SelectedYear { get; set; }

        private string? selectedGroupID;
        public string? SelectedGroupID { get => selectedGroupID; set { selectedGroupID = value; NotifyPropertyChanged("SelectedGroupID"); } }

        private string? selectedAccountID;
        public string? SelectedAccountID { get => selectedAccountID; set { selectedAccountID = value; NotifyPropertyChanged("SelectedAccountID"); } }

        private string? selectedSubaccountID;
        public string? SelectedSubaccountID { get => selectedSubaccountID; set { selectedSubaccountID = value; NotifyPropertyChanged("SelectedSubaccountID"); } }

        private ObservableCollection<PDCGRUPPI>? groups;
        public ObservableCollection<PDCGRUPPI>? Groups { get => groups; set { groups = value; NotifyPropertyChanged("Groups"); } }

        private ObservableCollection<PDCCONTI>? accounts;
        public ObservableCollection<PDCCONTI>? Accounts { get => accounts; set { accounts = value; NotifyPropertyChanged("Accounts"); } }

        private ObservableCollection<PDCSOTTO>? subaccounts;
        public ObservableCollection<PDCSOTTO>? Subaccounts { get => subaccounts; set { subaccounts = value; NotifyPropertyChanged("Subaccounts"); } }

        public ObservableCollection<PDCGRUPPI>? GetPDCGRUPPIs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public ObservableCollection<PDCCONTI>? GetPDCCONTIs(string GroupID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetList(GroupID);
        }

        public ObservableCollection<PDCSOTTO>? GetPDCSOTTOs(string GroupID, string AccountID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(GroupID, AccountID, CompanyID);
        }
    }
}
