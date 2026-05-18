using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.SRM;
using VulpesX.DAL.Store;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Models.Models;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class ACQOrderDetailMagnifierWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ACQOrderDetailMagnifierWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required string Title { get; set; }
        public required acq_orders_rows Row { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsEnabled => !IsReadonly;

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().Validate(Row, true);
        }

        public ObservableCollection<StockInfo>? GetStockInfos(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>().GetListByProduct(CompanyID, ProductID);
        }

        public GenericPriceInfo? GetCurrenSupplier(string ProductID, int EntityID, DateTime Date, decimal Quantity, string UM)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISRM_LISFORRepository>().GetCurrent(CompanyID, ProductID, EntityID, Date, Quantity, UM);
        }

        public GenericPriceInfo? GetLastPriceDifferentSupplier(string ProductID, int EntityID, long CurrentOrderID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().GetLastPriceDifferentSupplier(CompanyID, ProductID, EntityID, CurrentOrderID);
        }

        public GenericPriceInfo? GetLastPriceSameSupplier(string ProductID, int EntityID, long CurrentOrderID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().GetLastPriceSameSupplier(CompanyID, ProductID, EntityID, CurrentOrderID);
        }

        public tab_articolo_costi? GetLast(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_costiRepository>().GetLast(CompanyID, ProductID);
        }
    }
}
