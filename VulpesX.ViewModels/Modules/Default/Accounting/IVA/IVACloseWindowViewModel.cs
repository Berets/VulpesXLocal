using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
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
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;

namespace VulpesX.ViewModels.Modules.Default.Accounting.IVA
{
    public abstract class IVACloseWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public IVACloseWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public DateTime? PrintSince { get; set; }
        public DateTime? PrintUntil { get; set; }
        public bool IsDefinitive { get; set; }
        public ESERCIZIO? FiscalYear { get; set; }
        public int? AccountingYear { get; set; }
        public decimal PreviousAmount { get; set; } = 0;
        public decimal CompensationAmount { get; set; } = 0;

        public abstract ObservableCollection<ESERCIZIO>? GetESERCIZIOs();

        public abstract ESERCIZIO? GetESERCIZIO(int Year);

        public abstract LIBRIIVA? GetDefaultIVARecap();

        public abstract IVAReport? GetIVAReport(LIBRIIVA Default, DateTime Now);

        public abstract IVAReport? GetIVAReport(DateTime Now);

        public abstract IVAReportDetails? GetIVAReportDetails();

        public abstract bool UpdateLiquidationDefinitive(LIBRIIVA Default, IVAReport ReportData, ReportingHandler.ReportResult ReportResult);

        public abstract bool UpdateLiquidationDefinitive(IVAReport ReportData, ReportingHandler.ReportResult ReportResult);

        public abstract IVAReportYearly? GetIVAReportYearly();
    }

    public class IVACloseWindowViewModelDefault : IVACloseWindowViewModel
    {
        public IVACloseWindowViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public override ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public override ESERCIZIO? GetESERCIZIO(int Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetList(CompanyID)?.Where(w => w.eseann == Year).FirstOrDefault();
        }

        public override LIBRIIVA? GetDefaultIVARecap()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().GetDefaultIVARecap();
        }

        public override IVAReport? GetIVAReport(LIBRIIVA Default, DateTime Now)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().PrintIVARecap(CompanyID, PrintSince!.Value, PrintUntil!.Value, FiscalYear!.eseivavenBool, IsDefinitive, Default, Now);
        }

        public override IVAReport? GetIVAReport(DateTime Now)
        {
            throw new NotImplementedException();
        }

        public override IVAReportDetails? GetIVAReportDetails()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().PrintIVARecapDetails(CompanyID, PrintSince!.Value, PrintUntil!.Value, FiscalYear!.eseivavenBool, IsDefinitive);
        }

        public override bool UpdateLiquidationDefinitive(LIBRIIVA Default, IVAReport ReportData, ReportingHandler.ReportResult ReportResult)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().UpdateLiquidationDefinitive(CompanyID, $"R{Default.livcod.Trim()}", PrintUntil!.Value.Year, ReportData.StartingPage + ReportResult.PrintedPages, UserID, PrintUntil!.Value, ReportData.Total, ReportData.CompensationAmount);
        }

        public override bool UpdateLiquidationDefinitive(IVAReport ReportData, ReportingHandler.ReportResult ReportResult)
        {
            throw new NotImplementedException();
        }

        public override IVAReportYearly? GetIVAReportYearly()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().PrintIVARecapYearly(CompanyID, AccountingYear!.Value);
        }
    }

    public class IVACloseWindowViewModelUfp : IVACloseWindowViewModel
    {
        public IVACloseWindowViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public override ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetList(CompanyID);
        }

        public override ESERCIZIO? GetESERCIZIO(int Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetList(CompanyID)?.Where(w => w.eseann == Year).FirstOrDefault();
        }

        public override LIBRIIVA? GetDefaultIVARecap()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().GetDefaultIVARecap();
        }

        public override IVAReport? GetIVAReport(LIBRIIVA Default, DateTime Now)
        {
            throw new NotImplementedException();
        }

        public override IVAReport? GetIVAReport(DateTime Now)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().PrintIVARecap(CompanyID, PrintSince!.Value, PrintUntil!.Value, FiscalYear!.eseivavenBool, IsDefinitive, Now);
        }

        public override IVAReportDetails? GetIVAReportDetails()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().PrintIVARecapDetails(CompanyID, PrintSince!.Value, PrintUntil!.Value, FiscalYear!.eseivavenBool, IsDefinitive);
        }

        public override bool UpdateLiquidationDefinitive(LIBRIIVA Default, IVAReport ReportData, ReportingHandler.ReportResult ReportResult)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateLiquidationDefinitive(IVAReport ReportData, ReportingHandler.ReportResult ReportResult)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().UpdateLiquidationDefinitive(CompanyID, "RRI", PrintUntil!.Value.Year, ReportData.StartingPage + ReportResult.PrintedPages, UserID, PrintUntil!.Value, ReportData.Total,ReportData.CompensationAmount);
        }

        public override IVAReportYearly? GetIVAReportYearly()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().PrintIVARecapYearly(CompanyID, AccountingYear!.Value);
        }
    }

}
