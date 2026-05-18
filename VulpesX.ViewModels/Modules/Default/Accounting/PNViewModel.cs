using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Accounting;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static VulpesX.Models.Models.StockCheckExistance;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class PNViewModel : Base
    {
        public required string CompanyID { get; set; }

        public PNViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CostCenters = VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, true);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBusySitutation;
        public bool IsBusySitutation
        {
            get { return _isBusySitutation; }
            set
            {
                _isBusySitutation = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime year;
        public DateTime Year
        {
            get => year; set
            {
                year = value;
                NotifyPropertyChanged();
            }
        }

        private string? textFilter;
        public string? TextFilter
        {
            get => textFilter; set
            {
                textFilter = value;
                NotifyPropertyChanged();
            }
        }

        private int pageSize;
        public int PageSize
        {
            get => pageSize; set
            {
                pageSize = value;
                NotifyPropertyChanged();
            }
        }

        private int pageRequested;
        public int PageRequested
        {
            get => pageRequested; set
            {
                pageRequested = value;
                NotifyPropertyChanged();
            }
        }

        private int totalCount;
        public int TotalCount
        {
            get => totalCount; set
            {
                totalCount = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<PNTESTATA>? items;
        public ObservableCollection<PNTESTATA>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        private AccountingSituationViewModel? accountingSituation;
        public AccountingSituationViewModel? AccountingSituation { get => accountingSituation; set { accountingSituation = value; NotifyPropertyChanged("AccountingSituation"); } }

        public ObservableCollection<TCECO00F>? CostCenters { get; set; }

        private TCECO00F? selectedCostCenter;
        public TCECO00F? SelectedCostCenter
        {
            get { return selectedCostCenter; }
            set
            {
                selectedCostCenter = value;
                NotifyPropertyChanged();
            }
        }
        public ESERCIZIO? Esercizio { get; set; }

        public async Task Load(List<GenericIDDescription> Sorts, List<FilterEntry> Wheres)
        {
            IsBusy = true;

            try
            {
                var accountingRepo = VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>();

                var result = await Task.Run(() =>
                {
                    int itemsCount = 0;

                    var items = accountingRepo.GetHeadList(CompanyID, Year.Year, TextFilter, PageSize, PageRequested, Sorts, Wheres, out itemsCount);

                    var esercizio = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, Year.Year);

                    return new { items, itemsCount, esercizio };
                });

                Items = result.items;
                TotalCount = result.itemsCount;
                Esercizio = result.esercizio;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadSituation()
        {
            IsBusySitutation = true;

            try
            {
                var accountingRepo = VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>();

                var result = await Task.Run(() =>
                {
                    var situation = accountingRepo.GetAccountingSituation(CompanyID, Year.Year, SelectedCostCenter?.cecodc);

                    return new { situation };
                });

                AccountingSituation = result.situation;
            }
            finally
            {
                IsBusySitutation = false;
            }
        }

        public ObservableCollection<AccountingSituationViewModel.ASItem>? GetAccountingSituationDetail(string? GroupID, bool IsDare, string? AccountID)
        {
            var accountingRepo =
                   VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>();

            return accountingRepo.GetAccountingSituationDetails(CompanyID, Year.Year, GroupID ?? string.Empty, IsDare, AccountID ?? string.Empty);
        }

        public ObservableCollection<CAUCONT>? GetCAUCONT(string StateID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList(StateID);
        }

        public PNTESTATA? GetPNTESTATA(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Get(CompanyID, Year, ID);
        }

        public ObservableCollection<ABE>? GetABE(string N1FLCF)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(N1FLCF);
        }

        public bool IsReadOnly(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().PrintedOnGeneralJournal(CompanyID, Year, ID);
        }

        public AccountingRecordReport? GetPrintAccountingRecord(PNTESTATA Model)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().PrintAccountingRecord(Model);
        }

        public bool CheckPrinted(string CompanyID, int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().CheckPrinted(CompanyID, Year, ID);
        }

        public bool Delete(PNTESTATA Model)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Delete(Model);
        }

        public string? ExportCEEBalance(int Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().ExportCEEBalance(CompanyID, Year);
        }
    }
}
