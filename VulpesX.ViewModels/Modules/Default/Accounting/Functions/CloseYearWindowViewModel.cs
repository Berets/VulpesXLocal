using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Functions
{
    public class CloseYearWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public CloseYearWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
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

        public int AccountingYear { get; set; }

        private DateTime? untilDate;
        public DateTime? UntilDate { get => untilDate; set { untilDate = value; NotifyPropertyChanged("UntilDate"); } }

        private DateTime? newDate;
        public DateTime? NewDate { get => newDate; set { newDate = value; NotifyPropertyChanged("NewDate"); } }

        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public ESERCIZIO? GetESERCIZIO(int Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, Year);
        }

        public async Task<decimal> ComputeLossProfit()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().ComputeLossProfit(CompanyID, AccountingYear);
                });

                IsBusy = false;

                return result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> YearClosing()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().YearClosing(CompanyID, AccountingYear, UntilDate!.Value, NewDate!.Value, UserID);
                });

                IsBusy = false;

                return result;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
