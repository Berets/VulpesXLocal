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
using VulpesX.Models.Models.SRM;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class CostMaterialsViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public required Guid? CompanyUID { get; set; }

        public CostMaterialsViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<tab_articolo_costiListViewModel>? _items;
        public ObservableCollection<tab_articolo_costiListViewModel>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

        private ObservableCollection<tab_articolo_costi>? detailsItems;
        public ObservableCollection<tab_articolo_costi>? DetailsItems { get { return detailsItems; } set { detailsItems = value; NotifyPropertyChanged("DetailsItems"); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_costiRepository>().GetList(CompanyID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadProductCosts(string ProductID)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_costiRepository>().GetDetailsList(CompanyID, ProductID);
                });

                DetailsItems = result;
            }
            finally
            {
                IsBusy = false;
            }
        }


        public bool Delete(tab_articolo_costi Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_costiRepository>().Delete(Item);

        }
    }
}
