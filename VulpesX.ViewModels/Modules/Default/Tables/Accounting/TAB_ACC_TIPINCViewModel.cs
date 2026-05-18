using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public abstract class TAB_ACC_TIPINCViewModel : Base
    {
        public required string CompanyID { get; set; }
        public TAB_ACC_TIPINCViewModel()
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

        private ObservableCollection<TAB_ACC_TIPINC>? items;
        public ObservableCollection<TAB_ACC_TIPINC>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public abstract  Task Load();
    }

    public class TAB_ACC_TIPINCViewModelDefault : TAB_ACC_TIPINCViewModel
    {
        public TAB_ACC_TIPINCViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public override async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().GetList());

            IsBusy = false;

        }
    }

    public class TAB_ACC_TIPINCViewModelUfp : TAB_ACC_TIPINCViewModel
    {
        public TAB_ACC_TIPINCViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public override async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().GetList());

            IsBusy = false;

        }
    }
}
