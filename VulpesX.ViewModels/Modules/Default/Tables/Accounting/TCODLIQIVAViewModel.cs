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
    public abstract class TCODLIQIVAViewModel : Base
    {
        public required string CompanyID { get; set; }
        public TCODLIQIVAViewModel()
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

        private ObservableCollection<TCODLIQIVA>? items;
        public ObservableCollection<TCODLIQIVA>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public abstract Task Load();
    }

    public class TCODLIQIVAViewModelDefault : TCODLIQIVAViewModel
    {
        public TCODLIQIVAViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public override async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().GetList(CompanyID));

            IsBusy = false;

        }
    }

    public class TCODLIQIVAViewModelUfp : TCODLIQIVAViewModel
    {
        public TCODLIQIVAViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public override async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().GetList(CompanyID));

            IsBusy = false;

        }

    }
}
