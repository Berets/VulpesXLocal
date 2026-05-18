using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.IVA;

namespace VulpesX.Modules.Default.Accounting.IVA
{
    /// <summary>
    /// Interaction logic for IVACloseWindow.xaml
    /// </summary>
    public partial class IVACloseWindow : FluentDefaultWindow
    {
        private IVACloseWindowViewModel _dataContext;
        public IVACloseWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<IVACloseWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
            _dataContext.IsDefinitive = false;
            _dataContext.PrintSince = new DateTime(now.Year, now.Month, 1);
            _dataContext.PrintUntil = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
            _dataContext.AccountingYear = (cmbAccountingYear.SelectedItem as ESERCIZIO)?.eseann;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (rtiMonthly.IsSelected)
            {
                if (_dataContext.PrintSince.HasValue && _dataContext.PrintUntil.HasValue)
                {
                    _dataContext.FiscalYear = _dataContext.GetESERCIZIO(_dataContext.PrintSince.Value.Year);

                    if (_dataContext.FiscalYear != null)
                    {
                        if ((_dataContext.FiscalYear.eseivavenBool && _dataContext.PrintSince.Value >= _dataContext.FiscalYear.esedtiniva && _dataContext.PrintUntil.Value <= _dataContext.FiscalYear.esedtfniva) || !_dataContext.FiscalYear.eseivavenBool)
                        {
                            if (ConfirmHandler.Confirm($"Confermate la stampa {(_dataContext.IsDefinitive ? "DEFINITIVA" : "IN SIMULAZIONE")} della liquidazione IVA ?"))
                            {
                                Mouse.OverrideCursor = Cursors.Wait;
                                var defaultIVABookNumerator = _dataContext.GetDefaultIVARecap();
                                if (defaultIVABookNumerator != null)
                                {
                                    var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                                    var reportData = _dataContext.GetIVAReport(defaultIVABookNumerator, now);

                                    if (reportData != null)
                                    {
                                        reportData.PreviousAmount = _dataContext.PreviousAmount;

                                        var result = ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_IVARECAP, _dataContext.CompanyID, reportData, $"{reportData.TemporaryText} dal {_dataContext.PrintSince.Value.Date} al {_dataContext.PrintUntil.Value.Date}", $"{reportData.TemporaryText}_{_dataContext.PrintSince.Value.Date.ToString("dd_MM_yyyy")}_{_dataContext.PrintUntil.Value.Date.ToString("dd_MM_yyyy")}.pdf", true);

                                        if (_dataContext.FiscalYear.eseivavenBool && ConfirmHandler.Confirm("Stampare anche i dettagli delle fatture ?"))
                                        {
                                            // print details
                                            var reportDataDetails = _dataContext.GetIVAReportDetails();

                                            if (reportDataDetails != null)
                                            {
                                                // subreports
                                                var subReports = new List<ReportingHandler.SubReportInfo>() {
                                                    new ReportingHandler.SubReportInfo(){
                                                        Name = Constants.REPORT_TYPE_ACCOUNTING_IVARECAP_DETAILS_SUBIVA,
                                                        InternalName = "SubSalesPrevious",
                                                        Datasource = reportDataDetails.SalesPrevious } ,
                                                    new ReportingHandler.SubReportInfo(){
                                                        Name = Constants.REPORT_TYPE_ACCOUNTING_IVARECAP_DETAILS_SUBIVA,
                                                        InternalName = "SubSalesCurrent",
                                                        Datasource = reportDataDetails.SalesCurrent },
                                                    new ReportingHandler.SubReportInfo(){
                                                        Name = Constants.REPORT_TYPE_ACCOUNTING_IVARECAP_DETAILS_SUBIVA,
                                                        InternalName = "SubSalesPreviousPaid",
                                                        Datasource = reportDataDetails.SalesPreviousPaid } ,
                                                    new ReportingHandler.SubReportInfo(){
                                                        Name = Constants.REPORT_TYPE_ACCOUNTING_IVARECAP_DETAILS_SUBIVA,
                                                        InternalName = "SubSalesCurrentPaid",
                                                        Datasource = reportDataDetails.SalesCurrentPaid },
                                                    new ReportingHandler.SubReportInfo(){
                                                        Name = Constants.REPORT_TYPE_ACCOUNTING_IVARECAP_DETAILS_SUBIVA,
                                                        InternalName = "SubPurchasesPrevious",
                                                        Datasource = reportDataDetails.PurchasesPrevious } ,
                                                    new ReportingHandler.SubReportInfo(){
                                                        Name = Constants.REPORT_TYPE_ACCOUNTING_IVARECAP_DETAILS_SUBIVA,
                                                        InternalName = "SubPurchasesCurrent",
                                                        Datasource = reportDataDetails.PurchasesCurrent },
                                                    new ReportingHandler.SubReportInfo(){
                                                        Name = Constants.REPORT_TYPE_ACCOUNTING_IVARECAP_DETAILS_SUBIVA,
                                                        InternalName = "SubPurchasesPreviousPaid",
                                                        Datasource = reportDataDetails.PurchasesPreviousPaid } ,
                                                    new ReportingHandler.SubReportInfo(){
                                                        Name = Constants.REPORT_TYPE_ACCOUNTING_IVARECAP_DETAILS_SUBIVA,
                                                        InternalName = "SubPurchasesCurrentPaid",
                                                        Datasource = reportDataDetails.PurchasesCurrentPaid }
                                                };

                                                ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_IVARECAP_DETAILS, _dataContext.CompanyID, reportDataDetails, $"{reportData.TemporaryText} _dettagli_ dal {_dataContext.PrintSince.Value.Date} al {_dataContext.PrintUntil.Value.Date}", $"{reportData.TemporaryText}_dettagli_{_dataContext.PrintSince.Value.Date.ToString("dd_MM_yyyy")}_{_dataContext.PrintUntil.Value.Date.ToString("dd_MM_yyyy")}.pdf", true, subReports);
                                            }
                                        }
                                        if (_dataContext.IsDefinitive && result != null)
                                        {
                                            // updates
                                            _dataContext.UpdateLiquidationDefinitive(defaultIVABookNumerator, reportData, result);
                                        }
                                    }
                                }
                                else
                                {
                                    Mouse.OverrideCursor = null;
                                    ErrorHandler.Validation("Impossibile recuperare il libro IVA di default per la numerazione della liquidazione");
                                }
                            }
                        }
                        else
                        {
                            ErrorHandler.Validation($"In caso di IVA per cassa le date devono essere comprese nel periodo {_dataContext.FiscalYear?.esedtiniva?.ToString("dd/MM/yyyy")} - {_dataContext.FiscalYear?.esedtfniva?.ToString("dd/MM/yyyy")}");
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation("Esercizio non trovato");
                    }
                }
                else
                {
                    ErrorHandler.Validation("Tutti le informazioni richieste sono obbligatorie");
                }
            }
            else
            {
                if (_dataContext.AccountingYear.HasValue && _dataContext.AccountingYear > 0)
                {
                    if (ConfirmHandler.Confirm($"Confermate la stampa della liquidazione IVA annuale ?"))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;

                        var reportData = _dataContext.GetIVAReportYearly();

                        if (reportData != null)
                        {
                            ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_IVARECAP_YEARLY, _dataContext.CompanyID, reportData, $"{reportData.TemporaryText} del {_dataContext.AccountingYear}", $"{reportData.TemporaryText}_{_dataContext.AccountingYear}.pdf", true);

                            if (_dataContext.FiscalYear != null && _dataContext.FiscalYear.eseivaven == "S")
                            {
                                if (ConfirmHandler.Confirm("Stampare anche i dettagli delle fatture ?"))
                                {
                                    ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_IVADETAILS, _dataContext.CompanyID, reportData, $"{reportData.DetailsText} del {_dataContext.AccountingYear}", $"{reportData.DetailsText}_{_dataContext.AccountingYear}.pdf", true);
                                }
                            }
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation("Tutti le informazioni richieste sono obbligatorie");
                    }
                }
            }
        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.AccountingYear = (cmbAccountingYear.SelectedItem as ESERCIZIO)?.eseann;
            _dataContext.FiscalYear = (cmbAccountingYear.SelectedItem as ESERCIZIO);
        }
    }
}
