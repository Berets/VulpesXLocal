using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.CRM;
using VulpesX.Models;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Treasury
{
    public class BankFluxesWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public int Year { get; set; }
        public required string MonthFlag { get; set; }
        public required string GroupID;
        public required string AccountID;
        public required string SubaccountID;
        public required string Bank;
        public required string Agency;

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public decimal StartBalance { get; set; }
        public string? StartBalanceSign { get; set; }

        public BankFluxesWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public ObservableCollection<BankFluxItem>? Items { get; set; }
        public ObservableCollection<BankFluxMonthItem>? ItemsMonths { get; set; }

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

        public async Task Load()
        {
            IsBusy = true;

            var result = await Task.Run(() =>
            {
                var pnRigheRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();

                var items = pnRigheRepo.GetBankFluxes(CompanyID, Year,GroupID,AccountID,SubaccountID);
                var trend = pnRigheRepo.GetBankFluxesMonth(CompanyID, Year, GroupID, AccountID, SubaccountID);

                return new { items, trend };
            });

            Items = result.items;
            ItemsMonths = result.trend;

            IsBusy = false;

        }
    }
}
