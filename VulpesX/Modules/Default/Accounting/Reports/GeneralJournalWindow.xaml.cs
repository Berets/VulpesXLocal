using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VulpesX.DAL;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Reports;

namespace VulpesX.Modules.Default.Accounting.Reports
{
    /// <summary>
    /// Interaction logic for GeneralJournalWindow.xaml
    /// </summary>
    public partial class GeneralJournalWindow : FluentDefaultWindow
    {
        private GeneralJournalWindowViewModel _dataContext;

        private static string? printFullPath = null;
        private static int page = 0;
        private static int lastIndex = 0;
        private static int lastY = 22;
        private static int yIncrement = 2;
        private static string fontName = "Courier New";
        private static Font fontNormal = new Font(fontName, 6);
        private static Font fontBold = new Font(fontName, 6, System.Drawing.FontStyle.Bold);
        private static Font fontBoldUnderline = new Font(fontName, 6, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline);

        public GeneralJournalWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<GeneralJournalWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;

            _dataContext.AccountingYear = (cmbAccountingYear.Items[0] as ESERCIZIO)?.eseann;
            _dataContext.IsDefinitive = false;
            _dataContext.PrintUntil = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.PrintUntil.HasValue)
            {
                if (ConfirmHandler.Confirm($"Confermate la stampa {(_dataContext.IsDefinitive ? "DEFINITIVA" : "PROVVISORIA")} del giornale generale ?"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    printFullPath = FileHelper.GeneratePDFFilename($"GiornaleGeneraleAl_{_dataContext.PrintUntil.Value.Date.ToString("dd_MM_yyyy")}.pdf");

                    var reportData = _dataContext.GetGeneralJournalReport();

                    if (reportData != null)
                    {
                        page = reportData.StartingPage > 0 ? reportData.StartingPage + 1 : 1;
                        lastIndex = 0;
                        lastY = 22;

                        try
                        {

                            PrintDocument document = new PrintDocument();
                            document.PrintPage += (sender, args) => OnPrintPage(reportData, args);
                            document.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                            document.PrinterSettings.PrintToFile = true;
                            document.PrinterSettings.PrintFileName = printFullPath;
                            document.Print();

                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Show(ex.ToString());
                        }

                        EnumeratedPrintQueueTypes[] enumerationFlags = { EnumeratedPrintQueueTypes.Local };
                        var queue = new LocalPrintServer().GetPrintQueues(enumerationFlags).Where(w => w.Name == "Microsoft Print to PDF").FirstOrDefault();

                        if (queue != null)
                        {
                            while (queue.GetPrintJobInfoCollection().Count() > 0)
                            {
                                // wait
                            }
                        }

                        Mouse.OverrideCursor = null;
                        var proc = new ProcessStartInfo(printFullPath);
                        proc.UseShellExecute = true;
                        Process.Start(proc);
                        if (_dataContext.IsDefinitive)
                        {
                            // update PNRIGHE
                            _dataContext.UpdateJournalDefinitives(reportData, page);
                        }
                    }
                }
            }
            else
            {
                ErrorHandler.Validation("Tutti le informazioni richieste sono obbligatorie");
            }
        }

        private static void OnPrintPage(GeneralJournalReport Data, PrintPageEventArgs e)
        {
            if (e.Graphics != null)
            {
                e.Graphics.PageUnit = GraphicsUnit.Millimeter;

                #region Page header
                e.Graphics.DrawString(Data.CompanyInfo?.azrssl, new Font(fontName, 8, System.Drawing.FontStyle.Bold), Brushes.Black, 5, 5);
                e.Graphics.DrawString(Data.CompanyInfo?.azinsl, new Font(fontName, 6), Brushes.Black, 5, 8);
                e.Graphics.DrawString($"{Data.CompanyInfo?.azcasl}, {Data.CompanyInfo?.azlosl} ({Data.CompanyInfo?.azprsl})", new Font(fontName, 6), Brushes.Black, 5, 11);
                e.Graphics.DrawString($"P. IVA:{Data.CompanyInfo?.azpaiv} - C.F: {Data.CompanyInfo?.azcofi}", new Font(fontName, 6), Brushes.Black, 5, 14);
                e.Graphics.DrawString("GIORNALE GENERALE", new Font(fontName, 12, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline), Brushes.Black, 85, 5);
                var pageText = $"{Data.AccountingYear}/{page.ToString().PadLeft(5, '0')}";
                e.Graphics.DrawString($"{pageText}", new Font(fontName, 8, System.Drawing.FontStyle.Bold), Brushes.Black, 195 - e.Graphics.MeasureString(pageText, new Font(fontName, 8, System.Drawing.FontStyle.Bold)).Width, 5);
                #endregion

                #region Table header
                e.Graphics.DrawRectangle(new Pen(Color.Black, .1f), 5, 17, 195, 4);
                e.Graphics.DrawString("N. RIGA", fontBold, Brushes.Black, 6, 18);
                e.Graphics.DrawString("DATA E N. REGISTRAZIONE", fontBold, Brushes.Black, 20, 18);
                e.Graphics.DrawString("DATA E N. DOCUMENTO", fontBold, Brushes.Black, 52, 18);
                e.Graphics.DrawString("CONTO", fontBold, Brushes.Black, 82, 18);
                e.Graphics.DrawString("DARE", fontBold, Brushes.Black, 169 - e.Graphics.MeasureString("DARE", fontBold).Width, 18);
                e.Graphics.DrawString("AVERE", fontBold, Brushes.Black, 198 - e.Graphics.MeasureString("AVERE", fontBold).Width, 18);
                #endregion

                #region Rows
                while (lastIndex < Data.Rows?.Count)
                {
                    if (Data.Rows[lastIndex].IsPageTop)
                    {
                        e.Graphics.DrawLine(new Pen(Color.Black, .1f), 5, lastY + 1, (144 - e.Graphics.MeasureString("PROGRESSIVI PRECEDENTI", fontBold).Width) - 1, lastY + 1);
                        e.Graphics.DrawString("PROGRESSIVI PRECEDENTI", fontBold, Brushes.Black, 144 - e.Graphics.MeasureString("PROGRESSIVI PRECEDENTI", fontBold).Width, lastY);
                        var dare = (Data.Rows[lastIndex].DareTop ?? 0).ToString("N2");
                        var avere = (Data.Rows[lastIndex].AvereTop ?? 0).ToString("N2");
                        e.Graphics.DrawString($"{dare}", fontBold, Brushes.Black, 169 - e.Graphics.MeasureString(dare, fontBold).Width, lastY);
                        e.Graphics.DrawString($"{avere}", fontBold, Brushes.Black, 198 - e.Graphics.MeasureString(avere, fontBold).Width, lastY);
                    }
                    else
                    {
                        if (Data.Rows[lastIndex].IsPageBottom)
                        {
                            e.Graphics.DrawLine(new Pen(Color.Black, .1f), 5, lastY + 1, (144 - e.Graphics.MeasureString("PROGRESSIVI GENERALI", fontBold).Width) - 1, lastY + 1);
                            e.Graphics.DrawString("PROGRESSIVI GENERALI", fontBold, Brushes.Black, 144 - e.Graphics.MeasureString("PROGRESSIVI GENERALI", fontBold).Width, lastY);
                            var dare = (Data.Rows[lastIndex].DareBottom ?? 0).ToString("N2");
                            var avere = (Data.Rows[lastIndex].AvereBottom ?? 0).ToString("N2");
                            e.Graphics.DrawString($"{dare}", fontBold, Brushes.Black, 169 - e.Graphics.MeasureString(dare, fontBold).Width, lastY);
                            e.Graphics.DrawString($"{avere}", fontBold, Brushes.Black, 198 - e.Graphics.MeasureString(avere, fontBold).Width, lastY);
                        }
                        else
                        {
                            if (Data.Rows[lastIndex].IsDayTotal)
                            {
                                e.Graphics.DrawLine(new Pen(Color.Black, .1f), 5, lastY + 1, (144 - e.Graphics.MeasureString(Data.Rows[lastIndex].DayBreakText, fontBold).Width) - 1, lastY + 1);
                                e.Graphics.DrawString($"{Data.Rows[lastIndex].DayBreakText}", fontBold, Brushes.Black, 144 - e.Graphics.MeasureString(Data.Rows[lastIndex].DayBreakText, fontBold).Width, lastY);
                                var dare = (Data.Rows[lastIndex].DareDayTotal ?? 0).ToString("N2");
                                var avere = (Data.Rows[lastIndex].AvereDayTotal ?? 0).ToString("N2");
                                e.Graphics.DrawString($"{dare}", fontBold, Brushes.Black, 169 - e.Graphics.MeasureString(dare, fontBold).Width, lastY);
                                e.Graphics.DrawString($"{avere}", fontBold, Brushes.Black, 198 - e.Graphics.MeasureString(avere, fontBold).Width, lastY);
                            }
                            else
                            {
                                e.Graphics.DrawString($"{Data.Rows[lastIndex]?.RowNumber?.ToString().PadLeft(6, '0')}", fontNormal, Brushes.Black, 7, lastY);
                                e.Graphics.DrawString($"{(Data.Rows[lastIndex]?.RegistrationDate ?? DateTime.Now).ToString("dd/MM/yyyy")}", fontNormal, Brushes.Black, 20, lastY);
                                e.Graphics.DrawString($"{Data.Rows[lastIndex].RegistrationNumber}", fontNormal, Brushes.Black, 36, lastY);
                                e.Graphics.DrawString($"{(Data.Rows[lastIndex].DocumentDate.HasValue ? (Data.Rows[lastIndex].DocumentDate ?? DateTime.Now).ToString("dd/MM/yyyy") : "")}", fontNormal, Brushes.Black, 52, lastY);
                                e.Graphics.DrawString($"{Data.Rows[lastIndex].DocumentNumber}", fontNormal, Brushes.Black, 69, lastY);
                                e.Graphics.DrawString($"{Data.Rows[lastIndex].GroupID}", fontNormal, Brushes.Black, 82, lastY);
                                e.Graphics.DrawString($"{Data.Rows[lastIndex].AccountID}", fontNormal, Brushes.Black, 87, lastY);
                                e.Graphics.DrawString($"{Data.Rows[lastIndex].SubaccountID}", fontNormal, Brushes.Black, 92, lastY);
                                var dare = (Data.Rows[lastIndex].DareAmount ?? 0).ToString("N2").Trim();
                                var avere = (Data.Rows[lastIndex].AvereAmount ?? 0).ToString("N2").Trim();
                                e.Graphics.DrawString($"{dare}", fontNormal, Brushes.Black, 169 - e.Graphics.MeasureString(dare, fontNormal).Width, lastY);
                                e.Graphics.DrawString($"{avere}", fontNormal, Brushes.Black, 198 - e.Graphics.MeasureString(avere, fontNormal).Width, lastY);
                                lastY += yIncrement;
                                e.Graphics.DrawString($"Causale", fontBold, Brushes.Black, 9, lastY);
                                e.Graphics.DrawString($"{Data.Rows[lastIndex].CausalFullDescription}", fontNormal, Brushes.Black, 20, lastY);
                                e.Graphics.DrawString($"{Data.Rows[lastIndex].RowDescription}", fontNormal, Brushes.Black, 82, lastY);
                                if (!string.IsNullOrWhiteSpace(Data.Rows[lastIndex].Note))
                                {
                                    lastY += yIncrement;
                                    e.Graphics.DrawString($"Note", fontBold, Brushes.Black, 13, lastY);
                                    e.Graphics.DrawString($"{Data.Rows[lastIndex].Note}", fontNormal, Brushes.Black, 20, lastY);
                                }
                            }
                        }
                    }
                    if (Data.Rows[lastIndex].IsPageBottom)
                    {
                        lastIndex++;
                        e.HasMorePages = true;
                        break;
                    }
                    else
                    {
                        lastY += yIncrement;
                        lastIndex++;
                    }
                }
                #endregion

                if (lastIndex == Data.Rows?.Count)
                {
                    e.HasMorePages = false;
                }
                else
                {
                    e.HasMorePages = true;
                    page++;
                    lastY = 22;
                }
            }
        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.AccountingYear = (cmbAccountingYear.SelectedItem as ESERCIZIO)?.eseann;
        }
    }
}
