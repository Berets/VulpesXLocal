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
using VulpesX.Services.Tables.General;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.General
{
    public class TAB_GEN_TEXTSViewModel : Base
    {
        public required string CompanyID { get; set; }
        public TAB_GEN_TEXTSViewModel()
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

        private ObservableCollection<TAB_GEN_TEXTS>? items;
        public ObservableCollection<TAB_GEN_TEXTS>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public async Task Load(string TypeID)
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_TEXTSRepository>().GetList(CompanyID, TypeID));

            IsBusy = false;

        }

        public bool Delete(TAB_GEN_TEXTS Model)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_TEXTSRepository>().Delete(Model);
        }
    }
}
