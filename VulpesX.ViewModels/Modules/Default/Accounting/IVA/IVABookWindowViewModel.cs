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
    public class IVABookWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public IVABookWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }


        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }


        public int? AccountingYear { get; set; }
        public LIBRIIVA? SelectedIVABook { get; set; }
        public DateTime? PrintSince { get; set; }
        public DateTime? PrintUntil { get; set; }
        public bool IsDefinitive { get; set; }
        public ObservableCollection<LIBRIIVA>? IVABooks { get; set; }

        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public ObservableCollection<LIBRIIVA>? GetLIBRIIVAs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().GetList();
        }

        public IVABookReport? GetIVABookReport()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().PrintIVABook(CompanyID, AccountingYear!.Value, SelectedIVABook!, PrintSince!.Value, PrintUntil!.Value, IsDefinitive);
        }

        public async Task<bool> UpdatePrintedDefinitives(IVABookReport ReportData, ReportingHandler.ReportResult ReportResult)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().UpdatePrintedDefinitives(CompanyID, SelectedIVABook!.livcod, $"R{SelectedIVABook.livcod.Trim()}", PrintUntil!.Value.Year, ReportData.Rows ?? new List<PNIVA>(), ReportData.StartingPage + ReportResult.PrintedPages, UserID, PrintUntil.Value);
                });

                IsBusy = false;

                return result;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
