using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.General
{
    public class TAB_GEN_ACTIVITY_TYPESViewModel : Base
    {
        public required string CompanyID { get; set; }
        public TAB_GEN_ACTIVITY_TYPESViewModel()
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

        private ObservableCollection<TAB_GEN_ACTIVITY_TYPES>? items;
        public ObservableCollection<TAB_GEN_ACTIVITY_TYPES>? Items
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
                       Items = VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_ACTIVITY_TYPESRepository>().GetList(CompanyID));

            IsBusy = false;

        }
    }
}
