using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting.Assets;
using VulpesX.DAL.Tables.Accounting.Assets;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Accounting.Assets;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Assets
{
    public class ACC_ASSETS_CARDSViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ACC_ASSETS_CARDSViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
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

        public List<GenericIntIDDescription>? Years { get; set; }
        public int? SelectedYear { get; set; }

        private ObservableCollection<ACC_ASSETS_CARDS>? items;
        public ObservableCollection<ACC_ASSETS_CARDS>? Items { get { return items; } set { items = value; NotifyPropertyChanged(); } }

        public async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_CARDSRepository>().GetList(CompanyID, SelectedYear));

            IsBusy = false;
        }


        public ObservableCollection<ACC_ASSETS_DEP_HISTORY>? GetACC_ASSETS_DEP_HISTORYs(ACC_ASSETS_CARDS Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_DEP_HISTORYRepository>().GetList(Item.besoci, Item.beann4, Item.begrup, Item.becont, Item.besotc);
        }

        public ObservableCollection<ACC_ASSETS_DEP_CIV_HISTORY>? GetACC_ASSETS_DEP_CIV_HISTORYs(ACC_ASSETS_CARDS Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_DEP_CIV_HISTORYRepository>().GetList(Item.besoci, Item.beann4, Item.begrup, Item.becont, Item.besotc);
        }

        public ACC_ASSETS_TYPOLOGIES? GetACC_ASSETS_TYPOLOGIES(string GroupID, string AccountID, string SubaccountID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_TYPOLOGIESRepository>().GetIncremented(CompanyID, GroupID, AccountID, SubaccountID);
        }

        public List<GenericIntIDDescription>? GetDistinctYear()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_CARDSRepository>().GetDistinctYears(CompanyID);
        }
    }
}
