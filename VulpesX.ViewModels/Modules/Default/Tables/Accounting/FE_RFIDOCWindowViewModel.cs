using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class FE_RFIDOCWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public FE_RFIDOCWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required FE_RFIDOC Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFE_RFIDOCRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (!IsInsert)
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IFE_RFIDOCRepository>().Update(Data);
            }
            else
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IFE_RFIDOCRepository>().Insert(Data);

            }
        }
    }
}
