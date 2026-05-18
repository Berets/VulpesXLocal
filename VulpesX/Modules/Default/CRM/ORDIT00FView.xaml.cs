using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
using Telerik.Windows.Diagrams.Core;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.CRM;
using static VulpesX.Shared.Utilities.NotifierHelper;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for ORDIT00FView.xaml
    /// </summary>
    public partial class ORDIT00FView : UserControl
    {
        private ORDIT00FViewModel _dataContext;
        private int _selectedPage;
        private int _selectedDetailsPage;
        public ORDIT00FView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ORDIT00FViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            dtpYear.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpYear.Culture.DateTimeFormat.ShortDatePattern = "yyyy";
            dtpYear.SelectedValue = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            cmbStatus.ItemsSource = CommonsService.OrderStatuses;
            cmbStatus.SelectedValue = "A";

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                    LoadDetailsData();
                }
            };

            GridView.DataLoaded += (s, e) =>
            {
                rdpGrid.MoveToPage(_selectedPage);
                txtSearch_TextChanged(txtSearch, null);
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            GridViewDetails.DataLoaded += (s, e) =>
            {
                rdpGridDetails.MoveToPage(_selectedDetailsPage);
                txtSearchDetails_TextChanged(txtSearchDetails, null);
            };
            rdpGridDetails.PageIndexChanged += (s, e) =>
            {
                _selectedDetailsPage = e.NewPageIndex;
            };

            this.Loaded += (s, e) =>
            {
                OrdersListSelectorHandler.CallAction = (companyid, year, id, isProductionNeeded) =>
                {
                    var prods = _dataContext.GetPro_Ordines(companyid, year, id);

                    if (prods == null || prods.Count == 0)
                        return "N";

                    if ((prods.Any(any => any.Stato != "E") && prods.Any(any => any.Stato == "E")) || isProductionNeeded != prods.Count)
                    {
                        return "P";
                    }
                    if (prods.All(all => all.Stato == "E"))
                    {
                        return "R";
                    }
                    if (prods.All(all => all.Stato != "E"))
                    {
                        return "N";
                    }

                    return "D";
                };
            };
            this.Unloaded += (s, e) =>
            {
                OrdersListSelectorHandler.CallAction = null;
            };

            LoadDetailsData();
        }

        private async void LoadData()
        {
            string? selectedStatusID = cmbStatus.SelectedValue?.ToString();
            int? year = dtpYear.SelectedValue?.Year;

            if (selectedStatusID != null && year.HasValue)
            {
                await _dataContext.Load(year!.Value, selectedStatusID);
            }
        }

        private async void LoadDetailsData()
        {
            await _dataContext.LoadDetails();
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as ORDIT00F;

            if (item != null)
            {
                item = _dataContext.GetFull(item.OTANNO, item.OTNUOR);

                if (item != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDIT00FWindowViewModel>();
                    windowViewModel.Data = item;
                    windowViewModel.IsInsert = false;

                    var wORDIT00F = new ORDIT00FWindow(windowViewModel);
                    Mouse.OverrideCursor = null;
                    wORDIT00F.Owner = Window.GetWindow(this);
                    if (wORDIT00F.ShowDialog() == true)
                    {
                        LoadData();
                        LoadDetailsData();
                    }
                }
            }
        }

        private void cmdEditRows_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as ORDIT00F;
            if (item != null)
            {
                item = _dataContext.GetFull(item.OTANNO, item.OTNUOR);

                if (item != null)
                {
                    //var customerData = new CLIAMMIService().Get(ctx.CurrentCompanyID, item.OTCLIE ?? 0);
                    //item.DefaultFirstAgent = new AGENTI() { agecod = customerData?.CLAGEN, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT };
                    //item.DefaultSecondAgent = new AGENTI() { agecod = customerData?.CLAGEN2, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT };

                    var windowDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDID00FWindowViewModel>();
                    windowDetailViewModel.Head = item;

                    var wORDID00F = new ORDID00FWindow(windowDetailViewModel);
                    wORDID00F.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wORDID00F.ShowDialog() == true)
                    {
                        LoadData();
                        LoadDetailsData();
                    }
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as ORDIT00F;

            if (item != null)
            {
                item = _dataContext.GetFull(item.OTANNO, item.OTNUOR);

                if (item != null)
                {
                    if (item.CanDelete)
                    {
                        if (ConfirmHandler.Confirm($"Confermate di voler eliminare l'ordine n. {item.PrintFullID} ?\nNon sara' possibile recuperare le informazioni eliminate, procedere ?"))
                        {
                            // ask for reason
                            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CancelReasonWindowViewModel>();

                            var wAskCR = new CancelReasonWindow(windowViewModel);
                            wAskCR.Owner = Window.GetWindow(this);
                            Mouse.OverrideCursor = null;
                            if (wAskCR.ShowDialog() == true && !string.IsNullOrWhiteSpace(windowViewModel.SelectedReason))
                            {
                                Mouse.OverrideCursor = Cursors.Wait;
                                item.flgchi = "X";
                                item.canceled = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                item.canceledUserID = _dataContext.UserID;
                                item.canceledNote = windowViewModel.SelectedReason;

                                _dataContext.Update(item);

                                LoadData();
                                LoadDetailsData();
                            }
                        }
                        Mouse.OverrideCursor = null;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("Impossibile eliminare un ordine chiuso, trasformato in ordini di produzione o gia' annullato");
                    }
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            int year = dtpYear.SelectedValue?.Year ?? 0;

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDIT00FWindowViewModel>();
            windowViewModel.Data = new ORDIT00F()
            {
                otsoci = _dataContext.CompanyID,
                OTANNO = year,
                flgchi = "A",
                OTDAOR = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(),
                otdivi = "EUR",
                OTCIDI = "UIC",
                otling = "IT",
                addedUserID = _dataContext.UserID
            };
            windowViewModel.IsInsert = true;

            var wORDIT00F = new ORDIT00FWindow(windowViewModel);
            Mouse.OverrideCursor = null;
            wORDIT00F.Owner = Window.GetWindow(this);
            if (wORDIT00F.ShowDialog() == true)
            {
                var head = _dataContext.GetFull(windowViewModel.Data.OTANNO, windowViewModel.Data.OTNUOR);
                if (head != null)
                {
                    var windowDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDID00FWindowViewModel>();
                    windowDetailViewModel.Head = head;

                    var wORDID00F = new ORDID00FWindow(windowDetailViewModel);
                    wORDID00F.Owner = Window.GetWindow(this);
                    wORDID00F.ShowDialog();
                }

                LoadData();
                LoadDetailsData();
            }
        }

        private void cmdDDT_Click(object sender, RoutedEventArgs e)
        {
            if (GridView.SelectedItems != null && GridView.SelectedItems.Count > 0)
            {
                if (ConfirmHandler.Confirm("Procedere alla composizione di un nuovo DDT utilizzando gli ordini selezionati ?"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var selectedRows = new List<ORDID00F>();
                    var orders = new List<ORDIT00F>();
                    foreach (var item in GridView.SelectedItems.Cast<ORDIT00F>())
                    {
                        var full = _dataContext.GetFull(item.OTANNO, item.OTNUOR);

                        if (full != null)
                        {
                            orders.Add(full);
                            selectedRows.AddRange((full.Rows ?? new()).Where(w => (w.ODQTAEV < w.ODQTAV || w.ODQTAEV == null) && w.ODSTATO == null));
                        }
                    }

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDID00FSelectWindowViewModel>();
                    windowViewModel.OrdersHeads = orders;
                    windowViewModel.AvailableRows = new ObservableCollection<ORDID00F>(selectedRows.OrderBy(o => o.OTNUOR).ThenBy(o => o.ODRIGA));
                    windowViewModel.FlagTarget = "D";

                    var wSelection = new ORDID00FSelectWindow(windowViewModel);
                    wSelection.Owner = Window.GetWindow(this);
                    wSelection.ShowDialog();
                    LoadData();
                    LoadDetailsData();
                }
            }
        }

        private void cmdInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (GridView.SelectedItems != null && GridView.SelectedItems.Count > 0)
            {
                if (ConfirmHandler.Confirm("Procedere alla composizione della fattura partendo dagli ordini selezionati ?"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var selectedRows = new List<ORDID00F>();
                    var orders = new List<ORDIT00F>();
                    foreach (var item in GridView.SelectedItems.Cast<ORDIT00F>())
                    {
                        var full = _dataContext.GetFull(item.OTANNO, item.OTNUOR);

                        if (full != null)
                        {
                            orders.Add(full);
                            selectedRows.AddRange((full.Rows ?? new()).Where(w => (w.ODQTAEV < w.ODQTAV || w.ODQTAEV == null) && w.ODSTATO == null));
                        }
                    }

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDID00FSelectWindowViewModel>();
                    windowViewModel.OrdersHeads = orders;
                    windowViewModel.AvailableRows = new ObservableCollection<ORDID00F>(selectedRows.OrderBy(o => o.OTNUOR).ThenBy(o => o.ODRIGA));
                    windowViewModel.FlagTarget = "I";

                    var wSelection = new ORDID00FSelectWindow(windowViewModel);
                    wSelection.Owner = Window.GetWindow(this);
                    wSelection.ShowDialog();
                    LoadData();
                    LoadDetailsData();
                }
            }
        }

        private void cmdDDTDetails_Click(object sender, RoutedEventArgs e)
        {
            if (GridViewDetails.SelectedItems != null && GridViewDetails.SelectedItems.Count > 0)
            {
                if (ConfirmHandler.Confirm("Procedere alla composizione del DDT con le righe selezionate ?"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var selectedRows = GridViewDetails.SelectedItems.Cast<ORDID00F>().ToList();
                    var orders = new List<ORDIT00F>();
                    foreach (var item in selectedRows.GroupBy(g => new { g.otsoci, g.OTANNO, g.OTNUOR }))
                    {
                        var full = _dataContext.GetFull(item.Key.OTANNO, item.Key.OTNUOR);

                        if (full != null)
                            orders.Add(full);
                    }

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDID00FSelectWindowViewModel>();
                    windowViewModel.OrdersHeads = orders;
                    windowViewModel.AvailableRows = new ObservableCollection<ORDID00F>(selectedRows.OrderBy(o => o.OTNUOR).ThenBy(o => o.ODRIGA));
                    windowViewModel.FlagTarget = "D";

                    var wSelection = new ORDID00FSelectWindow(windowViewModel);
                    wSelection.Owner = Window.GetWindow(this);
                    wSelection.ShowDialog();
                    LoadData();
                    LoadDetailsData();
                }
            }
        }

        private void cmdInvoiceDetails_Click(object sender, RoutedEventArgs e)
        {
            if (GridViewDetails.SelectedItems != null && GridViewDetails.SelectedItems.Count > 0)
            {
                if (ConfirmHandler.Confirm("Procedere alla composizione della fattura con le righe selezionate ?"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var selectedRows = GridViewDetails.SelectedItems.Cast<ORDID00F>().ToList();
                    var orders = new List<ORDIT00F>();
                    foreach (var item in selectedRows.GroupBy(g => new { g.otsoci, g.OTANNO, g.OTNUOR }))
                    {
                        var full = _dataContext.GetFull(item.Key.OTANNO, item.Key.OTNUOR);

                        if (full != null)
                            orders.Add(full);
                    }

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDID00FSelectWindowViewModel>();
                    windowViewModel.OrdersHeads = orders;
                    windowViewModel.AvailableRows = new ObservableCollection<ORDID00F>(selectedRows.OrderBy(o => o.OTNUOR).ThenBy(o => o.ODRIGA));
                    windowViewModel.FlagTarget = "I";

                    var wSelection = new ORDID00FSelectWindow(windowViewModel);
                    wSelection.Owner = Window.GetWindow(this);
                    wSelection.ShowDialog();
                    LoadData();
                    LoadDetailsData();
                }
            }
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private void txtSearchDetails_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearchDetails.Text, GridViewDetails);
        }

        private void GridView_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            //_selectedPage = rdpGrid.PageIndex;
            //Mouse.OverrideCursor = Cursors.Wait;
            //// OPTIMIZE cloned code
            //if (ctx.CurrentUser.Roles.canDDT)
            //{
            //    bool canDDT = GridView.SelectedItems != null && GridView.SelectedItems.Count > 0 &&
            //                  GridView.SelectedItems.Cast<ORDIT00F>().GroupBy(g => new { g.OTCLIE, g.DESTIN }).Count() == 1 &&
            //                  GridView.SelectedItems.Cast<ORDIT00F>().All(all => all.flgchi == "F");

            //    if (canDDT)
            //    {
            //        foreach (var item in GridView.SelectedItems.Cast<ORDIT00F>())
            //        {
            //            if (item.Rows.Any(any => any.ODSTA == null))
            //            {
            //                canDDT = false;
            //                break;
            //            }
            //            var prodNeeded = item.Rows.Where(w => w.ODSTA != "#").Count();
            //            if (prodNeeded > 0)
            //            {
            //                var prods = pro_ordineService.GetListByOrder(item.otsoci, item.OTANNO, item.OTNUOR);
            //                if (prodNeeded == prods.Count)
            //                {
            //                    if (prods.Any(any => any.Stato != "E"))
            //                    {
            //                        canDDT = false;
            //                        break;
            //                    }

            //                }
            //                else
            //                {
            //                    canDDT = false;
            //                    break;
            //                }
            //            }
            //        }
            //        if (canDDT)
            //        {
            //            cmdDDT.IsEnabled = true;
            //        }
            //        else
            //        {
            //            cmdDDT.IsEnabled = false;
            //            cmdDDT.ToolTip = "Alcuni ordini selezionati hanno delle produzioni incomplete";
            //        }
            //    }
            //    else
            //    {
            //        cmdDDT.IsEnabled = false;
            //        cmdDDT.ToolTip = "Avete selezionato degli ordini annullati, per clienti/destinazioni differenti o gia' evasi";
            //    }
            //}
            //else
            //{
            //    cmdDDT.IsEnabled = false;
            //    cmdDDT.ToolTip = "Non si possiede l'abilitazione per l'evasione degli ordini clienti e la gestione dei DDT";
            //}
            //if (ctx.CurrentUser.Roles.canInvoices)
            //{
            //    bool canInvoice = GridView.SelectedItems != null && GridView.SelectedItems.Count > 0 &&
            //                      GridView.SelectedItems.Cast<ORDIT00F>().GroupBy(g => new { g.OTCLIE, g.DESTIN, g.OTPAGA, g.abiabi, g.abicab, g.OTBCON }).Count() == 1 &&
            //                      GridView.SelectedItems.Cast<ORDIT00F>().All(all => all.flgchi == "F");

            //    if (canInvoice)
            //    {
            //        foreach (var item in GridView.SelectedItems.Cast<ORDIT00F>())
            //        {
            //            if (item.Rows.Any(any => any.ODSTA == null))
            //            {
            //                canInvoice = false;
            //                break;
            //            }
            //            var prodNeeded = item.Rows.Where(w => w.ODSTA != "#").Count();
            //            if (prodNeeded > 0)
            //            {
            //                var prods = pro_ordineService.GetListByOrder(item.otsoci, item.OTANNO, item.OTNUOR);
            //                if (prodNeeded == prods.Count)
            //                {
            //                    if (prods.Any(any => any.Stato != "E"))
            //                    {
            //                        canInvoice = false;
            //                        break;
            //                    }

            //                }
            //                else
            //                {
            //                    canInvoice = false;
            //                    break;
            //                }
            //            }
            //        }
            //        if (canInvoice)
            //        {
            //            cmdInvoice.IsEnabled = true;
            //        }
            //        else
            //        {
            //            cmdInvoice.IsEnabled = false;
            //            cmdInvoice.ToolTip = "Alcuni ordini selezionati hanno delle produzioni incomplete";
            //        }
            //    }
            //}
            //else
            //{
            //    cmdInvoice.IsEnabled = false;
            //    cmdInvoice.ToolTip = "Non si possiede l'abilitazione per l'evasione degli ordini clienti e la gestione delle fatture";
            //}
            //Mouse.OverrideCursor = null;
        }

        private void GridViewDetails_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            //_detailsSelectedPage = rdpGridDetails.PageIndex;
            //Mouse.OverrideCursor = Cursors.Wait;
            //if (ctx.CurrentUser.Roles.canDDT)
            //{
            //    bool canDDT = GridViewDetails.SelectedItems != null && GridViewDetails.SelectedItems.Count > 0 &&
            //                  GridViewDetails.SelectedItems.Cast<ORDID00F>().GroupBy(g => new { customer = g.Customer.abecod, recipient = g.Recipient?.codesti }).Count() == 1;

            //    if (canDDT)
            //    {
            //        foreach (var item in GridViewDetails.SelectedItems.Cast<ORDID00F>().Where(w => w.ODSTA != "#"))
            //        {
            //            var prod = pro_ordineService.GetByOrder(item.otsoci, item.OTANNO, item.OTNUOR, item.ODRIGA);
            //            if (prod == null || prod.Stato != "E")
            //            {
            //                canDDT = false;
            //                break;
            //            }
            //        }
            //        if (canDDT)
            //        {
            //            cmdDDTDetails.IsEnabled = true;
            //        }
            //        else
            //        {
            //            cmdDDTDetails.IsEnabled = false;
            //            cmdDDTDetails.ToolTip = "Alcuni ordini selezionati hanno delle produzioni incomplete";
            //        }
            //    }
            //    else
            //    {
            //        cmdDDTDetails.IsEnabled = false;
            //        cmdDDTDetails.ToolTip = "Avete selezionato degli ordini annullati, per clienti/destinazioni differenti o gia' evasi";
            //    }
            //}
            //else
            //{
            //    cmdDDT.IsEnabled = false;
            //    cmdDDT.ToolTip = "Non si possiede l'abilitazione per l'evasione degli ordini clienti e la gestione dei DDT";
            //}
            //if (ctx.CurrentUser.Roles.canInvoices)
            //{
            //    bool canInvoiceDetails = GridViewDetails.SelectedItems != null && GridViewDetails.SelectedItems.Count > 0 &&
            //                  GridViewDetails.SelectedItems.Cast<ORDID00F>().GroupBy(g => new { customer = g.Customer.abecod, recipient = g.Recipient?.codesti, g.PaymentID, g.BankFullID }).Count() == 1;

            //    if (canInvoiceDetails)
            //    {
            //        foreach (var item in GridViewDetails.SelectedItems.Cast<ORDID00F>().Where(w => w.ODSTA != "#"))
            //        {
            //            var prod = pro_ordineService.GetByOrder(item.otsoci, item.OTANNO, item.OTNUOR, item.ODRIGA);
            //            if (prod == null || prod.Stato != "E")
            //            {
            //                canInvoiceDetails = false;
            //                break;
            //            }
            //        }
            //        if (canInvoiceDetails)
            //        {
            //            cmdInvoiceDetails.IsEnabled = true;
            //        }
            //        else
            //        {
            //            cmdInvoiceDetails.IsEnabled = false;
            //            cmdInvoiceDetails.ToolTip = "Alcuni ordini selezionati hanno delle produzioni incomplete";
            //        }
            //    }
            //    else
            //    {
            //        cmdInvoiceDetails.IsEnabled = false;
            //        cmdInvoiceDetails.ToolTip = "Avete selezionato degli ordini annullati, per clienti/destinazioni/pagamento/banca/agenti differenti o gia' evasi";
            //    }
            //}
            //else
            //{
            //    cmdInvoiceDetails.IsEnabled = false;
            //    cmdInvoiceDetails.ToolTip = "Non si possiede l'abilitazione per l'evasione degli ordini clienti e la gestione dei DDT";
            //}
            //Mouse.OverrideCursor = null;
        }

        private void dtpYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }
        #endregion

        #region Managements grid icons events
        private void rgDDTIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as RadGlyph)?.DataContext as ORDIT00F;
            if (item != null)
            {
                item = _dataContext.GetFull(item.OTANNO, item.OTNUOR);

                if (item != null && item.DDTs != null && item.DDTs.Count > 0)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDIT00FDDTsWindowViewModel>();
                    windowViewModel.Head = item;

                    var wOrderDDTs = new ORDIT00FDDTsWindow(windowViewModel);
                    Mouse.OverrideCursor = null;
                    wOrderDDTs.Owner = Window.GetWindow(this);
                    wOrderDDTs.ShowDialog();
                }
                else
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void rgInvoicesIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as ORDIT00F;

            if (item != null)
            {
                item = _dataContext.GetFull(item.OTANNO, item.OTNUOR);

                if (item != null && item.Invoices != null && item.Invoices.Count > 0)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDIT00FInvoicesWindowViewModel>();
                    windowViewModel.Head = item;

                    var wOrderInvoices = new ORDIT00FInvoicesWindow(windowViewModel);
                    wOrderInvoices.Owner = Window.GetWindow(this);
                    wOrderInvoices.ShowDialog();
                }
            }
        }

        private void rgSendEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as ORDIT00F;
            if (item != null)
            {
                if (item.CanSend)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    if (item != null)
                    {
                        item = _dataContext.GetPrintFull(item.OTANNO, item.OTNUOR, true);

                        if (item != null)
                        {
                            var reportData = _dataContext.PrintOrder(item);

                            if (reportData != null)
                            {
                                var pdf = ReportingHandler.PrintBookPDF(UserContext.Instance!.Domain!, Constants.MODULE_CRM, new string[] { Constants.REPORT_TYPE_ORDER }, _dataContext.CompanyID, reportData, $"Ordine n.{item.PrintFullID}", item.PrintFilename, false, !item.CanPrint);
                                if (!string.IsNullOrWhiteSpace(pdf))
                                {
                                    var atts = new ObservableCollection<string>() { pdf };

                                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SendMailWindowViewModel>();
                                    windowViewModel.SendClass = NotifierHelper.SendClasses.CRM_Orders;
                                    windowViewModel.Attachments = atts;
                                    windowViewModel.CustomerID = item.OTCLIE!.Value;
                                    windowViewModel.DocumentYear = item.OTANNO;
                                    windowViewModel.DocumentNumber = item.OTNUOR;
                                    windowViewModel.OriginalFilename = $"{Path.GetFileNameWithoutExtension(item.PrintFilename)}.zip";
                                    windowViewModel.Language = item.Language;

                                    var wSendEmail = new SendMailWindow(windowViewModel);
                                    wSendEmail.Owner = Window.GetWindow(this);
                                    if (wSendEmail.ShowDialog() == true)
                                    {
                                        LoadData();
                                    }
                                }
                            }
                            else
                            {
                                ErrorHandler.Validation($"Impossibile trovare l'ordine {item.PrintFullID}");
                            }
                        }
                    }
                }
                else
                { ErrorHandler.Validation("Impossibile inviare un'ordine non completamente firmato"); }
            }
        }

        private void rgPrint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as ORDIT00F;

            Mouse.OverrideCursor = Cursors.Wait;

            if (item != null)
            {
                item = _dataContext.GetPrintFull(item.OTANNO, item.OTNUOR, true);

                if (item != null)
                {
                    var reportData = _dataContext.PrintOrder(item);
                    if (reportData != null)
                        ReportingHandler.PrintBookPDF(UserContext.Instance!.Domain!, Constants.MODULE_CRM, new string[] { Constants.REPORT_TYPE_ORDER }, _dataContext.CompanyID, reportData, $"Ordine n.{item.PrintFullID}", item.PrintFilename, true, !item.CanPrint);
                    else
                        ErrorHandler.Validation($"Impossibile trovare l'ordine {item.PrintFullID}");
                }
            }
        }

        private void rgSign_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as ORDIT00F;

            if (item != null)
            {
                item = _dataContext.Get(item.OTANNO, item.OTNUOR);
                if (item != null)
                {
                    if (item.flgchi == "A" || item.flgchi == "I" || item.flgchi == "T")
                    {
                        if (!item.OTINFI.HasValue && string.IsNullOrWhiteSpace(item.OTINFIUSR) &&
                            !item.OTFITE.HasValue && string.IsNullOrWhiteSpace(item.OTFITEUSR) &&
                            !item.OTFICO.HasValue && string.IsNullOrWhiteSpace(item.OTFICOUSR))
                        {
                            // send to sign
                            if ((UserContext.Instance!.ACCESS!.Roles?.canOrdersSignTech ?? false) && !(UserContext.Instance!.ACCESS!.Roles?.canOrdersSignCommercial ?? false))
                            {
                                // send to sign and technical sign
                                if (ConfirmHandler.Confirm($"Inviando l'ordine {item.PrintFullID} alla firma tecnica non sara' piu' possibile modificarlo. Inoltre il tuo utente apporra' automaticamente anche la firma tecnica, proseguire ?"))
                                {
                                    var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                    item.OTINFI = now;
                                    item.OTINFIUSR = _dataContext.UserID;
                                    item.OTFITE = now;
                                    item.OTFITEUSR = _dataContext.UserID;
                                    item.flgchi = "T";
                                    _dataContext.Update(item);
                                }
                            }
                            else
                            {
                                // only send to sign
                                if (ConfirmHandler.Confirm($"Inviando l'ordine {item.PrintFullID} alla firma tecnica non sara' piu' possibile modificarlo, proseguire ?"))
                                {
                                    item.OTINFI = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                    item.OTINFIUSR = _dataContext.UserID;
                                    item.flgchi = "I";
                                    _dataContext.Update(item);
                                }
                            }
                        }
                        else
                        {
                            if (item.OTINFI.HasValue && !string.IsNullOrWhiteSpace(item.OTINFIUSR) &&
                                !item.OTFITE.HasValue && string.IsNullOrWhiteSpace(item.OTFITEUSR) &&
                                !item.OTFICO.HasValue && string.IsNullOrWhiteSpace(item.OTFICOUSR))
                            {
                                // technical sign
                                if ((UserContext.Instance!.ACCESS!.Roles?.canOffersSignTech ?? false))
                                {
                                    if (ConfirmHandler.Confirm($"Apporre la firma tecnica sull'ordine {item.PrintFullID} ?"))
                                    {
                                        item.OTFITE = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                        item.OTFITEUSR = _dataContext.UserID;
                                        item.flgchi = "T";
                                        _dataContext.Update(item);
                                    }
                                }
                                else
                                { ErrorHandler.Validation("Il tuo utente non ha l'autorizzazione necessaria per apporre la firma tecnica"); }
                            }
                            else
                            {
                                if (item.OTINFI.HasValue && !string.IsNullOrWhiteSpace(item.OTINFIUSR) &&
                                    item.OTFITE.HasValue && !string.IsNullOrWhiteSpace(item.OTFITEUSR) &&
                                    !item.OTFICO.HasValue && string.IsNullOrWhiteSpace(item.OTFICOUSR))
                                {
                                    // commercial sign
                                    if ((UserContext.Instance!.ACCESS!.Roles?.canOffersSignCommercial ?? false))
                                    {
                                        if (ConfirmHandler.Confirm($"Apporre la firma commerciale sull'ordine {item.PrintFullID} ?"))
                                        {
                                            item.OTFICO = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                            item.OTFICOUSR = _dataContext.UserID;
                                            item.flgchi = "F";
                                            _dataContext.Update(item);
                                        }
                                    }
                                    else
                                    {
                                        ErrorHandler.Validation("Il tuo utente non ha l'autorizzazione necessaria per apporre la firma commerciale");
                                    }
                                }
                            }
                        }
                        LoadData();
                        LoadDetailsData();
                    }
                }
            }
        }

        private void rgProduction_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = ((RadGlyph)sender).DataContext as ORDIT00F;

            if (item != null)
            {
                if (item.flgchi == "F")
                {
                    if (ConfirmHandler.Confirm("Confermate la generazione degli ordini di produzione ?"))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (item != null)
                        {
                            item = _dataContext.GetPrintFull(item.OTANNO, item.OTNUOR, false);

                            if (item != null)
                            {
                                if (_dataContext.GenerateProductionOrder(item))
                                {
                                    Mouse.OverrideCursor = null;
                                    InfoHandler.Show($"Ordini di produzione generati correttamente");
                                    LoadData();
                                    LoadDetailsData();
                                }
                                else
                                {
                                    Mouse.OverrideCursor = null;
                                    ErrorHandler.Validation("Errore durante la generazione degli ordini di produzione");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void rgOrderAttachmentsIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var row = (sender as RadGlyph)?.DataContext as ORDIT00F;
            if (row != null)
            {
                row = _dataContext.Get(row.OTANNO, row.OTNUOR);
                if (row != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDITAL00FWindowViewModel>();
                    windowViewModel.Head = row;

                    var wORDITAL00F = new ORDITAL00FWindow(windowViewModel);
                    wORDITAL00F.Owner = Window.GetWindow(this);
                    if (wORDITAL00F.ShowDialog() == true)
                        LoadData();
                }
                else
                { Mouse.OverrideCursor = null; }
            }
        }

        private void rgAlert_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var row = (sender as RadGlyph)?.DataContext as ORDIT00F;

            if (row != null)
            {
                row = _dataContext.Get(row.OTANNO, row.OTNUOR);

                if (row != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDITALEWindowViewModel>();
                    windowViewModel.Head = row;

                    var wORDITALE = new ORDITALEWindow(windowViewModel);
                    wORDITALE.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wORDITALE.ShowDialog() == true)
                        LoadData();
                }
            }
        }
        #endregion
    }
}
