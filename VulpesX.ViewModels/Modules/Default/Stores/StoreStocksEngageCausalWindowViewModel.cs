using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Store;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Stores
{
    public class StoreStocksEngageCausalWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public StoreStocksEngageCausalWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required store_stocks_engage Engage { get; set; }

        public ObservableCollection<store_causals>? UnloadCausals { get; set; }

        public ObservableCollection<store_causals>? GetStore_Causals()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().GetList(CompanyID, "-");
        }

        public bool Unload(store_causals Causal)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().Unload(Engage, Causal, CompanyID, UserID);
        }
    }
}
