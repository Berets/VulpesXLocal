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
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Reports
{
    public class BalanceSimulationWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public BalanceSimulationWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public int? AccountingYear { get; set; }
        public DateTime? PrintUntil { get; set; }
        public bool IncludeTemp { get; set; }
        public ObservableCollection<TCECO00F>? CostCenters { get; set; }

        private bool splitMonth;
        public bool SplitMonth
        {
            get { return splitMonth; }
            set
            {
                splitMonth = value;
                NotifyPropertyChanged();
            }
        }

        private bool splitMonthEnabled = true;
        public bool SplitMonthEnabled
        {
            get { return splitMonthEnabled; }
            set
            {
                splitMonthEnabled = value;
                NotifyPropertyChanged();
            }
        }

        private bool groupDiva;
        public bool GroupDiva
        {
            get { return groupDiva; }
            set
            {
                groupDiva = value;

                if (groupDiva)
                {
                    SplitMonth = false;
                    NotifyPropertyChanged("SplitMonth");

                    SplitMonthEnabled = false;
                    NotifyPropertyChanged("SplitMonthEnabled");
                }
                else
                {
                    SplitMonthEnabled = true;
                    NotifyPropertyChanged("SplitMonthEnabled");
                }

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public ObservableCollection<TCECO00F>? GetTCECO00Fs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, true);
        }

        public PDCBalanceReportOpposed? GetPDCBalanceReportOpposed(string? CostCenter)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().PrintPDCBalanceOpposed(CompanyID, AccountingYear!.Value, PrintUntil!.Value, IncludeTemp, CostCenter);
        }


        public PDCBalanceReportOpposed? GetPDCBalanceReportOpposedDIVA(string? CostCenter)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().PrintPDCBalanceOpposedDIVA(CompanyID, AccountingYear!.Value, PrintUntil!.Value, IncludeTemp, CostCenter);
        }

        public List<Tuple<int, PDCBalanceReportOpposed>>? GetPDCBalanceOpposedSplitted(string? CostCenter)
        {
            var esercizio = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, AccountingYear!.Value);

            if (esercizio == null || !esercizio.eseini.HasValue)
                return null;

            var monthStart = esercizio.eseini!.Value;

            var monthList = new List<Tuple<int, PDCBalanceReportOpposed>>();

            while (monthStart <= PrintUntil!.Value.Month)
            {
                var monthEnd = new DateTime(AccountingYear!.Value, monthStart, DateTime.DaysInMonth(AccountingYear!.Value, monthStart));

                var monthData = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().PrintPDCBalanceOpposed(CompanyID, AccountingYear!.Value, monthEnd, IncludeTemp, CostCenter);

                if (monthData != null)
                    monthList.Add(new Tuple<int, PDCBalanceReportOpposed>(monthStart, monthData));

                monthStart++;
            }

            return monthList;
        }
    }
}
