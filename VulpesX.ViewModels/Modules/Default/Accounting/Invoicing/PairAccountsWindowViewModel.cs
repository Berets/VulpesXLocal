using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Invoicing
{
    public class PairAccountsWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public PairAccountsWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public string? SupplierDescription { get; set; }
        public string? InvoiceText { get; set; }
        public ObservableCollection<ACC_EINVOICE_ROWS> Rows { get; set; } = new ObservableCollection<ACC_EINVOICE_ROWS>();
        public ObservableCollection<PNRIGHE> PNRows { get; set; } = new ObservableCollection<PNRIGHE>();
        public ObservableCollection<ACC_EINVOICE_VAT> PNIVARows { get; set; } = new ObservableCollection<ACC_EINVOICE_VAT>();
        public ObservableCollection<SUPPLIER_GROUPS>? Counterparts { get; set; }
        public ObservableCollection<TCECO00F>? CostCenters { get; set; }
        public List<Tuple<int, int>> Paired { get; set; } = new List<Tuple<int, int>>();
        public string? HeadCostCenterID { get; set; }

        public void LoadDetails()
        {
            CostCenters = VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, false);
        }

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

        public ObservableCollection<FE_IVADOC>? GetFE_IVADOCs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFE_IVADOCRepository>().GetList();
        }

        public ObservableCollection<ASSOGGETAMENTI>? GetASSOGGETAMENTIs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
        }

        public ASSOGGETAMENTI? GetFirstAliquota(string Rate)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetFirstAliquota(Rate);
        }
    }
}
