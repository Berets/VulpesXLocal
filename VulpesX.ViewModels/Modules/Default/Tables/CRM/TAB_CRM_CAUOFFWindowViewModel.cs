using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.CRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.CRM;
using VulpesX.Services.Tables.General;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.CRM
{
    public class TAB_CRM_CAUOFFWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public TAB_CRM_CAUOFFWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            OrderCausals = VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_CAUORDRepository>().GetList(CompanyID);
            Texts = VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_TEXTSRepository>().GetList(CompanyID, TAB_GEN_TEXTS.OFFERS);
        }

        public required TAB_CRM_CAUOFF Data { get; set; }
        public bool IsInsert { get; set; }

        public TAB_CRM_CAUORD? SelectedOrderCausal { get; set; }
        public ObservableCollection<TAB_CRM_CAUORD>? OrderCausals { get; set; }
        public ObservableCollection<TAB_GEN_TEXTS>? Texts { get; set; }
        public TAB_GEN_TEXTS? SelectedText { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_CAUOFFRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_CAUOFFRepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_CAUOFFRepository>().Update(Data);
            }
        }
    }
}
