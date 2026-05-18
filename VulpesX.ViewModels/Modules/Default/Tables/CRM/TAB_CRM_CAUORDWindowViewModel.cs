using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.CRM;
using VulpesX.DAL.Tables.Shipping;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.CRM;
using VulpesX.Services.Tables.General;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.CRM
{
    public class TAB_CRM_CAUORDWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public TAB_CRM_CAUORDWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUSBOLLRepository>().GetList("C");
            InvoiceCausals = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().GetList();
            Texts = VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_TEXTSRepository>().GetList(CompanyID, TAB_GEN_TEXTS.ORDERS);
        }

        public required TAB_CRM_CAUORD Data { get; set; }
        public bool IsInsert { get; set; }
        public CAUSBOLL? SelectedCausalDDT { get; set; }
        public ObservableCollection<CAUSBOLL>? Causals { get; set; }
        public CAUFAT00F? SelectedCausalInvoice { get; set; }
        public ObservableCollection<CAUFAT00F>? InvoiceCausals { get; set; }
        public ObservableCollection<TAB_GEN_TEXTS>? Texts { get; set; }
        public TAB_GEN_TEXTS? SelectedText { get; set; }

        public ObservableCollection<PDCGRUPPI>? GetPDCGRUPPIs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public List<PDCCONTI>? GetPDCCONTIs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
        }

        public List<PDCSOTTO>? GetPDCSOTTOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_CAUORDRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_CAUORDRepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_CAUORDRepository>().Update(Data);
            }
        }
    }
}
