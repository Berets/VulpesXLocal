using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class FE_PAGDOCWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public FE_PAGDOCWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            PaymentTypes = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPPAGRepository>().GetList();
        }

        public required FE_PAGDOC Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<TAB_ACC_TIPPAG>? PaymentTypes { get; set; }
        public TAB_ACC_TIPPAG? SelectedPaymentType { get; set; }

        public ObservableCollection<CAUCONT>? GetCAUCONT()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFE_PAGDOCRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (!IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IFE_PAGDOCRepository>().Update(Data);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IFE_PAGDOCRepository>().Insert(Data);
            }
        }
    }
}
