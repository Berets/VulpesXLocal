using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting.Assets;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Accounting.Assets;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Assets
{
    public class ACC_ASSETS_CARDSWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ACC_ASSETS_CARDSWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required ACC_ASSETS_CARDS Data { get; set; }
        public bool IsInsert { get; set; }
        public ObservableCollection<GenericIDDescription> Suspensions => CommonsService.AccountingAssetsSuspensions;
        public ObservableCollection<GenericIDDescription> BaseSigns => CommonsService.BaseSigns;

        public void LoadDetails()
        {
            Data.TypesList = VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_TYPESRepository>().GetList(CompanyID);
            Data.CostCentersList = VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, false);
            Data.CategoriesList = VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_CATEGORIESRepository>().GetList();
            Data.Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
            Data.Branches = VulpesServiceProvider.Provider.GetRequiredService<IFILIALIRepository>().GetList(CompanyID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_CARDSRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if(IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_CARDSRepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_CARDSRepository>().Update(Data);
            }
        }
    }
}
