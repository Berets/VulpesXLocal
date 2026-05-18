using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting.eInvoice;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Models.Accounting;
using VulpesX.Models.Ufp;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Ufp.Accounting.Invoicing
{
    public class CheckInvoiceEntranceWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public CheckInvoiceEntranceWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public required string InvoiceID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public required string VATNumber { get; set; }
        public int SupplierID { get; set; }


        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBusyEntrances;
        public bool IsBusyEntrances
        {
            get { return _isBusyEntrances; }
            set
            {
                _isBusyEntrances = value;
                NotifyPropertyChanged();
            }
        }

        private List<CheckInvoiceEntranceModel.DettaglioModel>? items;
        public List<CheckInvoiceEntranceModel.DettaglioModel>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        private List<CheckInvoiceEntranceModel.EntrataModel>? entranceLinked;
        public List<CheckInvoiceEntranceModel.EntrataModel>? EntranceLinked
        {
            get => entranceLinked; set
            {
                entranceLinked = value;
                NotifyPropertyChanged();
            }
        }

        private List<CheckInvoiceEntranceModel.EntrataModel>? entrances;
        public List<CheckInvoiceEntranceModel.EntrataModel>? Entrances
        {
            get => entrances; set
            {
                entrances = value;
                NotifyPropertyChanged();
            }
        }

        public async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                var data = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWSRepository>().GetRows(CompanyID, SupplierID, InvoiceID, InvoiceDate, VATNumber);

                Items = data.Item1;
                EntranceLinked = data.Item2;
            });
            IsBusy = false;
        }

        public async Task LoadEntrance(string DDTID)
        {
            IsBusyEntrances = true;

            await Task.Run(() =>
            {
                var data = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWSRepository>().GetEntrances(CompanyID, SupplierID, DDTID, EntranceLinked ?? new List<CheckInvoiceEntranceModel.EntrataModel>());

                Entrances = data;
            });
            IsBusyEntrances = false;
        }

        public async Task<bool> UpdateEntrance(CheckInvoiceEntranceModel.DettaglioModel Row, CheckInvoiceEntranceModel.EntrataModel Entrance)
        {
            IsBusyEntrances = true;

            bool result = false;
            await Task.Run(() =>
            {
                result = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWSRepository>().UpdateEntrance(Entrance,Row);
            });

            IsBusyEntrances = false;

            return result;
        }
    }
}
