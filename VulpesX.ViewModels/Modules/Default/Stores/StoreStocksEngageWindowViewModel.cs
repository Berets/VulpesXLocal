using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Stores
{
    public class StoreStocksEngageWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public StoreStocksEngageWindowViewModel()
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

        public required store_stocks_engage Data { get; set; }
        public bool IsInsert { get; set; }
        public bool IsUnloading { get; set; } = false;
        public bool EngagedEnabled => !IsUnloading;
        public ObservableCollection<store_stocks_lots>? Lots { get; set; }
        public tab_articolo? Product { get; set; }

        public tab_articolo? GetTab_Articolo()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(CompanyID, Data.product_id);
        }

        public ObservableCollection<store_stocks_lots>? GetStore_Stocks_Lots()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_lotsRepository>().GetList(CompanyID, Data.store_id, Data.product_id, true);
        }

        public string? Validate(decimal OlderQuantity)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().Validate(Data, IsInsert, IsUnloading, OlderQuantity);
        }

        public bool Update()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().Update(Data);
        }

        public bool Insert()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().Insert(Data);
        }
    }
}
