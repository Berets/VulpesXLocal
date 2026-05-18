using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls.Filtering.Editors;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Controls.Utilities;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.CRM;
using VulpesX.ViewModels.Modules.Default.General;
using VulpesX.WindowsFactory.Default.General;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for BOLLT00FView.xaml
    /// </summary>
    public partial class BOLLT00FView : UserControl
    {
        private BOLLT00FViewModel _dataContext;

        private List<GenericIDDescription> _currentSort = new List<GenericIDDescription>();
        private List<FilterEntry> _currentWhere = new List<FilterEntry>();

        public BOLLT00FView(string EntityType)
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<BOLLT00FViewModel>();
            _dataContext.EntityType = EntityType;
            _dataContext.Year = DateTime.Now;
            _dataContext.PageSize = 20;
            _dataContext.PageRequested = 0;

            InitializeComponent();

            this.DataContext = _dataContext;

            var customization = _dataContext.GetAZIENDA();
            rmiImpGILAT.Visibility = (customization?.azimpgilat ?? false) ? Visibility.Visible : Visibility.Collapsed;
            rmiImpBANCOLAT.Visibility = (customization?.azimpbancolat ?? false) ? Visibility.Visible : Visibility.Collapsed;

            dtpYear.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpYear.Culture.DateTimeFormat.ShortDatePattern = "yyyy";
            dtpYear.SelectedValue = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            cmbStatus.ItemsSource = CommonsService.DDTStatuses;
            cmbStatus.SelectedItem = cmbStatus.Items[0];

            cmdInvoice.Visibility = _dataContext.EntityType == "C" ? Visibility.Visible : Visibility.Collapsed;
            rddbImport.IsEnabled = _dataContext.EntityType == "C";
            rddbImport.Visibility = _dataContext.EntityType == "C" ? Visibility.Visible : Visibility.Collapsed;

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                }
            };

            GridView.Filtering += (s, e) =>
            {
                e.Cancel = true;

                TelerikControls.FilterManager(_currentWhere, e);

                LoadData();

                e.Cancel = false;
            };
            GridView.Sorting += (s, e) =>
            {
                if (e.OldSortingState == SortingState.None)
                {
                    e.NewSortingState = SortingState.Ascending;
                }
                else if (e.OldSortingState == SortingState.Ascending)
                {
                    e.NewSortingState = SortingState.Descending;
                }
                else
                {
                    e.NewSortingState = SortingState.None;
                }

                TelerikControls.SortManager(_currentSort, e);

                LoadData();

                e.Cancel = true;
            };
            GridView.FieldFilterEditorCreated += (s, e) =>
            {
                var stringFilterEditor = e.Editor as StringFilterEditor;

                if (stringFilterEditor != null)
                {
                    stringFilterEditor.MatchCaseVisibility = Visibility.Collapsed;
                }
            };
            GridView.FilterOperatorsLoading += (s, e) =>
            {
                TelerikControls.CleanFilters(e);
            };
            GridView.SelectionChanged += (s, e) =>
            {
                cmdInvoice.IsEnabled = _dataContext.EntityType == "C" && GridView.SelectedItems != null && GridView.SelectedItems.Count > 0 && GridView.SelectedItems.Cast<BOLLT00F>().All(all => all.BTSTATO == "R");
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _dataContext.PageRequested = e.NewPageIndex;
                LoadData();
            };

            LoadData();
        }

        private async void LoadData()
        {
            await _dataContext.Load(_currentSort, _currentWhere);
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as BOLLT00F;

            if (item != null)
            {
                item = _dataContext.GetFull(item.BTANNO, item.BTBOLL);

                if (item != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BOLLT00FWindowViewModel>();
                    windowViewModel.Data = item;
                    windowViewModel.IsInsert = false;

                    var wBOLLT00F = new BOLLT00FWindow(windowViewModel);
                    wBOLLT00F.Owner = Window.GetWindow(this);
                    if (wBOLLT00F.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as BOLLT00F;

            if (item != null)
            {
                item = _dataContext.GetFull(item.BTANNO, item.BTBOLL);

                if (item != null)
                {
                    if (item.BTNUBD == 0)
                    {
                        if (ConfirmHandler.Confirm($"Confermate l'eliminazione del DDT n.{item.PrintFullID} ?"))
                        {
                            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CancelReasonWindowViewModel>();

                            var wAsk = new CancelReasonWindow(windowViewModel);
                            wAsk.Owner = Window.GetWindow(this);

                            if (wAsk.ShowDialog() == true && !string.IsNullOrEmpty(windowViewModel.SelectedReason))
                            {
                                Mouse.OverrideCursor = Cursors.Wait;
                                _dataContext.CancelAndFreeLinkedOrders(item, windowViewModel.SelectedReason);
                                Mouse.OverrideCursor = null;
                            }
                        }
                        LoadData();
                    }
                    else
                    {
                        ErrorHandler.Validation("Impossibile annullare un DDT stampato definitivamente");
                    }
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            int year = _dataContext.Year.Year;

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BOLLT00FWindowViewModel>();
            windowViewModel.Data = new BOLLT00F()
            {
                bolsoc = _dataContext.CompanyID,
                BTANNO = year,
                BTDATP = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(),
                BTLING = "IT",
                BTCOLL = 1,
                BTFLCF = _dataContext.EntityType,
                Rows = new ObservableCollection<BOLLD00F>()
            };
            windowViewModel.IsInsert = true;

            var wBOLLT00F = new BOLLT00FWindow(windowViewModel);
            wBOLLT00F.Owner = Window.GetWindow(this);

            if (wBOLLT00F.ShowDialog() == true)
            {
                var windowDetailsViewModel = VulpesServiceProvider.Provider.GetRequiredService<BOLLD00FWindowViewModel>();
                windowDetailsViewModel.Head = windowViewModel.Data;

                var wBOLLD00F = new BOLLD00FWindow(windowDetailsViewModel);
                wBOLLD00F.Owner = Window.GetWindow(this);
                wBOLLD00F.ShowDialog();

                LoadData();
            }
        }

        private void cmdEditRows_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as BOLLT00F;

            if (item != null)
            {
                item = _dataContext.GetFull(item.BTANNO, item.BTBOLL);

                if (item != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BOLLD00FWindowViewModel>();
                    windowViewModel.Head = item;

                    var wBOLLD00F = new BOLLD00FWindow(windowViewModel);
                    wBOLLD00F.Owner = Window.GetWindow(this);
                    if (wBOLLD00F.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void rgPrintPreview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as RadGlyph)?.DataContext as BOLLT00F;
            if (item != null)
            {
                var data = _dataContext.GetPrintFull(item.BTANNO, item.BTBOLL);
                if (data != null)
                {
                    var reportData = _dataContext.PrintDDT(data);
                    if (reportData != null)
                        ReportingHandler.PrintBookPDF(UserContext.Instance!.Domain!, Constants.MODULE_SHIPPING, new string[] { Constants.REPORT_TYPE_DDT }, _dataContext.CompanyID, reportData, $"Bozza DDT n.{item.PrintFullID}", item.PrintFilename, true, true);
                    else
                        ErrorHandler.Validation($"Impossibile trovare il DDT {item.BTANNO}/{item.BTBOLL}");
                }
                else
                {
                    ErrorHandler.Validation($"Impossibile trovare il DDT {item.BTANNO}/{item.BTBOLL}");
                }
            }
        }

        private void rgPrint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as RadGlyph)?.DataContext as BOLLT00F;

            if (item != null)
            {
                item = _dataContext.Get(item.BTANNO, item.BTBOLL);
                if (item != null)
                {
                    if (item.BTNUBD == 0)
                    {
                        var causal = _dataContext.GetCAUSBOLL(item.BTCAUS ?? string.Empty);

                        if (causal != null && !string.IsNullOrWhiteSpace(causal.bolnum))
                        {
                            if (causal.bolmagBool)
                            {
                                if (!string.IsNullOrWhiteSpace(causal.BOLCAU))
                                {
                                    var storeCausal = _dataContext.GetStore_Causals(causal.BOLCAU);
                                    if (storeCausal != null)
                                    {
                                        // check if full engaged
                                        var quantities = _dataContext.GetRowsTotalQuantity(item.BTANNO, item.BTBOLL);

                                        if (quantities != null && quantities.Item2 == quantities.Item3) // check BOLLD00F1 is synced with current engages!
                                        {
                                            if (quantities.Item1 == quantities.Item2 && quantities.Item1 > 0 && quantities.Item2 > 0)
                                            {
                                                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AskDDTSendDateWindowViewModel>();
                                                windowViewModel.Data = item;
                                                windowViewModel.StoreCausal = storeCausal;

                                                var wAsk = new AskDDTSendDateWindow(windowViewModel);
                                                wAsk.Owner = Window.GetWindow(this);
                                                if (wAsk.ShowDialog() == true)
                                                    LoadData();
                                            }
                                            else
                                            {
                                                Mouse.OverrideCursor = null;
                                                ErrorHandler.Validation($"Il DDT selezionato non può essere stampato perchè incompleto\nVerificare di aver impegnato tutto il materiale necessario");
                                            }

                                        }
                                        else
                                        {
                                            Mouse.OverrideCursor = null;
                                            ErrorHandler.Validation($"Il DDT selezionato non può essere stampato perchè gli impegni reali [{quantities?.Item3.ToString("N6")}] non corrispondono agli impegni registrati per il documento [{quantities?.Item2.ToString("N6")}]\n\nProbabilmente sono stati modificati gli impegni nella gestione del magazzino");
                                        }
                                    }
                                    else
                                    {
                                        Mouse.OverrideCursor = null;
                                        ErrorHandler.Validation($"La causale impostata\n{causal.FullDescriptionSearchable}\nnon ha specificato nessuna causale di magazzino valida per lo scarico del materiale");
                                    }
                                }
                                else
                                {
                                    Mouse.OverrideCursor = null;
                                    ErrorHandler.Validation($"La causale impostata\n{causal.FullDescriptionSearchable}\nnon ha specificato nessuna causale di magazzino per lo scarico del materiale");
                                }
                            }
                            else
                            {
                                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AskDDTSendDateWindowViewModel>();
                                windowViewModel.Data = item;
                                windowViewModel.StoreCausal = null;

                                var wAsk = new AskDDTSendDateWindow(windowViewModel);
                                wAsk.Owner = Window.GetWindow(this);
                                if (wAsk.ShowDialog() == true)
                                    LoadData();
                            }
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Validation($"La causale impostata\n{causal?.FullDescriptionSearchable}\nnon ha specificato nessun numeratore necessario per assegnare il numero definitivo");
                        }
                    }
                    else
                    {
                        var data = _dataContext.GetPrintFull(item.BTANNO, item.BTBOLL);
                        if (data != null)
                        {
                            var reportData = _dataContext.PrintDDT(data);
                            if (reportData != null)
                                ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_SHIPPING, Constants.REPORT_TYPE_DDT, _dataContext.CompanyID, reportData, $"DDT n.{item.PrintFullID}", item.PrintFilename, true);
                            else
                                ErrorHandler.Validation($"Impossibile trovare il DDT {item.BTANNO}/{item.BTBOLL}");
                        }
                    }
                }
            }
        }

        private void cmdEditCustomer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var selected = (sender as Button)?.DataContext as BOLLT00F;

            if (selected != null && selected.BTCODC.HasValue && selected.BTCODC.Value > 0)
            {
                var item = _dataContext.GetABE(selected.BTCODC.Value);

                if (item != null)
                {
                    if (item.abecod > 0)
                    {
                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ABEWindowViewModel>();

                        FORNAMMI? supplierData = null;
                        FORNITORI? supplierCommercialData = null;
                        var groupsList = _dataContext.GetGruppi();
                        var accountCache = _dataContext.GetConti();
                        var subaccountCache = _dataContext.GetSotto();

                        if (item.abecfe == "F" || item.abecfe == "E")
                        {
                            supplierData = _dataContext.GetFORNAMMI(item.abecod);
                            if (supplierData != null)
                            {
                                supplierData.AccountCache = accountCache;
                                supplierData.SubaccountCache = subaccountCache;
                                supplierData.GroupsList = groupsList;
                            }
                            else
                            {
                                supplierData = new FORNAMMI()
                                {
                                    AccountCache = accountCache,
                                    SubaccountCache = subaccountCache,
                                    GroupsList = groupsList,
                                    Foraso = _dataContext.CompanyID,
                                    Foraco = item.abecod
                                };
                            }
                            supplierCommercialData = _dataContext.GetFORNITORI(item.abecod);
                            if (supplierCommercialData == null)
                                supplierCommercialData = new FORNITORI() { FOCLIF = item.abecod };
                        }

                        CLIAMMI? customerData = null;
                        CLIENTI? customerCommercialData = null;
                        if (item.abecfe == "C" || item.abecfe == "E")
                        {
                            customerData = _dataContext.GetCLIAMMI(item.abecod);
                            if (customerData != null)
                            {
                                customerData.AccountCache = accountCache;
                                customerData.SubaccountCache = subaccountCache;
                                customerData.GroupsList = groupsList;
                            }
                            else
                            {
                                customerData = new CLIAMMI()
                                {
                                    AccountCache = accountCache,
                                    SubaccountCache = subaccountCache,
                                    GroupsList = groupsList,
                                    Cliasoc = _dataContext.CompanyID,
                                    Cliacod = item.abecod
                                };
                            }
                            customerCommercialData = _dataContext.GetCLIENTI(item.abecod);
                            if (customerCommercialData == null)
                                customerCommercialData = new CLIENTI() { CLIENT = item.abecod };
                        }

                        windowViewModel.Data = item;
                        windowViewModel.CustomerNotes = _dataContext.GetNOTECLI1s(item.abecod);
                        windowViewModel.SupplierNotes = _dataContext.GetNOTEFORs(item.abecod);
                        windowViewModel.SupplierData = supplierData;
                        windowViewModel.CustomerData = customerData;
                        windowViewModel.CustomerCommercialData = customerCommercialData;
                        windowViewModel.SupplierCommercialData = supplierCommercialData;
                        windowViewModel.SupplierReferences = _dataContext.GetRFFTB00Fs(item.abecod);
                        windowViewModel.CustomerReferences = _dataContext.GetANDEFREs(item.abecod);
                        windowViewModel.CounterpartsRows = _dataContext.GetSUPPLIER_GROUPs(item.abecod);
                        windowViewModel.CounterpartsRowsCustomer = _dataContext.GetCUSTOMER_GROUPs(item.abecod);
                        windowViewModel.IsInsert = false;

                        var wABE = VulpesServiceProvider.Provider.GetRequiredService<IABEWindowFactory>().Create(windowViewModel);
                        Mouse.OverrideCursor = null;
                        wABE.Owner = Window.GetWindow(this);
                        if (wABE.ShowDialog() == true)
                            LoadData();
                    }
                }
            }
        }
        #endregion

        #region UC standard functions
        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.IsSelectable = cmbStatus.SelectedValue?.ToString() == "R";

            LoadData();
        }

        private void dtpYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }
        #endregion

        private void cmdInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (GridView.SelectedItems != null && GridView.SelectedItems.Count > 0)
            {
                if (ConfirmHandler.Confirm($"Confermate la creazione delle fatture per i DDT selezionati ?"))
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<YearWindowViewModel>();

                    var wAsk = new YearWindow(windowViewModel);
                    wAsk.Owner = Window.GetWindow(this);
                    if (wAsk.ShowDialog() == true)
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        var DDTList = new List<BOLLT00F>();

                        foreach (var ddt in GridView.SelectedItems.Cast<BOLLT00F>())
                        {
                            var full = _dataContext.GetFull(ddt.BTANNO, ddt.BTBOLL);

                            if (full != null)
                                DDTList.Add(full);
                        }

                        _dataContext.GenerateByDDT(DDTList, windowViewModel.SelectedYear);

                        LoadData();
                    }
                }
            }
        }

        #region Import
        private void rmiImpGILAT_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //var wImport = new wWaitImportingGILAT(ctx);
            //wImport.Owner = Window.GetWindow(this);
            //wImport.ShowDialog();
            //LoadData(_selectedPage);
        }

        private void rmiImpBANCOLAT_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //var wImport = new wWaitImportingBANCOLAT(ctx);
            //wImport.Owner = Window.GetWindow(this);
            //wImport.ShowDialog();
            //LoadData(_selectedPage);
        }
        #endregion

        #region Custom loading

        private void txtSearch_LostFocus(object sender, RoutedEventArgs? e)
        {
            if (e != null)
                LoadData();
            else
                (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }


        private void rnudPageSize_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            //rdpGrid.PageSize = (int)rnudPageSize.Value;
            //if (e.OldValue != null)
            //    LoadData(0);
        }
        #endregion
    }
}
