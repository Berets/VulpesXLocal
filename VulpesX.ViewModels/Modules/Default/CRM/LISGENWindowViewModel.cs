using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Article;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class LISGENWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public LISGENWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required CRM_LISGEN Data { get; set; }
        public bool IsInsert { get; set; }
        public ObservableCollection<GenericIDDescription> ValueTypes => CommonsService.StandardValueTypes;


        public void LoadDetails()
        {
            Data.UMsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);
            Data.Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (!IsInsert)
            {
                Data.updatedUserID = UserID;

                return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>().Update(Data);

            }
            else
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>().Insert(Data);
            }
        }
    }
}
