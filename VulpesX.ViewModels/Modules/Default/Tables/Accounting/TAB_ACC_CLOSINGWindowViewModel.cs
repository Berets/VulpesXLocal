using Azure.Core.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class TAB_ACC_CLOSINGWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public TAB_ACC_CLOSINGWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required TAB_ACC_CLOSING Data { get; set; }
        public bool IsInsert { get; set; }

        public void LoadData()
        {
            var causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetList();
            var groups = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();

            Data.CausalsClosing = causals;
            Data.AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            Data.SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            Data.GroupsClosing = groups;
            Data.CausalsReopen = causals;
            Data.GroupsReopen = groups;
            Data.CausalsLoss = causals;
            Data.GroupsLoss = groups;
            Data.CausalsProfit = causals;
            Data.GroupsProfit = groups;
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_CLOSINGRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_CLOSINGRepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_CLOSINGRepository>().Update(Data);
            }
        }
    }
}
