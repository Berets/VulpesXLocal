using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Telerik.Windows.Data;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.Commons;
using static VulpesX.Models.Models.Reports.Accounting.MastrinoReport;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for ECView.xaml
    /// </summary>
    public partial class ECView : UserControl
    {
        private ECViewModel _dataContext;
        public ECView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ECViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            var today = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            _dataContext.HasDrawn = false;
            _dataContext.EntityType = "C";
            _dataContext.ToDate = today;
            _dataContext.SinceDrawnDate = today;

            this.PreviewKeyDown += async (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    await LoadData();

                }
            };
            this.Loaded += async (s, e) =>
            {
                await LoadData();

                ECDropHandler.SyncECAction = async (entity, companyid, sourceyear, sourceid, sourcerow, targetyear, targetid, targetrow) =>
                {
                    var result = _dataContext.SyncEC(entity, companyid, sourceyear, sourceid, sourcerow, targetyear, targetid, targetrow);

                    if (result)
                        await LoadData();

                    return result;
                };
            };
            this.Unloaded += (s, e) =>
            {
                ECDropHandler.SyncECAction = null;
            };

            GridView.DataLoaded += (s, e) =>
            {
                rmiPrint.IsEnabled = _dataContext.Items != null && _dataContext.Items.Count > 0;

                if (_dataContext.Items?.Where(o => (o.ValoreValuta ?? 0) > 0).Any() ?? false)
                {
                    GridView.Columns["colValValuta"].IsVisible = true;
                    GridView.Columns["colValCambio"].IsVisible = true;
                    GridView.Columns["colValValoreValuta"].IsVisible = true;
                }
                else
                {
                    GridView.Columns["colValValuta"].IsVisible = false;
                    GridView.Columns["colValCambio"].IsVisible = false;
                    GridView.Columns["colValValoreValuta"].IsVisible = false;
                }

                GridView.ExpandAllGroups();
                //cmdAnalysis.IsEnabled = _dataContext.Items != null && _dataContext.Items.Count > 0 && _dataContext.EntityType == "C";
            };
        }

        private async Task LoadData()
        {
            if (!string.IsNullOrWhiteSpace(_dataContext.EntityType) && _dataContext.SelectedEntity != null && _dataContext.ToDate.HasValue)
            {
                await _dataContext.Load();

                GridView.GroupDescriptors.Clear();
                GridView.GroupDescriptors.Add(new GroupDescriptor() { Member = "Partita", SortDirection = null });

                _dataContext.SelectedItems = new System.Collections.ObjectModel.ObservableCollection<MastrinoECReportItem>();
            }
        }

        #region Filters
        private async void rdtLimitDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadData();
        }

        private void cmbEntityType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var oldValue = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as GenericIDDescription : null;
                var newValue = e.AddedItems[0] as GenericIDDescription;

                if (newValue != null)
                {
                    if ((oldValue != null && oldValue.ID != newValue.ID) || oldValue == null)
                    {
                        _dataContext.SelectedEntity = null;
                        _dataContext.Codes = _dataContext.GetABEs(newValue.ID!);
                        _dataContext.Items = null;
                    }
                }
            }
        }

        private async void acCode_LostFocus(object sender, RoutedEventArgs e)
        {
            var autoCompleteBox = sender as RadAutoCompleteBox;

            if (autoCompleteBox != null)
            {
                if (autoCompleteBox.SelectedItem == null)
                {
                    autoCompleteBox.SearchText = null;
                }
            }

            await LoadData();
        }

        private async void rdtDrawnLimitDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadData();
        }

        private async void chkOpen_Checked(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }

        private async void chkOpen_Unchecked(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }

        private async void chkDrawn_Checked(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }

        private async void chkDrawn_Unchecked(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }
        #endregion

        #region Context menu
        private async void rmiChangeRef_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var ec = GridView.SelectedItem as MastrinoECReportItem;
            if (ec != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ECChangeRefWindowViewModel>();
                windowViewModel.EC = ec;

                var wChangeRef = new ECChangeRefWindow(windowViewModel);
                wChangeRef.Owner = Window.GetWindow(this);
                if (wChangeRef.ShowDialog() == true)
                    await LoadData();
            }
        }

        private async void rmiLock_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = GridView.SelectedItem as MastrinoECReportItem;

            if (selected != null)
            {
                if (string.IsNullOrWhiteSpace(selected.LockedInfoText))
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CancelReasonWindowViewModel>();

                    var wAsk = new CancelReasonWindow(windowViewModel);
                    wAsk.Owner = Window.GetWindow(this);
                    if (wAsk.ShowDialog() == true)
                    {
                        if (_dataContext.Lock(selected, windowViewModel.SelectedReason ?? string.Empty))
                            await LoadData();
                    }
                }
                else
                {
                    if (ConfirmHandler.Confirm("Confermate lo sblocco del pagamento ?"))
                    {
                        if (_dataContext.Unlock(selected))
                            await LoadData();
                    }
                }
            }
        }

        private void cmGrid_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItem != null)
            {
                var selected = GridView.SelectedItem as MastrinoECReportItem;

                rmiChangeRef.IsEnabled = GridView.SelectedItem != null;

                if (selected != null)
                {
                    if (_dataContext.EntityType == "F")
                    {
                        if (string.IsNullOrWhiteSpace(selected.LockedInfoText))
                        {
                            rmiLock.Header = "Blocca il pagamento";
                        }
                        else
                        {
                            rmiLock.Header = $"Clicca per sbloccare > {selected.LockedInfoText.Trim()}";
                        }
                    }
                    else
                    {
                        rmiLock.IsEnabled = false;
                        rmiLock.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }
        #endregion

        #region Buttons
        private void cmdAnalysis_Click(object sender, RoutedEventArgs e)
        {
            //wCustomerAnalysis wCA = new wCustomerAnalysis(ctx, new CustomerAnalysisViewModel() { CustomerID = model.SelectedEntity.abecod, CustomerDescription = model.SelectedEntity.FullDescriptionSearchable, ToDate = model.ToDate.Value });
            //wCA.Owner = Window.GetWindow(this);
            //wCA.ShowDialog();
        }

        private void rmiPrint_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.ToDate.HasValue && _dataContext.SelectedEntity != null)
            {
                var reportData = new ECReport()
                {
                    CompanyInfo = _dataContext.GetAZIENDA(),
                    //LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{ctx.CurrentCompany.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png"),
                    ToDate = _dataContext.ToDate.Value,
                    Entity = _dataContext.GetABE(_dataContext.SelectedEntity.abecod),
                    ReportTitle = _dataContext.EntityType == "C" ? "ESTRATTO CONTO CLIENTE" : "ESTRATTO CONTO FORNITORE",
                    EntityDescription = _dataContext.EntityType == "C" ? "Cliente" : "Fornitore",
                    IstantText = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime().ToString("dd/MM/yyyy HH:mm:ss"),
                    Rows = _dataContext.Items
                };
                ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_EC, _dataContext.CompanyID, reportData, $"EC {_dataContext.ToDate.Value.ToString("dd_MM_yyyy")}", reportData.PrintFilename, true);
            }
        }

        private void rmiExcel_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

                    worksheet.Cell(1, 1).Value = $"Estratto conto {(_dataContext.EntityType == "C" ? "clienti" : "fornitori")} - {_dataContext.SelectedEntity?.abers1}";
                    worksheet.Range(1, 1, 1, 12).Merge();
                    worksheet.Range(1, 1, rowID, 12).Style.Fill.BackgroundColor = XLColor.GrannySmithApple;
                    worksheet.Range(1, 1, rowID, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(1, 1, 1, 12).Style.Font.FontSize = 14;

                    worksheet.Cell(rowID, 1).Value = "N°";
                    worksheet.Cell(rowID, 2).Value = "Data registrazione";
                    worksheet.Cell(rowID, 3).Value = "Documento";
                    worksheet.Cell(rowID, 4).Value = "Data documento";
                    worksheet.Cell(rowID, 5).Value = "Riferimento";
                    worksheet.Cell(rowID, 6).Value = "Data riferimento";
                    worksheet.Cell(rowID, 7).Value = "Scadenza";

                    worksheet.Cell(rowID, 8).Value = "Causale";
                    worksheet.Cell(rowID, 9).Value = "Val";
                    worksheet.Cell(rowID, 10).Value = "Dare";
                    worksheet.Cell(rowID, 11).Value = "Avere";
                    worksheet.Cell(rowID, 12).Value = "Saldo";
                    worksheet.Range(rowID, 1, rowID, 12).SetAutoFilter();
                    worksheet.Range(rowID, 1, rowID, 12).Style.Fill.BackgroundColor = XLColor.LightGray;

                    foreach (var reg in _dataContext.Items ?? new System.Collections.ObjectModel.ObservableCollection<MastrinoECReportItem>())
                    {
                        ++rowID;
                        worksheet.Cell(rowID, 1).Value = reg.Number;
                        worksheet.Cell(rowID, 2).Value = reg.RegistrationDate;

                        worksheet.Cell(rowID, 3).Value = reg.DocumentID;
                        worksheet.Cell(rowID, 4).Value = reg.DocumentDate;
                        worksheet.Cell(rowID, 5).Value = reg.ReferenceID;
                        worksheet.Cell(rowID, 6).Value = reg.ReferenceDate;
                        worksheet.Cell(rowID, 7).Value = reg.ExpireDate;
                        worksheet.Cell(rowID, 8).Value = reg.CausalFullDescription;
                        worksheet.Cell(rowID, 9).Value = reg.CurrencyID;
                        worksheet.Cell(rowID, 10).Value = reg.Dare;
                        worksheet.Cell(rowID, 11).Value = reg.Avere;
                        worksheet.Cell(rowID, 12).Value = reg.SaldoRiga;
                    }

                    worksheet.Column(4).Style.DateFormat.Format = "dd/MM/yyyy";
                    worksheet.Column(6).Style.DateFormat.Format = "dd/MM/yyyy";
                    worksheet.Column(7).Style.DateFormat.Format = "dd/MM/yyyy";
                    worksheet.ColumnsUsed().AdjustToContents();

                    worksheet.SheetView.FreezeRows(2);
                    workbook.SaveAs(sfdExcel.FileName);

                    if (System.IO.File.Exists(sfdExcel.FileName))
                        FileHelper.Open(sfdExcel.FileName);

                }
            }
        }
        #endregion

        #region Grid actions
        private async void cmdEditRegRows_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as MastrinoECReportItem;

            if (item != null)
            {
                var head = _dataContext.GetPNTESTATA(item.Year, item.Number);
                var causals = _dataContext.GetCAUCONT();

                if (head != null)
                {
                    var codes = !string.IsNullOrWhiteSpace(head.N1FLCF) ? _dataContext.GetABEs(head.N1FLCF) : null;

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNRIGHEWindowViewModel>();
                    windowViewModel.Head = head;
                    windowViewModel.HeadSelectedCausal = causals?.Where(w => w.caucod == head.pncaus).FirstOrDefault();
                    windowViewModel.IsInsert = false;

                    var wPNRIGHE = new PNRIGHEWindow(windowViewModel);
                    wPNRIGHE.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wPNRIGHE.ShowDialog() == true)
                        await LoadData();
                }
            }
        }

        private async void cmdEditRegHead_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as MastrinoECReportItem;

            if (item != null)
            {

                var head = _dataContext.GetPNTESTATA(item.Year, item.Number);
                var causals = _dataContext.GetCAUCONT();

                if (head != null)
                {
                    var codes = !string.IsNullOrWhiteSpace(head.N1FLCF) ? _dataContext.GetABEs(head.N1FLCF) : null;

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNTESTATAWindowViewModel>();
                    windowViewModel.CompanyID = _dataContext.CompanyID;
                    windowViewModel.Data = head;
                    windowViewModel.IsInsert = false;
                    windowViewModel.IsReadonly = _dataContext.IsReadOnly(head.N1ANNO, head.N1REGI);
                    windowViewModel.Causals = causals;
                    windowViewModel.SelectedCausal = causals?.Where(w => w.caucod == head.pncaus).FirstOrDefault();
                    windowViewModel.Codes = !string.IsNullOrWhiteSpace(head.N1FLCF) ? codes : null;
                    windowViewModel.SelectedEntity = !string.IsNullOrWhiteSpace(head.N1FLCF) ? codes?.Where(w => w.abecod == head.N1CLFO).FirstOrDefault() : null;

                    var wPNTESTATA = new PNTESTATAWindow(windowViewModel);
                    wPNTESTATA.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wPNTESTATA.ShowDialog() == true)
                        await LoadData();
                }
            }
        }
        #endregion


        private void chkGroup_Checked(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;

            if (check != null)
            {
                var group = check.DataContext as GroupViewModel;

                if (group != null)
                {
                    var references = group.Group.Items.Cast<MastrinoECReportItem>().GroupBy(g => new { ID = g.ReferenceID, Date = g.ReferenceDate });

                    foreach (var rf in references)
                    {
                        var first = rf.First();

                        if (_dataContext.SelectedItems == null)
                            _dataContext.SelectedItems = new System.Collections.ObjectModel.ObservableCollection<MastrinoECReportItem>();

                        _dataContext.SelectedItems.Add(new MastrinoECReportItem
                        {
                            CompanyID = first.CompanyID,
                            EntityType = first.EntityType,
                            Avere = rf.Sum(s => s.Avere),
                            CausalFullDescription = first.CausalFullDescription,
                            CurrencyID = first.CurrencyID,
                            Dare = rf.Sum(s => s.Dare),
                            DocumentDate = first.DocumentDate,
                            DocumentID = first.DocumentID,
                            EntityID = first.EntityID,
                            ExpireDate = first.ExpireDate,
                            LockedInfoText = first.LockedInfoText,
                            Note = first.Note,
                            Details = first.Details,
                            PartitaPrint = first.PartitaPrint,
                            PaymentID = first.PaymentID,
                            PaymentTypeID = first.PaymentTypeID,
                            ReferenceDate = first.ReferenceDate,
                            ReferenceID = first.ReferenceID,
                            RegistrationDate = first.RegistrationDate,
                            Reminders = first.Reminders,
                            RowID = first.RowID,
                            Scadenza2 = first.Scadenza2,
                            TypeID = first.TypeID,
                            Number = first.Number,
                            Valore = first.Valore,
                            Year = first.Year,
                        });
                    }
                }
            }
        }

        private void chkGroup_Unchecked(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;

            if (check != null)
            {
                var group = check.DataContext as GroupViewModel;

                if (group != null)
                {
                    var references = group.Group.Items.Cast<MastrinoECReportItem>().GroupBy(g => new { ID = g.ReferenceID, Date = g.ReferenceDate });

                    foreach (var rf in references)
                    {
                        if (_dataContext.SelectedItems == null)
                            _dataContext.SelectedItems = new System.Collections.ObjectModel.ObservableCollection<MastrinoECReportItem>();

                        var selected = _dataContext.SelectedItems.Where(o => o.ReferenceID == rf.Key.ID && o.ReferenceDate == rf.Key.Date).FirstOrDefault();

                        if (selected != null)
                            _dataContext.SelectedItems.Remove(selected);
                    }
                }
            }
        }


        private async void cmdAccountingRegistration_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.SelectedEntity != null && !string.IsNullOrEmpty(_dataContext.EntityType))
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ECAccountingRegistrationWindowViewModel>();
                windowViewModel.EntityType = _dataContext.EntityType;
                windowViewModel.Entity = _dataContext.SelectedEntity;
                windowViewModel.Items = _dataContext.SelectedItems;

                var window = new ECAccountingRegistrationWindow(windowViewModel);
                window.ShowDialog();

                await LoadData();
            }
        }

        private async void rgPrint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)!.DataContext as MastrinoECReportItem;
            if (item != null && !string.IsNullOrEmpty(item.InvoiceFile))
            {
                var fileData = await StorageHelperCustom.Ufp.DownloadAsync(StorageHelperCustom.Ufp.CLIENTI_FOLDER, item.InvoiceFile);

                if (fileData != null)
                {
                    string fullPath = Path.Combine(Path.GetTempPath(), item.InvoiceFile);

                    File.WriteAllBytes(fullPath, fileData);

                    if (File.Exists(fullPath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = fullPath,
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        ErrorHandler.Validation($"Non trovato file - {fullPath}");
                    }
                }
                else
                {
                    ErrorHandler.Validation($"Non trovate fatture archiviate - {item.InvoiceFile}");
                }
            }
            else
            {
                ErrorHandler.Validation($"Non trovate fatture archiviate - {item.InvoiceFile}");
            }
        }
    }
}
