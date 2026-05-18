using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.CRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.CRM;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.CRM
{
    public class TAB_CRM_FEASABILITYViewModel : Base
    {
        public required string CompanyID { get; set; }
        public TAB_CRM_FEASABILITYViewModel()
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

        private ObservableCollection<TAB_CRM_FEASABILITY>? items;
        public ObservableCollection<TAB_CRM_FEASABILITY>? Items
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
                       Items = VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_FEASABILITYRepository>().GetList());

            IsBusy = false;

        }
    }
}
