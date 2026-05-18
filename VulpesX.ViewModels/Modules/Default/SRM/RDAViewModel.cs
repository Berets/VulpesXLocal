using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Assets;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.SRM;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class RDAViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public RDAViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<SRM_RDA>? _items;
        public ObservableCollection<SRM_RDA>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        public async Task Load(string StatusID)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<ISRM_RDARepository>().GetList(CompanyID, StatusID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public long GetNumerator(GenericIDDescription Type)
        {
            var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            return numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(CompanyID, now.Year, Type, true));
        }

        public bool Update(SRM_RDA Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISRM_RDARepository>().Update(Item);
        }

        public bool UnlockCustomerOrder(ORDIT00F OrderHead, ORDID00F OrderRow, ObservableCollection<store_stocks_engage> Engages)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISRM_RDARepository>().UnlockCustomerOrder(OrderHead, OrderRow, Engages, UserID);
        }

        public ORDIT00F? GetORDIT00F(string CompanyID, int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().Get(CompanyID, Year, ID);
        }

        public ORDID00F? GetORDID00F(string CompanyID, int Year, int ID, int Row)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>().Get(CompanyID, Year, ID, Row);
        }

        public ObservableCollection<store_stocks_engage>? GetStore_Stocks_Engages(string CompanyID, int Year, int ID, int Row)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().GetListByCustomerOrderID(CompanyID, Year, ID, Row);
        }

        public FORNAMMI? GetFORNAMMI(int SupplierID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(CompanyID, SupplierID);
        }

        public tab_articolo? GetTab_Articolo(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(CompanyID, ProductID);
        }

        public GenericPriceInfo? GetGenericPriceInfo(string ProductID, int SupplierID, DateTime Reference, string UM)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISRM_LISFORRepository>().GetCurrent(CompanyID, ProductID, SupplierID, Reference, UM);
        }

        public string? GetDefaultStoreRaw()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().GetDefaultRawLoad(CompanyID)?.id;
        }

        public bool Insert_acq_orders_heads(acq_orders_heads Order)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().InsertFull(Order);
        }
    }
}
