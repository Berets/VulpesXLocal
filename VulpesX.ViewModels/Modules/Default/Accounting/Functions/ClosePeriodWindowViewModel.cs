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
    public class ClosePeriodWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ClosePeriodWindowViewModel()
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


        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }



       

        public async Task<bool> PeriodClosing()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().PeriodClosing(CompanyID, AccountingYear, UntilDate!.Value);
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
