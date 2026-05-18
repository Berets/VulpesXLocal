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
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Production
{
    public class ProductionOrderConfirmHalfWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ProductionOrderConfirmHalfWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required pro_ordine Order { get; set; }
        public ObservableCollection<pro_ordine_composizione> MaterialList { get; set; } = new ObservableCollection<pro_ordine_composizione>();
        public ObservableCollection<pro_ordine_composizione> HalfworkedList { get; set; } = new ObservableCollection<pro_ordine_composizione>();

        public ObservableCollection<tab_articolo_composizione>? GetComponentsByProduct(string ProductID, string RevisionID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetComponentsByProduct(CompanyID, ProductID, RevisionID);
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
    }
}
