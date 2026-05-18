using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Treasury;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Treasury
{
    public class CommitmentViewModel : Base
    {
        public required string CompanyID { get; set; }
        public CommitmentViewModel()
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

        private ObservableCollection<TES_IMFI>? items;
        public ObservableCollection<TES_IMFI>? Items { get => items; set { items = value; NotifyPropertyChanged("Items"); } }

        private ObservableCollection<TES_IMFI>? itemsNear;

        public ObservableCollection<TES_IMFI>? ItemsNear { get => itemsNear; set { itemsNear = value; NotifyPropertyChanged("ItemsNear"); } }

        public async Task Load()
        {
            IsBusy = true;

            var result = await Task.Run(() =>
            {
                var items = VulpesServiceProvider.Provider.GetRequiredService<ITES_IMFIRepository>().GetList(CompanyID);
                var trend = VulpesServiceProvider.Provider.GetRequiredService<ITES_IMFIRepository>().GetListNear(CompanyID);

                return new { items, trend };
            });

            Items = result.items;
            ItemsNear = result.trend;

            IsBusy = false;

        }

        public TES_IMFI? Get(string GroupID, string AccountID, string SubaccountID, DateTime Date)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITES_IMFIRepository>().Get(CompanyID, GroupID, AccountID, SubaccountID, Date);
        }

        public bool Delete(TES_IMFI Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITES_IMFIRepository>().Delete(Item);
        }
    }
}
