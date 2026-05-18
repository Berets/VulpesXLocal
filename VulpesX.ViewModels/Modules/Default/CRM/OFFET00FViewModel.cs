using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.SRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Reports.CRM;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class OFFET00FViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public required Guid? CompanyUID { get; set; }

        public OFFET00FViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<OFFET00F>? _items;
        public ObservableCollection<OFFET00F>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        public async Task Load(int Year, string StatusID)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IOFFET00FRepository>().GetList(CompanyID, Year, StatusID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public OFFET00F? GetFull(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IOFFET00FRepository>().GetFull(CompanyID, Year, ID);
        }

        public OFFET00F? GetPrintFull(int Year, int ID)
        {
            var azienda = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

            if (azienda == null)
                return null;

            return VulpesServiceProvider.Provider.GetRequiredService<IOFFET00FRepository>().GetPrintFull(CompanyID, Year, ID, azienda.AZCUSOFF, azienda.azpnotoff, azienda.azagedoff);
        }

        public OfferReport? PrintOffer(OFFET00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IOFFET00FRepository>().PrintOffer(Item);
        }

        public bool Update(OFFET00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IOFFET00FRepository>().Update(Item);
        }

        public bool CloseOrders(List<OFFET00F> Items, string CausalID, string Note)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IOFFET00FRepository>().CloseOrders(Items, CausalID, Note, UserID);
        }

    }
}
