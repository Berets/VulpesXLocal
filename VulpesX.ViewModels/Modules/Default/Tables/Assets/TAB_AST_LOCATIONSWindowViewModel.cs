using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Assets;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Assets
{
    public class TAB_AST_LOCATIONSWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public TAB_AST_LOCATIONSWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required TAB_AST_LOCATIONS Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_AST_LOCATIONRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_AST_LOCATIONRepository>().Insert(Data);

            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_AST_LOCATIONRepository>().Update(Data);
            }
        }

        public void LoadDetails()
        {
            Data.Cities = VulpesServiceProvider.Provider.GetRequiredService<ICOMUNIRepository>().GetList();
            Data.States = VulpesServiceProvider.Provider.GetRequiredService<ITAB_STATESRepository>().GetList();
        }
    }
}
