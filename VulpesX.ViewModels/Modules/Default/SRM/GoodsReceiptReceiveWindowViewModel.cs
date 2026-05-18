using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Production;
using VulpesX.DAL.SRM;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class GoodsReceiptReceiveWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public GoodsReceiptReceiveWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required acq_goods_receipts Data { get; set; }
        public required acq_orders_heads OrderHeadData { get; set; }
        public required acq_orders_rows OrderRowData { get; set; }
        public bool ShowLot { get; set; }
        public bool LotVisibility => ShowLot ? true : false;

        public bool LotLabelPrint { get; set; }
        public bool IsLotLocked { get; set; }

        public void LoadDetails()
        {
            IsLotLocked = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID)?.lot_locked ?? true;
            Data.quantity = OrderRowData.quantity - (OrderRowData.quantity_received ?? 0);
            Data.Causals = VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().GetList(CompanyID, "+");
            Data.Stores = VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().GetList(CompanyID);
            ShowLot = OrderRowData.Product?.HasLots ?? false;
            Data.Store = Data.Stores?.Where(w => w.id == OrderRowData.store_id).FirstOrDefault();
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_goods_receiptsRepository>().Validate(Data, ((OrderRowData.quantity ?? 0) - (OrderRowData.quantity_received ?? 0)), true);
        }

        public bool Insert(bool ConfirmClose)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_goods_receiptsRepository>().Insert(Data, OrderRowData, ConfirmClose);
        }

        public void UnlockOrdersProductions()
        {
            VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().UnlockOrders(CompanyID, OrderRowData.Jobs ?? new(), UserID);
        }

        public void UnlockOrdersCustomers()
        {
            VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().UnlockOrders(CompanyID, OrderRowData.CustomerOrders ?? new(), UserID);
        }

        public LotLabelReport? PrintLotLabel()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_lotsRepository>().PrintLotLabel(CompanyID, Data.store_id!, Data.product_id!, Data.lot!);
        }

        public tab_articolo? GetTab_Articolo(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(CompanyID, ProductID);
        }

        public string? GetLot(int ExpireDays)
        {
            var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            string? template = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().GetLotTemplate(CompanyID);

            int numerator = numRegRepo.GetNumber(CompanyID, now.Year, Constants.GOODS_LOTS, true);

            return VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GenerateLotID(template, Data.supplier_id, numerator, ExpireDays);
        }
    }
}
