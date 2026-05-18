using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting.Assets;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class ACC_ASSETS_TYPOLOGIESWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public ACC_ASSETS_TYPOLOGIESWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            AllAccounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetSimpleList();
            AllSubccounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(CompanyID);
            Signs = CommonsService.StandardAccountingSignsNone;
        }

        public required ACC_ASSETS_TYPOLOGIES Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<GenericIDDescription>? Signs { get; set; }
        public ObservableCollection<PDCCONTI>? AllAccounts { get; set; }
        public ObservableCollection<PDCSOTTO>? AllSubccounts { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_TYPOLOGIESRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_TYPOLOGIESRepository>().Insert(Data);

            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_TYPOLOGIESRepository>().Update(Data);
            }
        }

        public void LoadDetails()
        {
            Data.Groups = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            Data.AssetCategories = VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_CATEGORIESRepository>().GetList();
        }
    }
}