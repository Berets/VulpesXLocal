using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.SRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Reports.SRM;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class ACQOrderViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public required Guid? CompanyUID { get; set; }

        public ACQOrderViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<acq_orders_heads>? _items;
        public ObservableCollection<acq_orders_heads>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

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
                    return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().GetList(CompanyID, StatusID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }


        public acq_orders_heads? GetHead(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().Get(CompanyID, ID);
        }

        public acq_orders_heads? GetFull(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().GetFull(CompanyID, ID);
        }

        public acq_orders_heads? GetPrintFull(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().GetPrintFull(CompanyID, ID);
        }

        public PurchaseOrderReport? PrintPurchaseOrder(acq_orders_heads Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().PrintPurchaseOrder(Item);
        }

        public string? Validate(acq_orders_heads Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().Validate(Item, false);
        }

        public bool Update(acq_orders_heads Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().Update(Item);
        }
    }
}
