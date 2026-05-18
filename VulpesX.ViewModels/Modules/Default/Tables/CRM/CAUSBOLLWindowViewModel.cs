using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.CRM;
using VulpesX.DAL.Tables.Shipping;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.CRM
{
    public class CAUSBOLLWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public CAUSBOLLWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            InvoiceCausals = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().GetList();

            Numerators = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetDistinctCodeList(CompanyID);
            StoreCausals = VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().GetList(CompanyID);
        }

        public required CAUSBOLL Data { get; set; }
        public bool IsInsert { get; set; }
        public CAUFAT00F? SelectedInvoiceCausal { get; set; }
        public ObservableCollection<CAUFAT00F>? InvoiceCausals { get; set; }
        public store_causals? SelectedStoreCausal { get; set; }
        public NUMREG? SelectedNumerator { get; set; }
        public ObservableCollection<store_causals>? StoreCausals { get; set; }
        public ObservableCollection<NUMREG>? Numerators { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUSBOLLRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ICAUSBOLLRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ICAUSBOLLRepository>().Update(Data);
        }
    }
}
