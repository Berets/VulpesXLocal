using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Shipping;
using VulpesX.DAL.Store;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Models.Models;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class BOLLD00FMagnifierWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public BOLLD00FMagnifierWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required string Title { get; set; }
        public required BOLLD00F Row { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsEnabled => !IsReadonly;

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00FRepository>().Validate(Row, true);
        }

        public ObservableCollection<GenericIDDescription>? GetDESTINATARIs(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().GetSimpleList(EntityID, true);
        }

        public ObservableCollection<StockInfo>? GetStockInfos(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>().GetListByProduct(CompanyID, ProductID);
        }

        public GenericPriceInfo? GetCurrentCustomer(string ProductID, int EntityID, int RecipientID, DateTime Date, decimal Quantity, string UM)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().GetCurrent(CompanyID, ProductID, EntityID, RecipientID, Date, Quantity, UM);
        }

        public GenericPriceInfo? GetCurrentGeneric(string ProductID, DateTime Date, decimal Quantity, string UM)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>().GetCurrent(CompanyID, ProductID, Date, Quantity, UM);
        }

        public GenericPriceInfo? GetLastPriceDifferentCustomer(string ProductID, int EntityID, int Year, int Number)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>().GetLastPriceDifferentCustomer(CompanyID, ProductID, EntityID, Year, Number);
        }

        public GenericPriceInfo? GetLastPriceSameCustomer(string ProductID, int EntityID, int Year, int Number)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>().GetLastPriceSameCustomer(CompanyID, ProductID, EntityID, Year, Number);
        }
    }
}
