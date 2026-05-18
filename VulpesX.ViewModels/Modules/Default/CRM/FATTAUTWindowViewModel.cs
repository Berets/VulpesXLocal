using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class FATTAUTWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public FATTAUTWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required FATTAUT Data { get; set; }
        public bool IsInsert { get; set; }

        public ABE? SelectedSupplier { get; set; }
        public ObservableCollection<ABE>? Suppliers { get; set; }

        public void LoadDetails()
        {
            Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTAUTRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IFATTAUTRepository>().Insert(Data);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IFATTAUTRepository>().Update(Data);
            }
        }
    }
}
