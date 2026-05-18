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

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class LISCLICopyCustomerWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public LISCLICopyCustomerWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public ObservableCollection<ABE>? Customers { get; set; }
        public ABE? SelectedSourceCustomer { get; set; }
        public ABE? SelectedTargetCustomer { get; set; }

        public void LoadDetails()
        {
            Customers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("C");
        }
    }
}
