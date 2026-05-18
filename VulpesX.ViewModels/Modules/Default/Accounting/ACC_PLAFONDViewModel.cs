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
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class ACC_PLAFONDViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ACC_PLAFONDViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
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

        private DateTime year;
        public DateTime Year
        {
            get => year; set
            {
                year = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<ACC_PLAFOND>? items;
        public ObservableCollection<ACC_PLAFOND>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>().GetList(CompanyID, Year.Year);

                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> Cancel(ACC_PLAFOND Item, string SelectedReason)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    Item.canceled = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                    Item.canceledUserID = UserID;
                    Item.canceledNote = SelectedReason;

                    return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>().Update(Item);
                });

                return result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public int? GetEsercizio()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID)?.Where(w => w.eseest == "U").FirstOrDefault()?.eseann;
        }

        public ACC_PLAFOND_PARMS? GetACC_PLAFOND_PARMS()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>().Get(CompanyID);
        }

        public bool Close(List<ACC_PLAFOND> Rows, DateTime Date)
        {
            try
            {
                var accPlaRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>();

                bool result = true;

                foreach (var row in Rows)
                {
                    row.clidatchi = Date;
                    result = accPlaRepo.Update(row);
                }

                return result;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message.ToString());
                return false;
            }
        }
    }
}
