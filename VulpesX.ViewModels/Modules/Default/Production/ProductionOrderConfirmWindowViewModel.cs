using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VulpesX.Models.Default.Partials;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Production
{
    public class ProductionOrderConfirmWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ProductionOrderConfirmWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required pro_ordine Order { get; set; }

        private decimal quantityOrdered;
        public decimal QuantityOrdered { get => quantityOrdered; set { quantityOrdered = value; NotifyPropertyChanged("QuantityOrdered"); } }

        private decimal quantityAvailable;
        public decimal QuantityAvailable { get => quantityAvailable; set { quantityAvailable = value; NotifyPropertyChanged("QuantityOrdered"); } }

        private decimal quantityToEngage;
        public decimal QuantityToEngage { get => quantityToEngage; set { quantityToEngage = value; NotifyPropertyChanged("QuantityOrdered"); } }

        private decimal quantityToProduce;
        public decimal QuantityToProduce { get => quantityToProduce; set { quantityToProduce = value; NotifyPropertyChanged("QuantityToProduce"); } }

        public ObservableCollection<string>? Revisions { get; set; }
        public string? OlderRevisionID { get; set; }
        public decimal? OriginalQuantity { get; set; }
        public ObservableCollection<StockInfo>? AvailableStocks { get; set; }
        public ORDIT00F? CustomerOrder { get; set; }
        public ObservableCollection<pro_ordine_composizione>? ComponentsList { get; set; }
        public ObservableCollection<pro_ordine_composizione>? HalfworkedList { get; set; }

        public ObservableCollection<StockInfo>? CheckAvailabilityByProduct(string ProductID, string? Lot)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>().CheckAvailabilityByProduct(CompanyID, ProductID, Lot);
        }

        public ObservableCollection<store_stocks_engage>? GetList()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().GetList(CompanyID, Order.ArticoloID ?? string.Empty, long.Parse(Order.ID), false);
        }

        public ORDIT00F? GetPrintFull()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().GetPrintFull(CompanyID, Order.OrdineClienteAnno ?? 0, (int)(Order.OrdineClienteID ?? 0), false, false, false);
        }

        public tab_articolo? GetTab_Articolo(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(CompanyID, ProductID);

        }

        public ObservableCollection<string>? GetRevisioniSimpleList()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetRevisioniSimpleList(CompanyID, Order.ArticoloID ?? string.Empty);

        }

        public bool Update()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().Update(Order);

        }

        public bool Insertstore_stocks_engage(pro_ordine_composizione Composition, StockInfo Lot)
        {
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            var numerator = numRegRepo.GetFullLongID(now.Year, (numRegRepo.GetNumber(CompanyID, now.Year, Constants.STORE_ENGAGES, true)));

            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().Insert(new store_stocks_engage()
            {
                company_id = CompanyID,
                id = numerator,
                store_id = Lot.StoreID ?? string.Empty,
                product_id = Composition.ComponenteArticoloID ?? string.Empty,
                job_id = Order.ID,
                order_id = Order.ID,
                quantity = Lot.QuantityToEngage,
                lot = Lot.Lot != Constants.NO_LOT_ID ? Lot.Lot : null,
                add_user = UserID
            });
        }


        public bool Insertstore_stocks_engage(StockInfo Lot)
        {
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            var numerator = numRegRepo.GetFullLongID(now.Year, (numRegRepo.GetNumber(CompanyID, now.Year, Constants.STORE_ENGAGES, true)));

            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().Insert(new store_stocks_engage()
            {
                company_id = CompanyID,
                id = numerator,
                store_id = Lot.StoreID ?? string.Empty,
                product_id = Order.ArticoloID ?? string.Empty,
                job_id = Order.ID,
                order_id = Order.ID,
                quantity = Lot.QuantityToEngage,
                lot = Lot.Lot != Constants.NO_LOT_ID ? Lot.Lot : null,
                add_user = UserID
            });
        }

        public bool Recreate(bool Delete)
        {
            try
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();

                long newProdID = 0;
                if (Delete)
                {
                    newProdID = long.Parse(Order.ID);
                    VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().Delete(Order, UserID);
                }
                else
                {
                    newProdID = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(CompanyID, now.Year, Constants.PRODUCTION_ORDERS, true));
                }

                bool recreated = VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().GenerateProductionOrder(CustomerOrder!, UserID, newProdID, Order.RevisioneID, false);

                if (recreated)
                    Order = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().Get(CompanyID, newProdID.ToString())!;

                return recreated;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public ObservableCollection<store_stocks_engage>? GetSimpleListByProductionOrder()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().GetSimpleListByProductionOrder(CompanyID, long.Parse(Order.ID));
        }

        public ObservableCollection<pro_ordine_composizione>? GetMaterialsListByOrder()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizioneRepository>().GetMaterialsListByOrder(CompanyID, Order.ID);
        }

        public ObservableCollection<pro_ordine_composizione>? GetHalfmadeListByOrder()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizioneRepository>().GetHalfmadeListByOrder(CompanyID, Order.ID);
        }

        public bool Updatepro_ordine_composizione(pro_ordine_composizione Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizioneRepository>().Update(Item);
        }

        public bool Updatepro_ordine_composizioneHalf(string ID, string ProductID, string Revision, decimal Quantity)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizioneRepository>().UpdateHalfmade(CompanyID, ID, ProductID, Revision, Quantity);
        }

        public bool Insertpro_ordine_history(string Operation, string PreviousState)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_historyRepository>().Insert(new pro_ordine_history()
            {
                company_id = CompanyID,
                id = Order.ID,
                operation = Operation,
                cancelled_engages = false,
                production_quantity = (Order.Quantita ?? 0),
                engaged_quantity = (VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().GetByOrderIDEveryLot(CompanyID, Order.ID, Order.ArticoloID ?? string.Empty)?.quantity ?? 0),
                previous_state = PreviousState,
                username = UserID,
                client_name = Environment.MachineName
            });
        }

        public bool FreeByProductionOrder()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().FreeByProductionOrder(CompanyID, Order.ID, UserID);
        }

        public bool Delete(pro_ordine Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().Delete(Item, UserID);
        }

        public bool UpdateORDID00F(pro_ordine Item)
        {
            var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();

            var detail = ordidRepo.Get(Item.SocietaID, Item.OrdineClienteAnno ?? 0, (int)(Item.OrdineClienteID ?? 0), (int)(Item.OrdineClienteRiga ?? 0));

            if (detail != null)
            {
                detail.ODSTA = null;
                return ordidRepo.Update(detail);
            }

            return false;
        }

        public bool GenerateProductionOrder()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().GenerateProductionOrder(CustomerOrder!, UserID, long.Parse(Order.ID), OlderRevisionID, true);
        }

        public bool EngageAndClose()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().EngageAndClose(Order, AvailableStocks ?? new(), UserID);
        }

        public bool InsertSRM_RDA(pro_ordine_composizione Composition)
        {
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();

            decimal quantity = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().ComputeQuantityToOrder(Composition.SocietaID, Composition.ComponenteArticoloID ?? string.Empty, (Composition.Quantita ?? 0) - (Composition.QuantitaImpegnata + Composition.QuantitaGiaImpegnata));
            var defaultSupplierID = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetDefaultSupplier(Composition.SocietaID, Composition.ComponenteArticoloID ?? string.Empty);

            return VulpesServiceProvider.Provider.GetRequiredService<ISRM_RDARepository>().Insert(new SRM_RDA()
            {
                companyID = CompanyID,
                id = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(CompanyID, now.Year, Constants.RDA, true)),
                product_id = Composition.ComponenteArticoloID ?? string.Empty,
                note = quantity > 0 ? $"RDA generata automaticamente dall'ordine di produzione {Order.ID}" : $"RDA generata automaticamente dall'ordine di produzione {Order.ID} con errori nel calcolo della quantità da ordinare, verificare anagrafica articolo",
                original_needed = (Composition.Quantita ?? 0) - (Composition.QuantitaImpegnata + Composition.QuantitaGiaImpegnata),
                quantity = quantity,
                is_customer_material = false,
                production_order_id = Order.ID,
                requested_delivery = now,
                addedUserID = UserID,
                approval_date = UserContext.Instance!.ACCESS!.Roles?.canApproveRDA ?? false ? now : null,
                approval_user = UserContext.Instance!.ACCESS!.Roles?.canApproveRDA ?? false ? UserID : null,
                supplier_id = defaultSupplierID
            });
        }
    }
}
