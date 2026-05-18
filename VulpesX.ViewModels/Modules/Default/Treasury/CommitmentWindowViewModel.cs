using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Treasury;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Treasury
{
    public class CommitmentWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public CommitmentWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required TES_IMFI Data { get; set; }
        public bool IsInsert { get; set; }

        public string? LastGroupID;
        public string? LastAccountID;
        public string? LastSubaccountID;
        public DateTime? LastDate;
        public string? LastReference;

        public void LoadDetails()
        {
            Data.AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            Data.SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            Data.GroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITES_IMFIRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ITES_IMFIRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ITES_IMFIRepository>().Update(Data);
        }
    }
}
