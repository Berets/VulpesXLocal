using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class LISGENViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public required Guid? CompanyUID { get; set; }

        public LISGENViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<CRM_LISGEN>? items;
        public ObservableCollection<CRM_LISGEN>? Items { get { return items; } set { items = value; NotifyPropertyChanged("Items"); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        public async Task Load(string StatusID)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>().GetList(CompanyID, StatusID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool Update(CRM_LISGEN Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>().Update(Item);
        }

        public bool CopyFrom(string Source, string Target)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>().CopyFrom(CompanyID, Source, Target, UserID);
        }
    }
}
