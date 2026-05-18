using Chilkat;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.IVA;

namespace VulpesX.Modules.Default.Accounting.IVA
{
    /// <summary>
    /// Interaction logic for IVABookWindow.xaml
    /// </summary>
    public partial class IVABookWindow : FluentDefaultWindow
    {
        private IVABookWindowViewModel _dataContext;
        public IVABookWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<IVABookWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            _dataContext.AccountingYear = (cmbAccountingYear.Items[0] as ESERCIZIO)?.eseann;
            _dataContext.IsDefinitive = false;
            _dataContext.PrintSince = now;
            _dataContext.PrintUntil = now;
            _dataContext.IVABooks = _dataContext.GetLIBRIIVAs();
        }

        private async void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.AccountingYear > 0 && _dataContext.SelectedIVABook != null && _dataContext.PrintSince.HasValue && _dataContext.PrintUntil.HasValue)
            {
                if (MessageBoxResult.Yes == MessageBox.Show($"Confermate la stampa {(_dataContext.IsDefinitive ? "DEFINITIVA" : "PROVVISORIA")} del libro IVA {_dataContext.SelectedIVABook.FullDescriptionSearchable} ?", Constants.APP_NAME, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No))
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    var reportData = _dataContext.GetIVABookReport();

                    if (reportData != null)
                    {
                        var result = ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_IVABOOK, _dataContext.CompanyID, reportData, $"Libro IVA {_dataContext.SelectedIVABook.FullDescriptionSearchable} dal {_dataContext.PrintSince.Value.Date} al {_dataContext.PrintUntil.Value.Date}", $"LibroIVA_{_dataContext.SelectedIVABook.livcod}_{_dataContext.SelectedIVABook.livdes}_{_dataContext.PrintSince.Value.Date.ToString("dd_MM_yyyy")}_{_dataContext.PrintUntil.Value.Date.ToString("dd_MM_yyyy")}.pdf", true);

                        if (result != null && _dataContext.IsDefinitive)
                        {
                            // update numerator and PNIVA rows
                            await _dataContext.UpdatePrintedDefinitives(reportData, result);
                        }
                    }
                }
            }
            else
            {
                ErrorHandler.Validation("Tutti le informazioni richieste sono obbligatorie");
            }
        }

        private void cmdExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dataContext.AccountingYear > 0 && _dataContext.SelectedIVABook != null && _dataContext.PrintSince.HasValue && _dataContext.PrintUntil.HasValue)
                {
                    if (ConfirmHandler.Confirm($"Confermate l'estrazione del libro IVA {_dataContext.SelectedIVABook.FullDescriptionSearchable} ?"))
                    {
                        var reportData = _dataContext.GetIVABookReport();

                        if (reportData != null)
                        {
                            Microsoft.Win32.SaveFileDialog sfdExcel = new Microsoft.Win32.SaveFileDialog();
                            sfdExcel.Filter = "Excel |*.xlsx";
                            sfdExcel.ShowDialog();

                            if (!string.IsNullOrEmpty(sfdExcel.FileName))
                            {
                                using (var workbook = new XLWorkbook())
                                {
                                    var rowID = 1;

                                    var worksheetName = $"Libro IVA - {reportData.IVABook?.livcod}";
                                    var worksheet = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                                    worksheet.Cell(rowID, 1).Value = "Data registrazione";
                                    worksheet.Cell(rowID, 2).Value = "Numero protocollo";
                                    worksheet.Cell(rowID, 3).Value = "Data protocollo";
                                    worksheet.Cell(rowID, 4).Value = "N° fattura";
                                    worksheet.Cell(rowID, 5).Value = "Data fattura";
                                    worksheet.Cell(rowID, 6).Value = "Partita IVA";
                                    worksheet.Cell(rowID, 7).Value = "Ragione sociale";
                                    worksheet.Cell(rowID, 8).Value = "Imponibile";
                                    worksheet.Cell(rowID, 9).Value = "Segno";
                                    worksheet.Cell(rowID, 10).Value = "Aliquota";
                                    worksheet.Cell(rowID, 11).Value = "Imposta";
                                    worksheet.Cell(rowID, 12).Value = "Segno";
                                    worksheet.Cell(rowID, 13).Value = "Indeducibile";
                                    worksheet.Cell(rowID, 14).Value = "Segno";

                                    worksheet.Range(rowID, 1, rowID, 14).Style.Fill.BackgroundColor = XLColor.LightGray;
                                    worksheet.Range(rowID, 1, rowID, 14).SetAutoFilter();
                                    worksheet.SheetView.FreezeRows(1);

                                    PNIVA? last = null;
                                    foreach (var row in (reportData.Rows ?? new List<PNIVA>()).Where(o=>o.N4REGI > 0))
                                    {
                                        ++rowID;
                                        worksheet.Cell(rowID, 1).Value = row.N4DARE ?? last?.N4DARE;
                                        worksheet.Cell(rowID, 2).Value = row.N4DOCU ?? last?.N4DOCU;
                                        worksheet.Cell(rowID, 3).Value = row.N4DADO ?? last?.N4DADO;
                                        worksheet.Cell(rowID, 4).Value = row.N4RIFE ?? last?.N4RIFE;
                                        worksheet.Cell(rowID, 5).Value = row.N4DARI ?? last?.N4DARI;
                                        worksheet.Cell(rowID, 6).Value = row.VATID ?? last?.VATID;
                                        worksheet.Cell(rowID, 7).Value = row.CompanyDescription ?? last?.CompanyDescription;
                                        worksheet.Cell(rowID, 8).Value = row.N4IMEU;
                                        worksheet.Cell(rowID, 9).Value = row.N4SEGN;
                                        worksheet.Cell(rowID, 10).Value = row.RateFullDescription;
                                        worksheet.Cell(rowID, 11).Value = row.N4IVEU;
                                        worksheet.Cell(rowID, 12).Value = row.N4SEGN;
                                        worksheet.Cell(rowID, 13).Value = row.N4IIEU ?? 0;
                                        worksheet.Cell(rowID, 14).Value = row.N4SEGN;

                                        if(!string.IsNullOrEmpty(row.CompanyDescription))
                                            last = row;
                                    }

                                    worksheet.Column(1).Style.DateFormat.Format = "dd/MM/yyyy";
                                    worksheet.Column(3).Style.DateFormat.Format = "dd/MM/yyyy";
                                    worksheet.Column(5).Style.DateFormat.Format = "dd/MM/yyyy";
                                    worksheet.Column(8).Style.NumberFormat.Format = "0.00";
                                    worksheet.Column(11).Style.NumberFormat.Format = "0.00";
                                    worksheet.Column(13).Style.NumberFormat.Format = "0.00";

                                    rowID = 1;
                                    worksheet.Cell(rowID, 16).Value = "Aliquota";
                                    worksheet.Cell(rowID, 17).Value = "Imponibile";
                                    worksheet.Cell(rowID, 18).Value = "Imposta";
                                    worksheet.Cell(rowID, 19).Value = "IVA indeducibile";

                                    worksheet.Range(rowID, 16, rowID, 19).Style.Fill.BackgroundColor = XLColor.LightGray;
                                    worksheet.Range(rowID, 16, rowID, 19).SetAutoFilter();

                                    foreach (var vat in reportData.VATs ?? new List<Models.Models.Reports.Accounting.VATRecap>())
                                    {
                                        ++rowID;
                                        worksheet.Cell(rowID, 16).Value = vat.RateFullDescription;
                                        worksheet.Cell(rowID, 17).Value = vat.TotalAmount;
                                        worksheet.Cell(rowID, 18).Value = vat.TotalVATAmount;
                                        worksheet.Cell(rowID, 19).Value = vat.TotalNoVATAmount;
                                    }

                                    worksheet.Column(17).Style.NumberFormat.Format = "0.00";
                                    worksheet.Column(18).Style.NumberFormat.Format = "0.00";
                                    worksheet.Column(19).Style.NumberFormat.Format = "0.00";

                               

                                    worksheet.Columns(1, 200).AdjustToContents();
                                    workbook.SaveAs(sfdExcel.FileName);

                                    if (System.IO.File.Exists(sfdExcel.FileName))
                                        FileHelper.Open(sfdExcel.FileName);
                                }
                            }
                        }
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


    }
}
