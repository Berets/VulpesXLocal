using DocumentFormat.OpenXml.EMMA;
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
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Reports
{
    public class GeneralJournalWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public GeneralJournalWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public int? AccountingYear { get; set; }
        public DateTime? PrintUntil { get; set; }
        public bool IsDefinitive { get; set; }

        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public GeneralJournalReport? GetGeneralJournalReport()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().PrintGeneralJournal(CompanyID, AccountingYear!.Value, PrintUntil!.Value);
        }

        public bool UpdateJournalDefinitives(GeneralJournalReport ReportData, int LastPage)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().UpdateJournalDefinitives(ReportData, LastPage, CompanyID, UserID);
        }
    }
}
