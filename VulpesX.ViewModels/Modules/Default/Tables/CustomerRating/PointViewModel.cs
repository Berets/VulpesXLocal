using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.CustomerRating;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.CustomerRating
{
    public class PointViewModel : Base
    {
        public required string CompanyID { get; set; }
        public PointViewModel()
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

        private ObservableCollection<cr_tab_points_financial>? items;
        public ObservableCollection<cr_tab_points_financial>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<Icr_tab_points_financialRepository>().GetList(CompanyID));

            IsBusy = false;

        }

        public async Task Generate()
        {
            IsBusy = true;

            bool result = false;
            await Task.Run(() =>
                       result = VulpesServiceProvider.Provider.GetRequiredService<Icr_tab_points_financialRepository>().GenerateDefault(CompanyID));

            IsBusy = false;
        }

        public bool Delete(cr_tab_points_financial Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Icr_tab_points_financialRepository>().Delete(Item);
        }
    }
}
