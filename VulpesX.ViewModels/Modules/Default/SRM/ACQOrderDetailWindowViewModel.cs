using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.General;
using VulpesX.DAL.SRM;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Article;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class ACQOrderDetailWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ACQOrderDetailWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required acq_orders_heads Head { get; set; }


        private ObservableCollection<acq_orders_rows>? rows;
        public ObservableCollection<acq_orders_rows>? Rows
        {
            get { return rows; }
            set
            {
                rows = value;
                Parallel.ForEach(rows ?? new ObservableCollection<acq_orders_rows>(), (item) =>
                {
                    item.UMsCache = UMsCache;
                    item.ProductTypes = ProductTypes;
                    item.OrderDate = Head.order_date ?? DateTime.Now;
                    item.Products = Products;
                    item.Stores = Stores;
                    item.Rates = Rates;
                    item.SupplierID = Head.supplier_id;
                    item.ProductOrQuantityValueChanged += OnProductOrQuantityChanged;
                });
            }
        }
        public bool IsReadonly { get; set; }
        public bool IsEnabled => !IsReadonly;
        public bool HasMergedSigns { get; set; }
        public ObservableCollection<tab_articolo>? Products { get; set; }
        public ObservableCollection<store_stores>? Stores { get; set; }
        public ObservableCollection<ASSOGGETAMENTI>? Rates { get; set; }
        public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }
        public ObservableCollection<tab_articolo_tipo>? ProductTypes { get; set; }

        public void OnProductOrQuantityChanged(object? sender, EventArgs? e)
        {
            var item = sender as acq_orders_rows;

            if (item != null)
            {
                if (item.quantity.HasValue && !item.price.HasValue)
                {
                    var lastSup = VulpesServiceProvider.Provider.GetRequiredService<ISRM_LISFORRepository>().GetCurrent(item.company_id, item.product_id ?? string.Empty, item.SupplierID, item.OrderDate, (item.quantity ?? 0), item.unit_id);

                    if (lastSup != null)
                    {
                        item.price = lastSup.Price;
                        item.discount = lastSup.Discount1;
                        item.discount_type = lastSup.DiscountType1;
                    }
                }
            }
        }

        public void LoadDetails()
        {
            UMsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);
            ProductTypes = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_tipoRepository>().GetList(CompanyID, false);
            Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
            Stores = VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().GetList(CompanyID, true);
            Rates = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            Rows = VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().GetListFull(Head.company_id, Head.id) ?? new ObservableCollection<acq_orders_rows>();
        }

        public string? Validate(acq_orders_rows Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().Validate(Item, true);
        }

        public string? ValidateModel()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().ValidateModel(Rows ?? new ObservableCollection<acq_orders_rows>());
        }


        public bool Save()
        {
            Head.updatedUserID = UserID;

            if (HasMergedSigns)
            {
                // all signs merged
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                Head.commercial_signed = now;
                Head.commercial_signer = UserID;
                Head.management_signed = now;
                Head.management_signer = UserID;
            }

            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().UpdateAll(Head, Rows);
        }
    }
}
