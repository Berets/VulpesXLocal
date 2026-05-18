using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class TCECO00FViewModel : Base
    {
        public required string CompanyID { get; set; }
        public TCECO00FViewModel()
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

        private ObservableCollection<TCECO00F>? items;
        public ObservableCollection<TCECO00F>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public string? CanDelete(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().CanDelete(ID);
        }

        public bool Delete(TCECO00F Model)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().Delete(Model);
        }

        public async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, false));

            IsBusy = false;

        }
    }
}
