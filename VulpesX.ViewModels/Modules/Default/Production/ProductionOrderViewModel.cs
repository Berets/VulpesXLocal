using DocumentFormat.OpenXml.Drawing.Charts;
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
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Reports.Production;
using VulpesX.Shared;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VulpesX.ViewModels.Modules.Default.Production
{
    public class ProductionOrderViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ProductionOrderViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<pro_ordine>? _items;
        public ObservableCollection<pro_ordine>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        public async Task Load(int Year ,string StatusID)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().GetList(CompanyID, (long)Year, StatusID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public pro_ordine? Get(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().Get(CompanyID, ID);
        }

        public bool Delete(pro_ordine Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().Delete(Item, UserID);
        }

        public bool Update(pro_ordine Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().Update(Item);
        }

        public bool UpdateStatus(pro_ordine Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().UpdateStatus(Item);
        }

        public bool Insertpro_ordine_history(pro_ordine Item, string Operation, string PreviousState)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_historyRepository>().Insert(new pro_ordine_history()
            {
                company_id = CompanyID,
                id = Item.ID,
                operation = $"{Operation} > {Item.Stato}",
                cancelled_engages = false,
                production_quantity = (Item.Quantita ?? 0),
                engaged_quantity = (VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().GetByOrderIDEveryLot(CompanyID, Item.ID, Item.ArticoloID ?? string.Empty)?.quantity ?? 0),
                previous_state = PreviousState,
                username = UserID,
                client_name = Environment.MachineName,
            });
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

        public string GetNumerator()
        {
            var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            var date = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            return numRegRepo.GetFullLongID(date.Year, numRegRepo.GetNumber(CompanyID, date.Year, Constants.PRODUCTION_ORDERS, true)).ToString();
        }

        public ProductionReport? PrintProductionOrder(pro_ordine Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().PrintProductionOrder(Item);
        }

        public bool StoreDefaultCheck()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().DefaultsCheck(CompanyID);
        }

        public bool CausalDefaultCheck()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().DefaultsCheck(CompanyID);
        }
    }
}
