using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting.Assets;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class ACC_ASSETS_TYPOLOGIESViewModel : Base
    {
        public required string CompanyID { get; set; }
        public ACC_ASSETS_TYPOLOGIESViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

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

        private ObservableCollection<PDCSOTTO>? items;
        public ObservableCollection<PDCSOTTO>? Items { get { return items; } set { items = value; NotifyPropertyChanged(); } }

        private ObservableCollection<ACC_ASSETS_TYPOLOGIES>? itemsData;
        public ObservableCollection<ACC_ASSETS_TYPOLOGIES>? ItemsData { get { return itemsData; } set { itemsData = value; NotifyPropertyChanged(); } }

        public async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_TYPOLOGIESRepository>().GetPDCListWithTypology(CompanyID));

            IsBusy = false;
        }

        public async Task LoadDetails(PDCSOTTO Subaccount)
        {
            IsBusy = true;

            if (Subaccount.Group != null && Subaccount.Account != null)
            {
                await Task.Run(() =>
                           ItemsData = VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_TYPOLOGIESRepository>().GetList(CompanyID, Subaccount.Group.P1GRUP, Subaccount.Account.P2CONT, Subaccount.P3SOTC));
            }

            IsBusy = false;
        }

        public List<PDCSOTTO>? GetPDCSOTTOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
        }

        public List<PDCCONTI>? GetPDCCONTIs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
        }

        public bool Delete(ACC_ASSETS_TYPOLOGIES Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_TYPOLOGIESRepository>().Delete(Item);
        }
    }
}
