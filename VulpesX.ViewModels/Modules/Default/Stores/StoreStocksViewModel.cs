using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Stores
{
    public class StoreStocksViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public StoreStocksViewModel()
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

        private ObservableCollection<store_stocks>? items;
        public ObservableCollection<store_stocks>? Items { get => items; set { items = value; NotifyPropertyChanged("Items"); } }

        private ObservableCollection<store_movements>? movements;
        public ObservableCollection<store_movements>? Movements { get => movements; set { movements = value; NotifyPropertyChanged("Movements"); } }

        private ObservableCollection<store_stocks_engage>? engages;
        public ObservableCollection<store_stocks_engage>? Engages { get => engages; set { engages = value; NotifyPropertyChanged("Engages"); } }

        public ObservableCollection<store_stores>? Stores { get; set; }

        public store_stores? SelectedStore { get; set; }

        public bool ShowInfinite { get; set; }
        public bool ShowZeroLots { get; set; }

        public string? Warnings { get; set; }

        public ObservableCollection<store_stores>? GetStore_Stores()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().GetList(CompanyID, true);
        }

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                string? warnings = null;

                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>().GetList(CompanyID, SelectedStore!.id, ShowInfinite, out warnings);
                });

                Items = result;
                Warnings = warnings;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadMovements(string ProductID)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GetList(CompanyID, ProductID);
                });

                Movements = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadEngagements(string ProductID)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().GetList(CompanyID, ProductID);
                });

                Engages = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public long GetEngageNumerator()
        {
            var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            return numRegRepository.GetFullLongID(now.Year, numRegRepository.GetNumber(CompanyID, now.Year, Constants.STORE_ENGAGES, true));
        }

        public long GetMovementNumerator()
        {
            var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            return numRegRepository.GetFullLongID(now.Year, numRegRepository.GetNumber(CompanyID, now.Year, Constants.STORE_MOVEMENTS, true));
        }

        public bool UpdateEngage(store_stocks_engage Engage)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().Update(Engage);
        }

        public ObservableCollection<store_stocks_lots>? GetStore_Stocks_Lots(string StoreID, string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_lotsRepository>().GetList(CompanyID, StoreID, ProductID, ShowZeroLots);
        }

        public LotLabelReport? GetPrintLotLabel(string StoreID, string ProductID, string Lot)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_lotsRepository>().PrintLotLabel(CompanyID, StoreID, ProductID, Lot);
        }

        public tab_articolo? GetArticolo(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(CompanyID, ProductID);
        }
    }
}
