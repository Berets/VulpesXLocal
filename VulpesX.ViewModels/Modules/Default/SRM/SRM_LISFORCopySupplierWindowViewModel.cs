using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class SRM_LISFORCopySupplierWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public SRM_LISFORCopySupplierWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public ObservableCollection<ABE>? Suppliers { get; set; }
        public ABE? SelectedSourceSupplier { get; set; }
        public ABE? SelectedTargetSupplier { get; set; }

        public void LoadDetails()
        {
            Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
        }
    }
}
