using ClosedXML.Excel;
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
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using VulpesX.DAL;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Reports;

namespace VulpesX.Modules.Default.Accounting.Reports
{
    /// <summary>
    /// Interaction logic for BatchReportWindow.xaml
    /// </summary>
    public partial class BatchReportWindow : FluentDefaultWindow
    {
        private BatchReportWindowViewModel _dataContext;
        public BatchReportWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<BatchReportWindowViewModel>();

            InitializeComponent();

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            _dataContext.EntityType = "F";
            _dataContext.From = now.AddMonths(-1);
            _dataContext.To = now;

            this.DataContext = _dataContext;
        }

        private async void cmdExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.From.HasValue && _dataContext.To.HasValue)
            {
                if (_dataContext.From <= _dataContext.To)
                {
                    if (ConfirmHandler.Confirm($"Confermate l'estrazione del partitario {(_dataContext.EntityType == "C" ? "clienti" : "fornitori")} per il periodo \n {_dataContext.From.Value.ToString("dd/MM/yyyy")} - {_dataContext.To.Value.ToString("dd/MM/yyyy")}?"))
                    {
                        await _dataContext.Load();

                        if (_dataContext.Items != null && _dataContext.Items.Any())
                        {
                            Microsoft.Win32.SaveFileDialog sfdExcel = new Microsoft.Win32.SaveFileDialog();
                            sfdExcel.Filter = "Excel |*.xlsx";
                            sfdExcel.ShowDialog();

                            if (!string.IsNullOrEmpty(sfdExcel.FileName))
                            {
                                using (var workbook = new XLWorkbook())
                                {

                                    var rowID = 2;

                                    var worksheetName = "Dati";
                                    var worksheet = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                                    worksheet.Cell(1, 1).Value = $"Partitario {(_dataContext.EntityType == "C" ? "clienti" : "fornitori")} - {_dataContext.From.Value.ToString("dd/MM/yyyy")} - {_dataContext.To.Value.ToString("dd/MM/yyyy")}";
                                    worksheet.Range(1, 1, 1, 11).Merge();
                                    worksheet.Range(1, 1, rowID, 11).Style.Fill.BackgroundColor = XLColor.GrannySmithApple;
                                    worksheet.Range(1, 1, rowID, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    worksheet.Range(1, 1, 1, 11).Style.Font.FontSize = 14;

                                    worksheet.Cell(rowID, 1).Value = _dataContext.EntityType == "C" ? "Cliente" : "Fornitore";

                                    worksheet.Cell(rowID, 2).Value = "Data registrazione";
                                    worksheet.Cell(rowID, 3).Value = "Anno";
                                    worksheet.Cell(rowID, 4).Value = "Numero";
                                    worksheet.Cell(rowID, 5).Value = "Riga";
                                    worksheet.Cell(rowID, 6).Value = "Importo";
                                    worksheet.Cell(rowID, 7).Value = "Segno";

                                    worksheet.Cell(rowID, 8).Value = "Doc";
                                    worksheet.Cell(rowID, 9).Value = "Doc data";
                                    worksheet.Cell(rowID, 10).Value = "Rif";
                                    worksheet.Cell(rowID, 11).Value = "Rif data";

                                    worksheet.Range(rowID, 1, rowID, 11).SetAutoFilter();
                                    worksheet.Range(rowID, 1, rowID, 11).Style.Fill.BackgroundColor = XLColor.LightGray;

                                    foreach (var ent in _dataContext.Items ?? new System.Collections.ObjectModel.ObservableCollection<Models.Models.Reports.Accounting.BatchReportModel.EntityModel>())
                                    {
                                        ++rowID;

                                        var initialValue = ent.InitialValue;
                                        var initialValueSign = _dataContext.EntityType == "C" ? "D" : "A";

                                        if (initialValue < 0)
                                        {
                                            if (_dataContext.EntityType == "F")
                                            {
                                                initialValue = initialValue * -1;
                                                initialValueSign = "D";
                                            }
                                            else
                                            {
                                                initialValue = initialValue * -1;
                                                initialValueSign = "A";
                                            }
                                        }

                                        worksheet.Cell(rowID, 1).Value = $"{ent.EntityID} - {ent.EntityDescription}";
                                        worksheet.Cell(rowID, 6).Value = initialValue;
                                        worksheet.Cell(rowID, 7).Value = initialValueSign;
                                        worksheet.Range(rowID, 1, rowID, 11).Style.Fill.BackgroundColor = XLColor.LightCyan;

                                        decimal entityTotal = initialValue ?? 0;
                                        var entityTotalSign = initialValueSign;

                                        foreach (var mov in ent.Movements ?? new List<Models.Models.Reports.Accounting.BatchReportModel.MovementModel>())
                                        {
                                            ++rowID;
                                            worksheet.Cell(rowID, 2).Value = mov.Date;
                               
                                            worksheet.Cell(rowID, 3).Value = mov.Year;
                                            worksheet.Cell(rowID, 4).Value = mov.ID;
                                            worksheet.Cell(rowID, 5).Value = mov.Row;
                                            worksheet.Cell(rowID, 6).Value = mov.Import;
                                            worksheet.Cell(rowID, 7).Value = mov.Sign;
                                            worksheet.Cell(rowID, 8).Value = mov.DocumentID;
                                            worksheet.Cell(rowID, 9).Value = mov.DocumentDate;
                                            worksheet.Cell(rowID, 10).Value = mov.ReferenceID;
                                            worksheet.Cell(rowID, 11).Value = mov.ReferenceDate;

                                            if(_dataContext.EntityType == "F")
                                            {
                                                if(entityTotalSign == "A")
                                                {
                                                    entityTotal = mov.Sign == "D" ? entityTotal - mov.Import : entityTotal + mov.Import;

                                                    if(entityTotal < 0)
                                                    {
                                                        entityTotal = entityTotal * -1;
                                                        entityTotalSign = "D";
                                                    }
                                                }
                                                else
                                                {
                                                    entityTotal = mov.Sign == "A" ? entityTotal - mov.Import : entityTotal + mov.Import;

                                                    if (entityTotal < 0)
                                                    {
                                                        entityTotal = entityTotal * -1;
                                                        entityTotalSign = "A";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (entityTotalSign == "D")
                                                {
                                                    entityTotal = mov.Sign == "A" ? entityTotal - mov.Import : entityTotal + mov.Import;

                                                    if (entityTotal < 0)
                                                    {
                                                        entityTotal = entityTotal * -1;
                                                        entityTotalSign = "A";
                                                    }
                                                }
                                                else
                                                {
                                                    entityTotal = mov.Sign == "D" ? entityTotal - mov.Import : entityTotal + mov.Import;

                                                    if (entityTotal < 0)
                                                    {
                                                        entityTotal = entityTotal * -1;
                                                        entityTotalSign = "D";
                                                    }
                                                }
                                            }
                                        }

                                     

                                        ++rowID;
                                        worksheet.Cell(rowID, 1).Value = $"TOTALE - {ent.EntityDescription}";
                                        worksheet.Cell(rowID, 6).Value = entityTotal;
                                        worksheet.Cell(rowID, 7).Value = entityTotalSign;
                                        worksheet.Range(rowID, 1, rowID, 11).Style.Fill.BackgroundColor = XLColor.LightCoral;

                                       
                                    }

                                    worksheet.Column(2).Style.DateFormat.Format = "dd/MM/yyyy";
                                    worksheet.Column(9).Style.DateFormat.Format = "dd/MM/yyyy";
                                    worksheet.Column(11).Style.DateFormat.Format = "dd/MM/yyyy";
                                    worksheet.ColumnsUsed().AdjustToContents();

                                    worksheet.SheetView.FreezeRows(2);
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
                    ErrorHandler.Validation("Selezionare un periodo valido per l'estrazione");
                }
            }
            else
            {
                ErrorHandler.Validation("Selezionare un periodo per l'estrazione");
            }
        }
    }
}
