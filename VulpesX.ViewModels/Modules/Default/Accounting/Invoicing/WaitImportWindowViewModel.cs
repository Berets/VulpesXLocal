using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting.eInvoice;
using VulpesX.Models;
using VulpesX.Models.Models.Accounting.eInvoice;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Invoicing
{
    public class WaitImportWindowViewModel
    {
        public required string Direction { get; set; }
        public required string CompanyID { get; set; }
        public Guid? CompanyGuid { get; set; }
        public required string UserID { get; set; }

        public WaitImportWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyGuid = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.UserName;
        }

        public int InsertLocal(SdIItemGX Item, short AttachementCount)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().InsertLocal(Item, AttachementCount, CompanyID, UserID, Direction);
        }
    }
}
