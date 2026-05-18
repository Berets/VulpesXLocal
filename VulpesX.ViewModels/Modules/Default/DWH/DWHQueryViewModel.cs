using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.DWH;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.DWH
{
    public class DWHQueryViewModel : Base
    {
        public required string CompanyID { get; set; }
        public DWHQueryViewModel()
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

        private ObservableCollection<DWH_Query>? items;
        public ObservableCollection<DWH_Query>? Items
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
                       Items = VulpesServiceProvider.Provider.GetRequiredService<Idwh_queryRepository>().GetList(CompanyID));

            IsBusy = false;

        }

        public bool Delete(DWH_Query Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Idwh_queryRepository>().Delete(Item);
        }

        public DWH_Query? GetQuery(Guid QueryID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Idwh_queryRepository>().Get(CompanyID, QueryID);
        }
    }
}
