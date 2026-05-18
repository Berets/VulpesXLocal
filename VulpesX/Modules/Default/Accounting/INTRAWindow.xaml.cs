using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
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
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for INTRAWindow.xaml
    /// </summary>
    public partial class INTRAWindow : FluentDefaultWindow
    {
        private INTRAWindowViewModel _dataContext;
        public INTRAWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<INTRAWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            rdtPeriod.Culture = new System.Globalization.CultureInfo("it-IT");
            rdtPeriod.Culture.DateTimeFormat.ShortDatePattern = "MMMM yyyy";

            _dataContext.Month = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
        }

        private async void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.Month.HasValue)
            {
                var list = await _dataContext.GetINTRAS();

                if (list != null && list.Any())
                {
                    Microsoft.Win32.SaveFileDialog sfdExcel = new Microsoft.Win32.SaveFileDialog();
                    sfdExcel.Filter = "Excel |*.xlsx";
                    sfdExcel.ShowDialog();

                    if (!string.IsNullOrEmpty(sfdExcel.FileName))
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var rowID = 1;

                            var azienda = _dataContext.GetAZIENDA();

                            #region BIS
                            var worksheetName = "Modello INTRA_Bis";
                            var wsBis = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                            wsBis.Cell(rowID, 1).Value = "Tipo riepilogo";
                            wsBis.Cell(rowID, 2).Value = "Tipo periodo";
                            wsBis.Cell(rowID, 3).Value = "Mese o Trimestre";
                            wsBis.Cell(rowID, 4).Value = "Anno";
                            wsBis.Cell(rowID, 5).Value = "Partita IVA Contribuente";
                            wsBis.Cell(rowID, 6).Value = "Stato";
                            wsBis.Cell(rowID, 7).Value = "Codice IVA";
                            wsBis.Cell(rowID, 8).Value = "Ammontare delle operazioni in Euro";
                            wsBis.Cell(rowID, 9).Value = "Ammontare delle operazioni in Valuta";
                            wsBis.Cell(rowID, 10).Value = "Natura transazione A";
                            wsBis.Cell(rowID, 11).Value = "Natura transazione B";
                            wsBis.Cell(rowID, 12).Value = "Nomenclatura combinata";
                            wsBis.Cell(rowID, 13).Value = "Massa netta";
                            wsBis.Cell(rowID, 14).Value = "Unita supplementari";
                            wsBis.Cell(rowID, 15).Value = "Valore statistico in Euro";
                            wsBis.Cell(rowID, 16).Value = "Condizioni di consegna";
                            wsBis.Cell(rowID, 17).Value = "Modo di trasporto";
                            wsBis.Cell(rowID, 18).Value = "Paese di destinazione -  Mod. INTRA 1 Bis";
                            wsBis.Cell(rowID, 19).Value = "Provincia di origine -  Mod. INTRA 1 Bis";
                            wsBis.Cell(rowID, 20).Value = "Paese di provenienza  - Mod. INTRA 2 Bis";
                            wsBis.Cell(rowID, 21).Value = "Paese di origine";
                            wsBis.Cell(rowID, 22).Value = "Provincia di destinazione - Mod. INTRA 2 Bis";
                            wsBis.Range(rowID, 1, rowID, 22).Style.Fill.BackgroundColor = XLColor.LightGray;

                            foreach (var inv in list.Where(o => o.Causal?.fattipo == "V"))
                            {
                                ++rowID;

                                wsBis.Cell(rowID, 1).Value = "0";
                                wsBis.Cell(rowID, 2).Value = "1";
                                wsBis.Cell(rowID, 3).Value = _dataContext.Month.Value.Month;
                                wsBis.Cell(rowID, 4).Value = _dataContext.Month.Value.Year;
                                wsBis.Cell(rowID, 5).Value = azienda?.azpaiv;
                                wsBis.Cell(rowID, 6).Value = inv.Customer?.nazcod?.Trim();
                                wsBis.Cell(rowID, 7).Value = inv.Customer?.abepiv?.TrimEnd();
                                wsBis.Cell(rowID, 8).Value = inv.GrandTotal + inv.FTTRAS;
                                wsBis.Cell(rowID, 9).Value = 0;
                                wsBis.Cell(rowID, 10).Value = "1";
                                wsBis.Cell(rowID, 11).Value = "1";
                                wsBis.Cell(rowID, 12).Value = "82079078";
                                wsBis.Cell(rowID, 13).Value = Math.Ceiling(inv.Rows?.FirstOrDefault()?.LinkedDDT?.BTPES2 ?? 0);
                                wsBis.Cell(rowID, 14).Value = inv.Rows?.Sum(s => s.FDQTAV ?? 0);
                                wsBis.Cell(rowID, 15).Value = inv.GrandTotal + inv.FTTRAS;
                                wsBis.Cell(rowID, 16).Value = "F";
                                wsBis.Cell(rowID, 17).Value = "4";
                                wsBis.Cell(rowID, 18).Value = inv.Customer?.nazcod?.Trim();
                                wsBis.Cell(rowID, 19).Value = azienda?.azprsa;
                                wsBis.Cell(rowID, 20).Value = " ";
                                wsBis.Cell(rowID, 21).Value = "IT";
                                wsBis.Cell(rowID, 22).Value = " ";
                            }

                            wsBis.ColumnsUsed().AdjustToContents();
                            #endregion

                            #region TER
                            worksheetName = "Modello INTRA_Ter";
                            var wsTer = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                            rowID = 1;

                            wsTer.Cell(rowID, 1).Value = "Tipo riepilogo";
                            wsTer.Cell(rowID, 2).Value = "Tipo periodo";
                            wsTer.Cell(rowID, 3).Value = "Mese o Trimestre";
                            wsTer.Cell(rowID, 4).Value = "Anno";
                            wsTer.Cell(rowID, 5).Value = "Partita IVA Contribuente";
                            wsTer.Cell(rowID, 6).Value = "Mese da rettificare";
                            wsTer.Cell(rowID, 7).Value = "Trimestre da rettificare";
                            wsTer.Cell(rowID, 8).Value = "Anno di riferimento del periodo da rettificare";
                            wsTer.Cell(rowID, 9).Value = "Stato";
                            wsTer.Cell(rowID, 10).Value = "Codice IVA";
                            wsTer.Cell(rowID, 11).Value = "Segno";
                            wsTer.Cell(rowID, 12).Value = "Ammontare delle operazioni in Euro";
                            wsTer.Cell(rowID, 13).Value = "Ammontare delle operazioni in valuta - Mod. INTRA 2 Ter";
                            wsTer.Cell(rowID, 14).Value = "Natura transazione A";
                            wsTer.Cell(rowID, 15).Value = "Nomenclatura combinata";
                            wsTer.Cell(rowID, 16).Value = "Valore statistico in euro";
                            wsTer.Range(rowID, 1, rowID, 16).Style.Fill.BackgroundColor = XLColor.LightGray;

                            wsTer.ColumnsUsed().AdjustToContents();
                            #endregion

                            #region QUATER
                            worksheetName = "Modello INTRA_Quater";
                            var wsQuater = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                            rowID = 1;

                            wsQuater.Cell(rowID, 1).Value = "Tipo riepilogo";
                            wsQuater.Cell(rowID, 2).Value = "Tipo periodo";
                            wsQuater.Cell(rowID, 3).Value = "Mese o Trimestre";
                            wsQuater.Cell(rowID, 4).Value = "Anno";
                            wsQuater.Cell(rowID, 5).Value = "Partita IVA Contribuente";
                            wsQuater.Cell(rowID, 6).Value = "Stato";
                            wsQuater.Cell(rowID, 7).Value = "Codice IVA";
                            wsQuater.Cell(rowID, 8).Value = "Ammontare delle operazioni in Euro";
                            wsQuater.Cell(rowID, 9).Value = "Ammontare delle operazioni in valuta - Mod. INTRA 2 Quater";
                            wsQuater.Cell(rowID, 10).Value = "Riferimento fattura  - Numero";
                            wsQuater.Cell(rowID, 11).Value = "Riferimento fattura  - Data (GGMMAAAA)";
                            wsQuater.Cell(rowID, 12).Value = "Codice servizio";
                            wsQuater.Cell(rowID, 13).Value = "Modalità di erogazione";
                            wsQuater.Cell(rowID, 14).Value = "Modalità di incasso";
                            wsQuater.Cell(rowID, 15).Value = "Paese di pagamento";
                            wsQuater.Range(rowID, 1, rowID, 15).Style.Fill.BackgroundColor = XLColor.LightGray;

                            foreach (var inv in list.Where(o => o.Causal?.fattipo == "C"))
                            {
                                ++rowID;

                                wsQuater.Cell(rowID, 1).Value = "0";
                                wsQuater.Cell(rowID, 2).Value = "1";
                                wsQuater.Cell(rowID, 3).Value = _dataContext.Month.Value.Month;
                                wsQuater.Cell(rowID, 4).Value = _dataContext.Month.Value.Year;
                                wsQuater.Cell(rowID, 5).Value = azienda?.azpaiv;
                                wsQuater.Cell(rowID, 6).Value = inv.Customer?.nazcod?.Trim();
                                wsQuater.Cell(rowID, 7).Value = inv.Customer?.abepiv?.TrimEnd();
                                wsQuater.Cell(rowID, 8).Value = inv.GrandTotal + inv.FTTRAS;
                                wsQuater.Cell(rowID, 9).Value = 0;
                                wsQuater.Cell(rowID, 10).Value = inv.FTNUFD;
                                wsQuater.Cell(rowID, 11).Value = inv.FTDAOR?.ToString("dMMyyyy");
                                wsQuater.Cell(rowID, 12).Value = "25622";
                                wsQuater.Cell(rowID, 13).Value = " ";
                                wsQuater.Cell(rowID, 14).Value = " ";
                                wsQuater.Cell(rowID, 15).Value = inv.Customer?.nazcod?.Trim();
                            }

                            wsQuater.ColumnsUsed().AdjustToContents();
                            #endregion

                            #region QUIN
                            worksheetName = "Modello INTRA_Quinquies";
                            var wQuin = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                            rowID = 1;

                            wQuin.Cell(rowID, 1).Value = "Tipo riepilogo";
                            wQuin.Cell(rowID, 2).Value = "Tipo periodo";
                            wQuin.Cell(rowID, 3).Value = "Mese o Trimestre";
                            wQuin.Cell(rowID, 4).Value = "Anno";
                            wQuin.Cell(rowID, 5).Value = "Partita IVA Contribuente";
                            wQuin.Cell(rowID, 6).Value = "Sezione 3 da rettificare - Sezione Doganale";
                            wQuin.Cell(rowID, 7).Value = "Sezione 3 da rettificare - Anno";
                            wQuin.Cell(rowID, 8).Value = "Sezione 3 da rettificare - Protocollo dichiarazione";
                            wQuin.Cell(rowID, 9).Value = "Sezione 3 da rettificare - Prog. Sez. 3";
                            wQuin.Cell(rowID, 10).Value = "Stato";
                            wQuin.Cell(rowID, 11).Value = "Codice IVA";
                            wQuin.Cell(rowID, 12).Value = "Ammontare delle operazioni in Euro";
                            wQuin.Cell(rowID, 13).Value = "Ammontare delle operazioni in valuta - Mod. INTRA 2 Quinquies";
                            wQuin.Cell(rowID, 14).Value = "Riferimento fattura  - Numero";
                            wQuin.Cell(rowID, 15).Value = "Riferimento fattura  - Data (GGMMAAAA)";
                            wQuin.Cell(rowID, 16).Value = "Codice servizio";
                            wQuin.Cell(rowID, 17).Value = "Modalità di erogazione";
                            wQuin.Cell(rowID, 18).Value = "Modalità di incasso";
                            wQuin.Cell(rowID, 19).Value = "Paese di pagamento";
                            wQuin.Range(rowID, 1, rowID, 19).Style.Fill.BackgroundColor = XLColor.LightGray;

                            wQuin.ColumnsUsed().AdjustToContents();
                            #endregion

                            #region QUIN
                            worksheetName = "Modello INTRA_Sexies";
                            var wSex = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

                            rowID = 1;

                            wSex.Cell(rowID, 1).Value = "Tipo riepilogo";
                            wSex.Cell(rowID, 2).Value = "Tipo periodo";
                            wSex.Cell(rowID, 3).Value = "Mese o Trimestre";
                            wSex.Cell(rowID, 4).Value = "Anno";
                            wSex.Cell(rowID, 5).Value = "Partita IVA Contribuente";
                            wSex.Cell(rowID, 6).Value = "Stato nuovo destinatario";
                            wSex.Cell(rowID, 7).Value = "Codice IVA destinatario";
                            wSex.Cell(rowID, 8).Value = "Tipo operazione";
                            wSex.Cell(rowID, 9).Value = " Stato nuovo destinatario";
                            wSex.Cell(rowID, 10).Value = "Codice IVA nuovo destinatario";
                            wSex.Range(rowID, 1, rowID, 10).Style.Fill.BackgroundColor = XLColor.LightGray;

                            wSex.ColumnsUsed().AdjustToContents();
                            #endregion


                            workbook.SaveAs(sfdExcel.FileName);

                            if (System.IO.File.Exists(sfdExcel.FileName))
                                Process.Start(@"cmd.exe ", @"/c" + sfdExcel.FileName);
                        }
                    }

                    this.Close();
                }
                else
                {
                    ErrorHandler.Validation($"Nessun dato da estrarre per - {_dataContext.Month.Value.ToString("MM-yyyy")}");
                }
            }
            else
            {
                ErrorHandler.Validation("Selezionare il mese da estrarre");
            }
        }

    }
}
