using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.SRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class RDAWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public RDAWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required SRM_RDA Data { get; set; }
        public bool CanApprove { get; set; }
        public bool IsInsert { get; set; }
        public bool IsReadonly => (Data.approval_date.HasValue && !CanApprove) || Data.canceled.HasValue;
        public bool IsEnabled => !IsReadonly;
        public ObservableCollection<tab_articolo>? Products { get; set; }

        public void LoadDetails()
        {
            Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
            Data.Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
            CanApprove = UserContext.Instance!.ACCESS!.Roles?.canApproveRDA ?? false;
        }

        public int? GetDefaultSupplier(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetDefaultSupplier(CompanyID, ProductID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISRM_RDARepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if(IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ISRM_RDARepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ISRM_RDARepository>().Update(Data);
            }
        }
    }
}
