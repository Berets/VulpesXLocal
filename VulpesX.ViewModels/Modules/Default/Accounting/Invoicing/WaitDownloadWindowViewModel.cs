using CerberoRetrieveAPI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting.eInvoice;
using VulpesX.Models;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Invoicing
{
    public class WaitDownloadWindowViewModel
    {
        public required string ApiKey { get; set; }
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public WaitDownloadWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public int Insert(SdIItemVulpesX Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().Insert(Item, CompanyID, UserID, "R");
        }
    }
}
