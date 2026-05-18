using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Reports;

namespace VulpesX.Modules.Default.Accounting.Reports
{
    /// <summary>
    /// Interaction logic for BalanceSimulationWindow.xaml
    /// </summary>
    public partial class BalanceSimulationWindow : FluentDefaultWindow
    {
        private BalanceSimulationWindowViewModel _dataContext;
        public BalanceSimulationWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<BalanceSimulationWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;

            _dataContext.CostCenters = _dataContext.GetTCECO00Fs();
            _dataContext.AccountingYear = (cmbAccountingYear.Items[0] as ESERCIZIO)?.eseann;
            _dataContext.PrintUntil = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            acCC.SelectedItem = _dataContext.CostCenters?.Where(w => w.cecodc == null).FirstOrDefault();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.PrintUntil.HasValue)
            {
                if (ConfirmHandler.Confirm($"Confermate la stampa del bilancino di verifica al {_dataContext.PrintUntil.Value.ToString("dd/MM/yyyy")}?"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var costCenter = acCC.SelectedItem != null ? (acCC.SelectedItem as TCECO00F) : null;
                    var costCenterID = costCenter != null ? costCenter.cecodc : null;

                    var reportData = _dataContext.GetPDCBalanceReportOpposed(costCenterID);

                    if (reportData != null)
                    {
                        reportData.CostCenter = (costCenter != null) ? $"Centro di costo {costCenter.cecodc}-{costCenter.cedesc}" : string.Empty;

                        var subReports = new List<ReportingHandler.SubReportInfo>()
                        {
                            new ReportingHandler.SubReportInfo(){
                                Name = Constants.REPORT_TYPE_ACCOUNTING_BALANCE_SIMULATION_SUB,
                                InternalName = "SubA",
                                Datasource = reportData.SubreportAP?.LeftGroup },
                            new ReportingHandler.SubReportInfo(){
                                                Name = Constants.REPORT_TYPE_ACCOUNTING_BALANCE_SIMULATION_SUB,
                                                InternalName = "SubP",
                                                Datasource = reportData.SubreportAP?.RightGroup },
                            new ReportingHandler.SubReportInfo(){
                                                Name = Constants.REPORT_TYPE_ACCOUNTING_BALANCE_SIMULATION_SUB,
                                                InternalName = "SubC",
                                                Datasource = reportData.SubreportCR?.LeftGroup },
                            new ReportingHandler.SubReportInfo(){
                                                Name = Constants.REPORT_TYPE_ACCOUNTING_BALANCE_SIMULATION_SUB,
                                                InternalName = "SubR",
                                                Datasource = reportData.SubreportCR?.RightGroup
                        }};
                        var result = ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_BALANCE_SIMULATION, _dataContext.CompanyID, reportData, $"Bilancino al {_dataContext.PrintUntil.Value.Date}", $"Bilancino_al_{_dataContext.PrintUntil.Value.Date.ToString("dd_MM_yyyy")}.pdf", true, subReports);
                    }
                    Mouse.OverrideCursor = null;
                }
            }
            else
            {
                ErrorHandler.Validation("Tutti le informazioni richieste sono obbligatorie");
            }
        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.AccountingYear = (cmbAccountingYear.SelectedItem as ESERCIZIO)?.eseann;
        }

        #region Autocompletes
        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }
        private void ac_LostFocus(object sender, RoutedEventArgs e)
        {
            var ac = sender as RadAutoCompleteBox;
            if (ac != null)
            {
                if (ac.SelectedItem == null)
                {
                    ac.SearchText = null;
                }
            }
        }
        #endregion

        private void cmdExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dataContext.PrintUntil.HasValue)
                {
                    if (ConfirmHandler.Confirm($"Confermate l'estrazione del bilancino di verifica al {_dataContext.PrintUntil.Value.ToString("dd/MM/yyyy")}?"))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        var costCenter = acCC.SelectedItem != null ? (acCC.SelectedItem as TCECO00F) : null;
                        var costCenterID = costCenter != null ? costCenter.cecodc : null;


                        if (_dataContext.SplitMonth)
                        {
                            var monthData = _dataContext.GetPDCBalanceOpposedSplitted(costCenterID);

                            if (monthData != null)
                            {
                                Microsoft.Win32.SaveFileDialog sfdExcel = new Microsoft.Win32.SaveFileDialog();
                                sfdExcel.Filter = "Excel |*.xlsx";
                                sfdExcel.ShowDialog();

                                if (!string.IsNullOrEmpty(sfdExcel.FileName))
                                {
                                    using (var workbook = new XLWorkbook())
                                    {
                                        var rowID = 2;

                                        #region Bilancio
                                        var worksheetName = "Bilancio";
                                        var worksheet = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                                        worksheet.Cell(rowID, 1).Value = "Gruppo";
                                        worksheet.Cell(rowID, 2).Value = "Conto";
                                        worksheet.Cell(rowID, 3).Value = "Sottoconto";
                                        worksheet.Cell(rowID, 4).Value = "Descrizione";

                                        int columnHeader = 5;
                                        List<Tuple<int, int>> columnsMonths = new List<Tuple<int, int>>();

                                        foreach (var mon in monthData)
                                        {
                                            worksheet.Cell(rowID - 1, columnHeader).Value = $"{mon.Item1} - {new DateTime(_dataContext.AccountingYear!.Value, mon.Item1, 1).ToString("MMMM", new CultureInfo("it-IT"))}";

                                            worksheet.Cell(rowID, columnHeader).Value = "Saldo";
                                            worksheet.Cell(rowID, columnHeader + 1).Value = "Segno";

                                            worksheet.Column(columnHeader).Style.NumberFormat.Format = "0.00";

                                            columnsMonths.Add(new Tuple<int, int>(mon.Item1, columnHeader));

                                            ++columnHeader;
                                            ++columnHeader;
                                        }

                                        worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Fill.BackgroundColor = XLColor.LightGray;

                                        worksheet.Range(rowID, 1, rowID, 4).SetAutoFilter();
                                        worksheet.SheetView.FreezeRows(2);

                                        //ATTIVITA
                                        List<Tuple<string?, string?, string?, string?, bool, string?, int>> rowsActivitySubaccounts = new List<Tuple<string?, string?, string?, string?, bool, string?, int>>();

                                        foreach (var stc in monthData.SelectMany(s => s.Item2?.SubreportAP?.LeftGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>()).GroupBy(g => new { g.GroupID, g.AccountID, g.SubaccountID, g.SubaccountDescription, g.IsSubTotal, g.SubtotalText }).OrderBy(o => o.Key.GroupID).ThenBy(o => o.Key.AccountID).ThenBy(o => o.Key.SubaccountID))
                                        {
                                            ++rowID;

                                            if (stc.Key.IsSubTotal)
                                            {
                                                worksheet.Cell(rowID, 1).Value = stc.First().SubTotalMark;
                                                worksheet.Cell(rowID, 4).Value = stc.Key.SubtotalText;

                                                worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Font.Bold = true;
                                                worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                if ((stc.FirstOrDefault()?.IsGroupSubtotal) ?? false)
                                                    worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                            }
                                            else
                                            {
                                                worksheet.Cell(rowID, 1).Value = stc.Key.GroupID;
                                                worksheet.Cell(rowID, 2).Value = stc.Key.AccountID;
                                                worksheet.Cell(rowID, 3).Value = stc.Key.SubaccountID;
                                                worksheet.Cell(rowID, 4).Value = stc.Key.SubaccountDescription;
                                            }

                                            rowsActivitySubaccounts.Add(new Tuple<string?, string?, string?, string?, bool, string?, int>(stc.Key.GroupID, stc.Key.AccountID, stc.Key.SubaccountID, stc.Key.SubaccountDescription, stc.Key.IsSubTotal, stc.Key.SubtotalText, rowID));
                                        }

                                        foreach (var mon in monthData)
                                        {
                                            var col = columnsMonths.Where(o => o.Item1 == mon.Item1).Select(s => s.Item2).FirstOrDefault();

                                            foreach (var cmp in mon?.Item2.SubreportAP?.LeftGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                var row = rowsActivitySubaccounts.Where(o => o.Item1 == cmp.GroupID && o.Item2 == cmp.AccountID && o.Item3 == cmp.SubaccountID && o.Item4 == cmp.SubaccountDescription && o.Item5 == cmp.IsSubTotal && o.Item6 == cmp.SubtotalText).Select(s => s.Item7).FirstOrDefault();

                                                if (cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(row, col).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(row, col + 1).Value = cmp.SubTotalAmountSign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(row, col).Value = cmp.Amount;
                                                    worksheet.Cell(row, col + 1).Value = cmp.Sign;
                                                }
                                            }
                                        }

                                        //PASSIVITA
                                        List<Tuple<string?, string?, string?, string?, bool, string?, int>> rowsPassivitySubaccounts = new List<Tuple<string?, string?, string?, string?, bool, string?, int>>();

                                        foreach (var stc in monthData.SelectMany(s => s.Item2?.SubreportAP?.RightGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>()).GroupBy(g => new { g.GroupID, g.AccountID, g.SubaccountID, g.SubaccountDescription, g.IsSubTotal, g.SubtotalText }).OrderBy(o => o.Key.GroupID).ThenBy(o => o.Key.AccountID).ThenBy(o => o.Key.SubaccountID))
                                        {
                                            ++rowID;

                                            if (stc.Key.IsSubTotal)
                                            {
                                                worksheet.Cell(rowID, 1).Value = stc.First().SubTotalMark;
                                                worksheet.Cell(rowID, 4).Value = stc.Key.SubtotalText;

                                                worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Font.Bold = true;
                                                worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                if ((stc.FirstOrDefault()?.IsGroupSubtotal) ?? false)
                                                    worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                            }
                                            else
                                            {
                                                worksheet.Cell(rowID, 1).Value = stc.Key.GroupID;
                                                worksheet.Cell(rowID, 2).Value = stc.Key.AccountID;
                                                worksheet.Cell(rowID, 3).Value = stc.Key.SubaccountID;
                                                worksheet.Cell(rowID, 4).Value = stc.Key.SubaccountDescription;
                                            }

                                            rowsPassivitySubaccounts.Add(new Tuple<string?, string?, string?, string?, bool, string?, int>(stc.Key.GroupID, stc.Key.AccountID, stc.Key.SubaccountID, stc.Key.SubaccountDescription, stc.Key.IsSubTotal, stc.Key.SubtotalText, rowID));
                                        }

                                        foreach (var mon in monthData)
                                        {
                                            var col = columnsMonths.Where(o => o.Item1 == mon.Item1).Select(s => s.Item2).FirstOrDefault();

                                            foreach (var cmp in mon?.Item2.SubreportAP?.RightGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                var row = rowsPassivitySubaccounts.Where(o => o.Item1 == cmp.GroupID && o.Item2 == cmp.AccountID && o.Item3 == cmp.SubaccountID && o.Item4 == cmp.SubaccountDescription && o.Item5 == cmp.IsSubTotal && o.Item6 == cmp.SubtotalText).Select(s => s.Item7).FirstOrDefault();

                                                if (cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(row, col).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(row, col + 1).Value = cmp.SubTotalAmountSign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(row, col).Value = cmp.Amount;
                                                    worksheet.Cell(row, col + 1).Value = cmp.Sign;
                                                }
                                            }
                                        }

                                        worksheet.ColumnsUsed().AdjustToContents();
                                        #endregion

                                        rowID = 2;

                                        #region Conto economico
                                        worksheetName = "Conto economico";
                                        worksheet = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                                        worksheet.Cell(rowID, 1).Value = "Gruppo";
                                        worksheet.Cell(rowID, 2).Value = "Conto";
                                        worksheet.Cell(rowID, 3).Value = "Sottoconto";
                                        worksheet.Cell(rowID, 4).Value = "Descrizione";

                                        columnHeader = 5;
                                        columnsMonths = new List<Tuple<int, int>>();

                                        foreach (var mon in monthData)
                                        {
                                            worksheet.Cell(rowID - 1, columnHeader).Value = $"{mon.Item1} - {new DateTime(_dataContext.AccountingYear!.Value, mon.Item1, 1).ToString("MMMM", new CultureInfo("it-IT"))}";

                                            worksheet.Cell(rowID, columnHeader).Value = "Saldo";
                                            worksheet.Cell(rowID, columnHeader + 1).Value = "Segno";

                                            worksheet.Column(columnHeader).Style.NumberFormat.Format = "0.00";

                                            columnsMonths.Add(new Tuple<int, int>(mon.Item1, columnHeader));

                                            ++columnHeader;
                                            ++columnHeader;
                                        }

                                        worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Fill.BackgroundColor = XLColor.LightGray;

                                        worksheet.Range(rowID, 1, rowID, 4).SetAutoFilter();
                                        worksheet.SheetView.FreezeRows(2);

                                        //COSTI
                                        List<Tuple<string?, string?, string?, string?, bool, string?, int>> rowsCostSubaccounts = new List<Tuple<string?, string?, string?, string?, bool, string?, int>>();

                                        foreach (var stc in monthData.SelectMany(s => s.Item2?.SubreportCR?.LeftGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>()).GroupBy(g => new { g.GroupID, g.AccountID, g.SubaccountID, g.SubaccountDescription, g.IsSubTotal, g.SubtotalText }).OrderBy(o => o.Key.GroupID).ThenBy(o => o.Key.AccountID).ThenBy(o => o.Key.SubaccountID))
                                        {
                                            ++rowID;

                                            if (stc.Key.IsSubTotal)
                                            {
                                                worksheet.Cell(rowID, 1).Value = stc.First().SubTotalMark;
                                                worksheet.Cell(rowID, 4).Value = stc.Key.SubtotalText;

                                                worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Font.Bold = true;
                                                worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                if ((stc.FirstOrDefault()?.IsGroupSubtotal) ?? false)
                                                    worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                            }
                                            else
                                            {
                                                worksheet.Cell(rowID, 1).Value = stc.Key.GroupID;
                                                worksheet.Cell(rowID, 2).Value = stc.Key.AccountID;
                                                worksheet.Cell(rowID, 3).Value = stc.Key.SubaccountID;
                                                worksheet.Cell(rowID, 4).Value = stc.Key.SubaccountDescription;
                                            }

                                            rowsCostSubaccounts.Add(new Tuple<string?, string?, string?, string?, bool, string?, int>(stc.Key.GroupID, stc.Key.AccountID, stc.Key.SubaccountID, stc.Key.SubaccountDescription, stc.Key.IsSubTotal, stc.Key.SubtotalText, rowID));
                                        }

                                        foreach (var mon in monthData)
                                        {
                                            var col = columnsMonths.Where(o => o.Item1 == mon.Item1).Select(s => s.Item2).FirstOrDefault();

                                            foreach (var cmp in mon?.Item2.SubreportCR?.LeftGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                var row = rowsCostSubaccounts.Where(o => o.Item1 == cmp.GroupID && o.Item2 == cmp.AccountID && o.Item3 == cmp.SubaccountID && o.Item4 == cmp.SubaccountDescription && o.Item5 == cmp.IsSubTotal && o.Item6 == cmp.SubtotalText).Select(s => s.Item7).FirstOrDefault();

                                                if (cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(row, col).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(row, col + 1).Value = cmp.SubTotalAmountSign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(row, col).Value = cmp.Amount;
                                                    worksheet.Cell(row, col + 1).Value = cmp.Sign;
                                                }
                                            }
                                        }

                                        //RICAVI
                                        List<Tuple<string?, string?, string?, string?, bool, string?, int>> rowsGainSubaccounts = new List<Tuple<string?, string?, string?, string?, bool, string?, int>>();

                                        foreach (var stc in monthData.SelectMany(s => s.Item2?.SubreportCR?.RightGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>()).GroupBy(g => new { g.GroupID, g.AccountID, g.SubaccountID, g.SubaccountDescription, g.IsSubTotal, g.SubtotalText }).OrderBy(o => o.Key.GroupID).ThenBy(o => o.Key.AccountID).ThenBy(o => o.Key.SubaccountID))
                                        {
                                            ++rowID;

                                            if (stc.Key.IsSubTotal)
                                            {
                                                worksheet.Cell(rowID, 1).Value = stc.First().SubTotalMark;
                                                worksheet.Cell(rowID, 4).Value = stc.Key.SubtotalText;

                                                worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Font.Bold = true;
                                                worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                if ((stc.FirstOrDefault()?.IsGroupSubtotal) ?? false)
                                                    worksheet.Range(rowID, 1, rowID, columnsMonths.Max(o => o.Item2) + 1).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                            }
                                            else
                                            {
                                                worksheet.Cell(rowID, 1).Value = stc.Key.GroupID;
                                                worksheet.Cell(rowID, 2).Value = stc.Key.AccountID;
                                                worksheet.Cell(rowID, 3).Value = stc.Key.SubaccountID;
                                                worksheet.Cell(rowID, 4).Value = stc.Key.SubaccountDescription;
                                            }

                                            rowsGainSubaccounts.Add(new Tuple<string?, string?, string?, string?, bool, string?, int>(stc.Key.GroupID, stc.Key.AccountID, stc.Key.SubaccountID, stc.Key.SubaccountDescription, stc.Key.IsSubTotal, stc.Key.SubtotalText, rowID));
                                        }

                                        foreach (var mon in monthData)
                                        {
                                            var col = columnsMonths.Where(o => o.Item1 == mon.Item1).Select(s => s.Item2).FirstOrDefault();

                                            foreach (var cmp in mon?.Item2.SubreportCR?.RightGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                var row = rowsGainSubaccounts.Where(o => o.Item1 == cmp.GroupID && o.Item2 == cmp.AccountID && o.Item3 == cmp.SubaccountID && o.Item4 == cmp.SubaccountDescription && o.Item5 == cmp.IsSubTotal && o.Item6 == cmp.SubtotalText).Select(s => s.Item7).FirstOrDefault();

                                                if (cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(row, col).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(row, col + 1).Value = cmp.SubTotalAmountSign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(row, col).Value = cmp.Amount;
                                                    worksheet.Cell(row, col + 1).Value = cmp.Sign;
                                                }
                                            }
                                        }

                                        worksheet.ColumnsUsed().AdjustToContents();
                                        #endregion

                                        workbook.SaveAs(sfdExcel.FileName);

                                        if (System.IO.File.Exists(sfdExcel.FileName))
                                            FileHelper.Open(sfdExcel.FileName);
                                    }
                                }
                            }
                        }
                        else
                        {
                            PDCBalanceReportOpposed? reportData = null;

                            if (_dataContext.GroupDiva)
                            {
                                reportData = _dataContext.GetPDCBalanceReportOpposedDIVA(costCenterID);

                                if (reportData != null)
                                {
                                    reportData.CostCenter = (costCenter != null) ? $"Centro di costo {costCenter.cecodc}-{costCenter.cedesc}" : string.Empty;

                                    Microsoft.Win32.SaveFileDialog sfdExcel = new Microsoft.Win32.SaveFileDialog();
                                    sfdExcel.Filter = "Excel |*.xlsx";
                                    sfdExcel.ShowDialog();

                                    if (!string.IsNullOrEmpty(sfdExcel.FileName))
                                    {
                                        using (var workbook = new XLWorkbook())
                                        {
                                            var rowID = 1;

                                            #region Bilancio
                                            var worksheetName = "Bilancio";
                                            var worksheet = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                                            worksheet.Cell(rowID, 1).Value = "DIVA";
                                            worksheet.Cell(rowID, 2).Value = "Saldo";
                                            worksheet.Cell(rowID, 3).Value = "Segno";

                                            worksheet.Range(rowID, 1, rowID, 3).Style.Fill.BackgroundColor = XLColor.LightGray;

                                            worksheet.Range(rowID, 1, rowID, 1).SetAutoFilter();
                                            worksheet.SheetView.FreezeRows(1);

                                            // ATTIVITA'
                                            foreach (var cmp in reportData.SubreportAP?.LeftGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                ++rowID;
                                                if (!cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubaccountDescription;
                                                    worksheet.Cell(rowID, 2).Value = cmp.Amount;
                                                    worksheet.Cell(rowID, 3).Value = cmp.Sign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubTotalMark;
                                                    worksheet.Cell(rowID, 2).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(rowID, 3).Value = cmp.SubTotalAmountSign;

                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 3)).Style.Font.Bold = true;

                                                    worksheet.Range(rowID, 1, rowID, 3).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                    if (cmp.IsGroupSubtotal)
                                                        worksheet.Range(rowID, 1, rowID, 3).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                                }
                                            }
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportAP?.LeftGroup?.TotalText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportAP?.LeftGroup?.TotalAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportAP?.LeftGroup?.TotalAmountSign;
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportAP?.LeftGroup?.BalanceText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportAP?.LeftGroup?.BalanceAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportAP?.LeftGroup?.BalanceAmountSign;

                                            // PASSIVITA'
                                            foreach (var cmp in reportData.SubreportAP?.RightGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                ++rowID;
                                                if (!cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubaccountDescription;
                                                    worksheet.Cell(rowID, 2).Value = cmp.Amount;
                                                    worksheet.Cell(rowID, 3).Value = cmp.Sign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubTotalMark;
                                                    worksheet.Cell(rowID, 2).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(rowID, 3).Value = cmp.SubTotalAmountSign;

                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Font.Bold = true;

                                                    worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                    if (cmp.IsGroupSubtotal)
                                                        worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                                }
                                            }
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportAP?.RightGroup?.TotalText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportAP?.RightGroup?.TotalAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportAP?.RightGroup?.TotalAmountSign;
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportAP?.RightGroup?.BalanceText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportAP?.RightGroup?.BalanceAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportAP?.RightGroup?.BalanceAmountSign;

                                            worksheet.Columns(1, 200).AdjustToContents();
                                            worksheet.Column("E").Style.NumberFormat.Format = "0.00";
                                            #endregion

                                            rowID = 1;

                                            #region Conto economico
                                            worksheetName = "Conto economico";
                                            worksheet = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                                            worksheet.Cell(rowID, 1).Value = "DIVA";
                                            worksheet.Cell(rowID, 2).Value = "Saldo";
                                            worksheet.Cell(rowID, 3).Value = "Segno";

                                            // COSTI'
                                            foreach (var cmp in reportData.SubreportCR?.LeftGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                ++rowID;
                                                if (!cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubaccountDescription;
                                                    worksheet.Cell(rowID, 2).Value = cmp.Amount;
                                                    worksheet.Cell(rowID, 3).Value = cmp.Sign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubTotalMark;
                                                    worksheet.Cell(rowID, 2).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(rowID, 3).Value = cmp.SubTotalAmountSign;

                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Font.Bold = true;

                                                    worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                    if (cmp.IsGroupSubtotal)
                                                        worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                                }
                                            }
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportCR?.LeftGroup?.TotalText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportCR?.LeftGroup?.TotalAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportCR?.LeftGroup?.TotalAmountSign;
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportCR?.LeftGroup?.BalanceText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportCR?.LeftGroup?.BalanceAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportCR?.LeftGroup?.BalanceAmountSign;

                                            // RICAVI'
                                            foreach (var cmp in reportData.SubreportCR?.RightGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                ++rowID;
                                                if (!cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubaccountDescription;
                                                    worksheet.Cell(rowID, 2).Value = cmp.Amount;
                                                    worksheet.Cell(rowID, 3).Value = cmp.Sign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubTotalMark;
                                                    worksheet.Cell(rowID, 2).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(rowID, 3).Value = cmp.SubTotalAmountSign;

                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Font.Bold = true;

                                                    worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                    if (cmp.IsGroupSubtotal)
                                                        worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                                }
                                            }
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportCR?.RightGroup?.TotalText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportCR?.RightGroup?.TotalAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportCR?.RightGroup?.TotalAmountSign;
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportCR?.RightGroup?.BalanceText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportCR?.RightGroup?.BalanceAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportCR?.RightGroup?.BalanceAmountSign;

                                            worksheet.Columns(1, 200).AdjustToContents();
                                            worksheet.Column("E").Style.NumberFormat.Format = "0.00";
                                            #endregion

                                            workbook.SaveAs(sfdExcel.FileName);

                                            if (System.IO.File.Exists(sfdExcel.FileName))
                                                FileHelper.Open(sfdExcel.FileName);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                reportData = _dataContext.GetPDCBalanceReportOpposed(costCenterID);

                                if (reportData != null)
                                {
                                    reportData.CostCenter = (costCenter != null) ? $"Centro di costo {costCenter.cecodc}-{costCenter.cedesc}" : string.Empty;

                                    Microsoft.Win32.SaveFileDialog sfdExcel = new Microsoft.Win32.SaveFileDialog();
                                    sfdExcel.Filter = "Excel |*.xlsx";
                                    sfdExcel.ShowDialog();

                                    if (!string.IsNullOrEmpty(sfdExcel.FileName))
                                    {
                                        using (var workbook = new XLWorkbook())
                                        {
                                            var rowID = 1;

                                            #region Bilancio
                                            var worksheetName = "Bilancio";
                                            var worksheet = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                                            worksheet.Cell(rowID, 1).Value = "Gruppo";
                                            worksheet.Cell(rowID, 2).Value = "Conto";
                                            worksheet.Cell(rowID, 3).Value = "Sottoconto";
                                            worksheet.Cell(rowID, 4).Value = "Descrizione";
                                            worksheet.Cell(rowID, 5).Value = "Saldo";
                                            worksheet.Cell(rowID, 6).Value = "Segno";

                                            worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightGray;

                                            worksheet.Range(rowID, 1, rowID, 4).SetAutoFilter();
                                            worksheet.SheetView.FreezeRows(1);

                                            // ATTIVITA'
                                            foreach (var cmp in reportData.SubreportAP?.LeftGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                ++rowID;
                                                if (!cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.GroupID;
                                                    worksheet.Cell(rowID, 2).Value = cmp.AccountID;
                                                    worksheet.Cell(rowID, 3).Value = cmp.SubaccountID;
                                                    worksheet.Cell(rowID, 4).Value = cmp.SubaccountDescription;
                                                    worksheet.Cell(rowID, 5).Value = cmp.Amount;
                                                    worksheet.Cell(rowID, 6).Value = cmp.Sign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubTotalMark;
                                                    worksheet.Cell(rowID, 4).Value = cmp.SubtotalText;
                                                    worksheet.Cell(rowID, 5).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(rowID, 6).Value = cmp.SubTotalAmountSign;

                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 3)).Merge();
                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Font.Bold = true;

                                                    worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                    if (cmp.IsGroupSubtotal)
                                                        worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                                }
                                            }
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportAP?.LeftGroup?.TotalText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportAP?.LeftGroup?.TotalAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportAP?.LeftGroup?.TotalAmountSign;
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportAP?.LeftGroup?.BalanceText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportAP?.LeftGroup?.BalanceAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportAP?.LeftGroup?.BalanceAmountSign;

                                            // PASSIVITA'
                                            foreach (var cmp in reportData.SubreportAP?.RightGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                ++rowID;
                                                if (!cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.GroupID;
                                                    worksheet.Cell(rowID, 2).Value = cmp.AccountID;
                                                    worksheet.Cell(rowID, 3).Value = cmp.SubaccountID;
                                                    worksheet.Cell(rowID, 4).Value = cmp.SubaccountDescription;
                                                    worksheet.Cell(rowID, 5).Value = cmp.Amount;
                                                    worksheet.Cell(rowID, 6).Value = cmp.Sign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubTotalMark;
                                                    worksheet.Cell(rowID, 4).Value = cmp.SubtotalText;
                                                    worksheet.Cell(rowID, 5).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(rowID, 6).Value = cmp.SubTotalAmountSign;

                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 3)).Merge();
                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Font.Bold = true;

                                                    worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                    if (cmp.IsGroupSubtotal)
                                                        worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                                }
                                            }
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportAP?.RightGroup?.TotalText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportAP?.RightGroup?.TotalAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportAP?.RightGroup?.TotalAmountSign;
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportAP?.RightGroup?.BalanceText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportAP?.RightGroup?.BalanceAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportAP?.RightGroup?.BalanceAmountSign;

                                            worksheet.Columns(1, 200).AdjustToContents();
                                            worksheet.Column("E").Style.NumberFormat.Format = "0.00";
                                            #endregion

                                            rowID = 1;

                                            #region Conto economico
                                            worksheetName = "Conto economico";
                                            worksheet = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                                            worksheet.Cell(rowID, 1).Value = "Gruppo";
                                            worksheet.Cell(rowID, 2).Value = "Conto";
                                            worksheet.Cell(rowID, 3).Value = "Sottoconto";
                                            worksheet.Cell(rowID, 4).Value = "Descrizione";
                                            worksheet.Cell(rowID, 5).Value = "Saldo";
                                            worksheet.Cell(rowID, 6).Value = "Segno";

                                            // COSTI'
                                            foreach (var cmp in reportData.SubreportCR?.LeftGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                ++rowID;
                                                if (!cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.GroupID;
                                                    worksheet.Cell(rowID, 2).Value = cmp.AccountID;
                                                    worksheet.Cell(rowID, 3).Value = cmp.SubaccountID;
                                                    worksheet.Cell(rowID, 4).Value = cmp.SubaccountDescription;
                                                    worksheet.Cell(rowID, 5).Value = cmp.Amount;
                                                    worksheet.Cell(rowID, 6).Value = cmp.Sign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubTotalMark;
                                                    worksheet.Cell(rowID, 4).Value = cmp.SubtotalText;
                                                    worksheet.Cell(rowID, 5).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(rowID, 6).Value = cmp.SubTotalAmountSign;

                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 3)).Merge();
                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Font.Bold = true;

                                                    worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                    if (cmp.IsGroupSubtotal)
                                                        worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                                }
                                            }
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportCR?.LeftGroup?.TotalText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportCR?.LeftGroup?.TotalAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportCR?.LeftGroup?.TotalAmountSign;
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportCR?.LeftGroup?.BalanceText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportCR?.LeftGroup?.BalanceAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportCR?.LeftGroup?.BalanceAmountSign;

                                            // RICAVI'
                                            foreach (var cmp in reportData.SubreportCR?.RightGroup?.Rows ?? new List<Models.Models.Reports.Accounting.PDCBalanceReport.PDCBalanceReportItem>())
                                            {
                                                ++rowID;
                                                if (!cmp.IsSubTotal)
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.GroupID;
                                                    worksheet.Cell(rowID, 2).Value = cmp.AccountID;
                                                    worksheet.Cell(rowID, 3).Value = cmp.SubaccountID;
                                                    worksheet.Cell(rowID, 4).Value = cmp.SubaccountDescription;
                                                    worksheet.Cell(rowID, 5).Value = cmp.Amount;
                                                    worksheet.Cell(rowID, 6).Value = cmp.Sign;
                                                }
                                                else
                                                {
                                                    worksheet.Cell(rowID, 1).Value = cmp.SubTotalMark;
                                                    worksheet.Cell(rowID, 4).Value = cmp.SubtotalText;
                                                    worksheet.Cell(rowID, 5).Value = cmp.SubTotalAmount;
                                                    worksheet.Cell(rowID, 6).Value = cmp.SubTotalAmountSign;

                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 3)).Merge();
                                                    worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Font.Bold = true;

                                                    worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightBlue;

                                                    if (cmp.IsGroupSubtotal)
                                                        worksheet.Range(rowID, 1, rowID, 6).Style.Fill.BackgroundColor = XLColor.LightCoral;
                                                }
                                            }
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportCR?.RightGroup?.TotalText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportCR?.RightGroup?.TotalAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportCR?.RightGroup?.TotalAmountSign;
                                            ++rowID;
                                            worksheet.Cell(rowID, 1).Value = reportData.SubreportCR?.RightGroup?.BalanceText;
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Merge();
                                            worksheet.Range(worksheet.Cell(rowID, 1), worksheet.Cell(rowID, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                                            worksheet.Cell(rowID, 5).Value = reportData.SubreportCR?.RightGroup?.BalanceAmount;
                                            worksheet.Cell(rowID, 6).Value = reportData.SubreportCR?.RightGroup?.BalanceAmountSign;

                                            worksheet.Columns(1, 200).AdjustToContents();
                                            worksheet.Column("E").Style.NumberFormat.Format = "0.00";
                                            #endregion

                                            workbook.SaveAs(sfdExcel.FileName);

                                            if (System.IO.File.Exists(sfdExcel.FileName))
                                                FileHelper.Open(sfdExcel.FileName);
                                        }
                                    }
                                }
                            }
                        }
                        Mouse.OverrideCursor = null;
                    }
                }
                else
                {
                    ErrorHandler.Validation("Tutti le informazioni richieste sono obbligatorie");
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message.ToString());
            }
        }

    }
}
