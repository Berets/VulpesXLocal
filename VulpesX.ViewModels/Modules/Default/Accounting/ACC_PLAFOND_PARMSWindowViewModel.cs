using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class ACC_PLAFOND_PARMSWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ACC_PLAFOND_PARMSWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required ACC_PLAFOND_PARMS Data { get; set; }
        public bool IsInsert { get; set; }

        public void LoadDetail()
        {
            Data.ProductsList = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
            Data.RatesList = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            Data.AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            Data.SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            Data.GroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (!IsInsert)
            {
                Data.updatedUserID = UserID;
                if (VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>().Update(Data))
                {
                    return true;
                }
            }
            else
            {
                Data.addedUserID = UserID;
                if (VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>().Insert(Data))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
