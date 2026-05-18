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
using VulpesX.DAL.SRM;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class GoodsReceiptViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public required Guid? CompanyUID { get; set; }

        public GoodsReceiptViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;

            Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
        }

        private ObservableCollection<acq_orders_rows>? _items;
        public ObservableCollection<acq_orders_rows>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

        private ObservableCollection<acq_goods_receipts>? itemsHistory;
        public ObservableCollection<acq_goods_receipts>? ItemsHistory { get { return itemsHistory; } set { itemsHistory = value; NotifyPropertyChanged(); } }

        public ObservableCollection<ABE>? Suppliers { get; set; }

        private ABE? selectedSupplier;
        public ABE? SelectedSupplier { get => selectedSupplier; set { selectedSupplier = value; NotifyPropertyChanged(); } }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        private bool _isBusyHistory;
        public bool IsBusyHistory
        {
            get { return _isBusyHistory; }
            set { _isBusyHistory = value; NotifyPropertyChanged(); }
        }

        public async Task LoadGoods()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().GetToReceiptList(CompanyID, SelectedSupplier!.abecod);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadHistory()
        {
            IsBusyHistory = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Iacq_goods_receiptsRepository>().GetList(CompanyID, FromDate!.Value, ToDate!.Value);
                });

                ItemsHistory = result;
            }
            finally
            {
                IsBusyHistory = false;
            }
        }

        public acq_orders_rows? GetFull(long ID, int Sequence)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().GetFull(CompanyID, ID, Sequence);
        }

        public long GetNumerator()
        {
            var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            return numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(CompanyID, now.Year, Constants.GOODS_RECEIPTS, true));
        }

        public ObservableCollection<acq_orders_rows>? GetAcq_Orders_Rows(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().GetList(CompanyID, ID);
        }

        public acq_orders_heads? Get_Acq_Orders_Head(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().Get(CompanyID, ID);
        }

        public bool Update(acq_orders_heads Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().Update(Item);
        }

        public bool Update(acq_orders_rows Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().Update(Item);
        }
    }
}
