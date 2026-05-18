using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Assets;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Assets;
using VulpesX.DAL.Tables.Productions;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Assets
{
    public class ASSET00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ASSET00FWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required ASSET00F Data { get; set; }
        public bool IsInsert { get; set; }

        public void LoadDetails()
        {
            Data.Locations = VulpesServiceProvider.Provider.GetRequiredService<ITAB_AST_LOCATIONRepository>().GetList(CompanyID);
            Data.Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetList(CompanyID);
            Data.Customers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("C");
            Data.ResourceItems = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetList(CompanyID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IASSET00FRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IASSET00FRepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IASSET00FRepository>().Update(Data);
            }

        }
    }
}
