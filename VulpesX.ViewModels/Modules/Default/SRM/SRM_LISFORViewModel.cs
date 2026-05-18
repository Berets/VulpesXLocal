using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.SRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class SRM_LISFORViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public SRM_LISFORViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<SRM_LISFOR>? _items;
        public ObservableCollection<SRM_LISFOR>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

        public ObservableCollection<ABE>? Suppliers { get; set; }

        private ABE? selectedSupplier;
        public ABE? SelectedSupplier { get => selectedSupplier; set { selectedSupplier = value; NotifyPropertyChanged("SelectedSupplier"); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        public void LoadDetails()
        {
            Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
        }

        public async Task Load(string StatusID)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<ISRM_LISFORRepository>().GetList(CompanyID, SelectedSupplier?.abecod, StatusID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool Update(SRM_LISFOR Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISRM_LISFORRepository>().Update(Item);
        }

        public bool CopyFrom(int Source, int Target)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISRM_LISFORRepository>().CopyFrom(CompanyID, Source, Target, UserID);
        }
    }
}
