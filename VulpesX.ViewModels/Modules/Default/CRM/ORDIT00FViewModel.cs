using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Production;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Reports.CRM;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class ORDIT00FViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public required Guid? CompanyUID { get; set; }

        public ORDIT00FViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<ORDIT00F>? _items;
        public ObservableCollection<ORDIT00F>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }


        private ObservableCollection<ORDID00F>? detailsItems;
        public ObservableCollection<ORDID00F>? DetailsItems { get { return detailsItems; } set { detailsItems = value; NotifyPropertyChanged("DetailsItems"); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        private bool _isBusyDetails;
        public bool IsBusyDetails
        {
            get { return _isBusyDetails; }
            set { _isBusyDetails = value; NotifyPropertyChanged(); }
        }

        public async Task Load(int Year, string StatusID)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().GetList(CompanyID, Year, StatusID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadDetails()
        {
            IsBusyDetails = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().GetFulfillmentList(CompanyID);
                });

                DetailsItems = result;
            }
            finally
            {
                IsBusyDetails = false;
            }
        }

        public ORDIT00F? Get(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().Get(CompanyID, Year, ID);
        }

        public ORDIT00F? GetFull(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().GetFull(CompanyID, Year, ID);
        }

        public bool Update(ORDIT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().Update(Item);
        }

        public ORDIT00F? GetPrintFull(int Year, int ID, bool IsGetInfoFromCompany)
        {
            if (IsGetInfoFromCompany)
            {
                var azienda = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

                if (azienda == null)
                    return null;

                return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().GetPrintFull(CompanyID, Year, ID, azienda.AZCUSORD, azienda.azpnotord, azienda.azagedord);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().GetPrintFull(CompanyID, Year, ID, false, false, false);
            }
        }

        public OrderReport? PrintOrder(ORDIT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().PrintOrder(Item);
        }

        public bool GenerateProductionOrder(ORDIT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().GenerateProductionOrder(Item, UserID, null, null, true);
        }

        public ObservableCollection<pro_ordine>? GetPro_Ordines(string CompanyID, int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().GetListByOrder(CompanyID, Year, ID);
        }
    }
}
