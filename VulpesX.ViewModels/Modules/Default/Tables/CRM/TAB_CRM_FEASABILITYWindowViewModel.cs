using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.CRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.CRM;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.CRM
{
    public class TAB_CRM_FEASABILITYWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public TAB_CRM_FEASABILITYWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required TAB_CRM_FEASABILITY Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_FEASABILITYRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_FEASABILITYRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_FEASABILITYRepository>().Update(Data);
        }
    }
}
