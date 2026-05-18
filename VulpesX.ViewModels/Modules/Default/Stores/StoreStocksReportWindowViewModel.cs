using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Stores
{
    public class StoreStocksReportWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public StoreStocksReportWindowViewModel()
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

        public ObservableCollection<tab_articolo>? Products { get; set; }
        public tab_articolo? SelectedProduct { get; set; }
        public ObservableCollection<store_stores>? Stores { get; set; }
        public store_stores? SelectedStore { get; set; }
        public ObservableCollection<tab_articolo_tipo>? ProductTypes { get; set; }
        public tab_articolo_tipo? SelectedProductType { get; set; }

        public DateTime? PrintUntil { get; set; }

        public bool ShowZeroStocks { get; set; }
        public bool ShowMovements { get; set; }
        public bool ShowMovementsCanceled { get; set; }
        public bool ShowEngages { get; set; }
        public bool ShowEngagesUnloaded { get; set; }
        public bool ShowInfinite { get; set; }
        public bool ShowLots { get; set; }

        public void LoadDetails()
        {
            Stores = VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().GetList(CompanyID, true);

            Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID, true);

            ProductTypes = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_tipoRepository>().GetList(CompanyID, true);
        }

        public async Task<StocksListReport?> GetStocksListReport()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>().PrintStocks(CompanyID, ShowInfinite, ShowZeroStocks, ShowLots, ShowMovements, ShowEngages, ShowMovementsCanceled, ShowEngagesUnloaded, SelectedStore?.id, SelectedProductType?.ID, SelectedProduct?.ID, PrintUntil);
                });

                return result;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
