using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting.Assets;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class ACC_ASSETS_TYPESViewModel : Base
    {
        public required string CompanyID { get; set; }
        public ACC_ASSETS_TYPESViewModel()
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

        private ObservableCollection<ACC_ASSETS_TYPES>? items;
        public ObservableCollection<ACC_ASSETS_TYPES>? Items { get { return items; } set { items = value; NotifyPropertyChanged(); } }

        public async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_TYPESRepository>().GetList(CompanyID));

            IsBusy = false;
        }
    }
}
