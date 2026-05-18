using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Accounting.eInvoice;
using VulpesX.DAL.CRM;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Invoicing
{
    public class AskAccountingWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public AskAccountingWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required ACC_EINVOICE_HEADS Invoice { get; set; }

        public DateTime? SelectedRegDate { get; set; }
        public DateTime? SelectedProtDate { get; set; }
        public ESERCIZIO? AccountingYear { get; set; }
        public CAUCONT? SelectedCausal { get; set; }
        public ObservableCollection<CAUCONT>? Causals { get; set; }
        public TCECO00F? SelectedCostCenter { get; set; }
        public ObservableCollection<TCECO00F>? CostCenters { get; set; }

        public bool IsSupplier { get; set; }
        public bool ShowCostCenter { get; set; }

        public void LoadDetails()
        {
            CostCenters = VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, false);
            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
        }

        public Tuple<string, DateTime>? GetLastProtocolUsed()
        {
            if (SelectedRegDate.HasValue && SelectedCausal != null)
                if (!string.IsNullOrEmpty(SelectedCausal.cauliv))
                    return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().GetLastProtocolUsed(CompanyID, SelectedRegDate.Value, 999999, SelectedCausal.cauliv);

            return null;
        }

        public ObservableCollection<SUPPLIER_GROUPS>? GetSUPPLIER_GROUPSs()
        {
            if (SelectedCausal != null)
                if (Invoice.fattfor.HasValue)
                    return VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>().GetListFull(CompanyID, Invoice.fattfor.Value, SelectedCausal.caucod);

            return null;
        }


        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public ACC_EINVOICE_HEADS? GetFull(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetFull(ID);
        }

        public bool AccountingReceivedInvoice(ObservableCollection<PNRIGHE> Rows)
        {
            if (AccountingYear != null && SelectedRegDate != null && SelectedProtDate != null && SelectedCausal != null)
                return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().AccountingReceivedInvoice(CompanyID, Invoice, AccountingYear, SelectedRegDate.Value, SelectedProtDate.Value, SelectedCausal, Rows.Where(w => w.N1IMEU > 0).ToList(), UserID);

            return false;
        }

        public bool AccountingSentExternalInvoice()
        {
            if (AccountingYear != null && SelectedRegDate != null && SelectedProtDate != null && SelectedCausal != null)
                return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().AccountingSentExternalInvoice(CompanyID, Invoice, AccountingYear, SelectedRegDate.Value, SelectedProtDate.Value, SelectedCausal, UserID);

            return false;
        }
    }
}
