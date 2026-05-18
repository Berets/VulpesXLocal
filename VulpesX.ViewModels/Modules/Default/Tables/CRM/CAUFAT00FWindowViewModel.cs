using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.CRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.CRM
{
    public class CAUFAT00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public CAUFAT00FWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
            FEDocTypes = VulpesServiceProvider.Provider.GetRequiredService<IFE_TIPODOCRepository>().GetList();

            Numerators = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetDistinctCodeList(CompanyID);
        }
   
        public required CAUFAT00F Data { get; set; }
        public bool IsInsert { get; set; }

        public CAUCONT? SelectedCausal { get; set; }
        public CAUCONT? SelectedSelfInvoiceCausal { get; set; }
        public ObservableCollection<CAUCONT>? Causals { get; set; }
        public NUMREG? SelectedNumerator { get; set; }
        public ObservableCollection<NUMREG>? Numerators { get; set; }
        public FE_TIPODOC? SelectedFEDocType { get; set; }
        public ObservableCollection<FE_TIPODOC>? FEDocTypes { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().Update(Data);
        }
    }
}
