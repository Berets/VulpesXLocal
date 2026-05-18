using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
using System.Xml;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Models.Models;
using VulpesX.Modules.Default.Accounting;
using VulpesX.Modules.Default.Accounting.Invoicing;
using VulpesX.Modules.Default.General;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;
using VulpesX.ViewModels.Modules.Default.General;
using VulpesX.ViewModels.Modules.Ufp.Accounting.Invoicing;
using VulpesX.WindowsFactory.Default.General;

namespace VulpesX.Modules.Ufp.Accounting.Invoicing
{
    /// <summary>
    /// Interaction logic for ACC_EINVOICE_HEADS.xaml
    /// </summary>
    public partial class ACC_EINVOICE_HEADSView : UserControl
    {
        private ACC_EINVOICE_HEADSViewModel _dataContext;
        private int _selectedPage;

        public ACC_EINVOICE_HEADSView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ACC_EINVOICE_HEADSViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();

                    CheckInvoicesWaiting();
                }
            };

            GridView.DataLoaded += (s, e) =>
            {
                txtSearch_TextChanged(txtSearch, null);

                Parallel.ForEach(_dataContext.Items ?? new ObservableCollection<ACC_EINVOICE_HEADS>(), item =>
                {
                    item.Suppliers = _dataContext.Suppliers;
                });

                rdpGrid.MoveToPage(_selectedPage);
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            CheckInvoicesWaiting();

            // set format for month/year selector
            rdtPeriod.Culture = new System.Globalization.CultureInfo("it-IT");
            rdtPeriod.Culture.DateTimeFormat.ShortDatePattern = "MMMM yyyy";
            _dataContext.Period = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            _dataContext.LoadDetails();
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }

        private void CheckInvoicesWaiting()
        {
            using (BackgroundWorker bgwCheck = new BackgroundWorker())
            {
                bgwCheck.DoWork += delegate (object? s, DoWorkEventArgs args)
                {
                    string? APIKey = _dataContext.GetApiKey();

                    if (!string.IsNullOrWhiteSpace(APIKey))
                    {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                        var client = new CerberoRetrieveAPI.RetrieveInvoicesClient();
                        if (client.State != System.ServiceModel.CommunicationState.Faulted)
                        {
                            try
                            {
                                var request = new CerberoRetrieveAPI.GetNewInvoicesCountRequest(APIKey);
                                var result = client.GetNewInvoicesCountAsync(request).Result;
                                args.Result = result.GetNewInvoicesCountResult;
                            }
                            catch (Exception exc)
                            {
                                Exception? exi = exc;
                                StringBuilder sb = new StringBuilder();
                                do
                                {
                                    sb.Append(exi.Message).Append("\n\n");
                                    exi = exi.InnerException;
                                } while (exi != null);
                                ErrorHandler.Validation(sb.ToString());
                                client.Close();
                            }
                            client.Close();
                        }
                    }
                };
                bgwCheck.RunWorkerAsync();
                bgwCheck.RunWorkerCompleted += (s, e) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        cmdDownload.Content = $"SCARICA FATTURE ({e.Result ?? 0})";
                    });
                };
            }
        }

        #region Buttons
        // RECEIVED
        private void cmdDownload_Click(object sender, RoutedEventArgs e)
        {
            string? APIKey = _dataContext.GetApiKey();
            if (!string.IsNullOrWhiteSpace(APIKey))
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<WaitDownloadWindowViewModel>();
                windowViewModel.ApiKey = APIKey;

                var wWait = new WaitDownloadWindow(windowViewModel);
                wWait.Owner = Window.GetWindow(this);
                wWait.ShowDialog();
                LoadData();
                CheckInvoicesWaiting();
            }
            else
            {
                ErrorHandler.Validation("Questa società non risulta abilitata al servizio di integrazione con il Sistema Di Interscambio (SDI) dell'Agenzia delle Entrate");
            }
            LoadData();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as ACC_EINVOICE_HEADS;
            if (item != null)
            {
                item = _dataContext.GetFull(item.id);

                if (item != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_EINVOICE_HEADSWindowViewModel>();
                    windowViewModel.Data = item;
                    windowViewModel.IsInsert = false;

                    var wDetails = new ACC_EINVOICE_HEADSWindow(windowViewModel);
                    wDetails.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    wDetails.ShowDialog();
                    LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as ACC_EINVOICE_HEADS;
            if (item != null)
            {
                item = _dataContext.Get(item.id);

                if (item != null)
                {
                    if (ConfirmHandler.Confirm($"Confermate di voler eliminare la fattura n. {item.fattnum} di {item.fattabers1} ?\nSara' necessario scaricarla nuovamente, procedere ?"))
                    {
                        if (!string.IsNullOrWhiteSpace(item.fattstampa))
                        {
                            if (ConfirmHandler.Confirm($"Attenzione, la fattura che si sta eliminando risulta gia' stampata, procedere comunque ?"))
                            {
                                if (item.accounted.HasValue)
                                {
                                    if (ConfirmHandler.Confirm($"Attenzione, la fattura che si sta eliminando risulta gia' contabilizzata, procedere comunque ?"))
                                    {
                                        Mouse.OverrideCursor = Cursors.Wait;
                                        _dataContext.Delete(item, null);
                                    }
                                }
                                else
                                {
                                    Mouse.OverrideCursor = Cursors.Wait;
                                    _dataContext.Delete(item, null);
                                }
                            }
                        }
                        else
                        {
                            Mouse.OverrideCursor = Cursors.Wait;
                            _dataContext.Delete(item, null);
                        }
                        Mouse.OverrideCursor = null;
                        LoadData();
                    }
                }
            }
        }

        private void cmdAddEntity_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as ACC_EINVOICE_HEADS;
            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ABEWindowViewModel>();
                windowViewModel.Data = new ABE()
                {
                    abecfe = "F",
                    abetpo = "1",
                    abers1 = item.fattabers1,
                    abeind = item.fattforinde ?? string.Empty,
                    abecap = !string.IsNullOrWhiteSpace(item.fattforcap) ? int.Parse(item.fattforcap) : null,
                    abeloc = item.fattforloca?.ToUpper(),
                    abepro = item.fattforprov,
                    abepiv = item.fattpiva,
                    abecfi = item.fattpiva,
                    isocod = item.fattiso,
                    nazcod = item.fattfornazi
                };
                windowViewModel.Recipients = new ObservableCollection<DESTINATARI>();
                windowViewModel.SupplierRecipients = new ObservableCollection<DESFOR>();
                windowViewModel.CustomerNotes = new ObservableCollection<NOTECLI1>();
                windowViewModel.SupplierNotes = new ObservableCollection<NOTEFOR>();
                windowViewModel.SupplierData = new FORNAMMI()
                {
                    AccountCache = _dataContext.AccountCache,
                    SubaccountCache = _dataContext.SubaccountCache,
                    GroupsList = _dataContext.GroupCache,
                    Foraso = _dataContext.CompanyID
                };
                windowViewModel.CustomerData = new CLIAMMI()
                {
                    AccountCache = _dataContext.AccountCache,
                    SubaccountCache = _dataContext.SubaccountCache,
                    GroupsList = _dataContext.GroupCache,
                    Cliasoc = _dataContext.CompanyID
                };
                windowViewModel.SupplierCommercialData = new FORNITORI()
                { };
                windowViewModel.CustomerCommercialData = new CLIENTI()
                {
                };
                windowViewModel.SupplierReferences = new ObservableCollection<RFFTB00F>();
                windowViewModel.CustomerReferences = new ObservableCollection<ANDEFRES>();
                windowViewModel.CounterpartsRows = new ObservableCollection<SUPPLIER_GROUPS>();
                windowViewModel.CounterpartsRowsCustomer = new ObservableCollection<CUSTOMER_GROUPS>();
                windowViewModel.IsInsert = true;

                var wABE = VulpesServiceProvider.Provider.GetRequiredService<IABEWindowFactory>().Create(windowViewModel);
                wABE.Owner = Window.GetWindow(this);
                wABE.ShowDialog();
                LoadData();
            }
        }
        #endregion

        #region Context menu
        // received
        private void cmGridReceived_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItem != null)
            {
                var item = GridView.SelectedItem as ACC_EINVOICE_HEADS;
                if (item != null)
                {
                    item = _dataContext.Get(item.id);

                    if (item != null)
                    {
                        rmiAccountingReceived.IsEnabled = !item.accountedBool && item.fattfor.HasValue;
                        if (rmiAccountingReceived.IsEnabled)
                        {
                            rmiAccountingReceived.Header = $"Contabilizza la fattura n. {item.fattnum} del {item.fattdata.ToString("dd/MM/yyyy")}";
                        }
                        else
                        {
                            if (item.fattfor.HasValue)
                                rmiAccountingReceived.Header = $"Fattura n. {item.fattnum} del {item.fattdata.ToString("dd/MM/yyyy")} gia' contabilizzata con registrazione {item.fattannoreg}/{item.fattnumreg}";
                            else
                                rmiAccountingReceived.Header = $"Fattura n. {item.fattnum} del {item.fattdata.ToString("dd/MM/yyyy")} NON contabilizzabile per mancanza del fornitore in anagrafica";
                        }
                    }
                }
            }
            else
            {
                rmiAccountingReceived.IsEnabled = false;
            }
        }

        private void rmiAccountingReceived_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItem != null)
            {
                var selected = GridView.SelectedItem as ACC_EINVOICE_HEADS;

                if (selected != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AskAccountingWindowViewModel>();
                    windowViewModel.Invoice = selected;
                    windowViewModel.IsSupplier = true;
                    windowViewModel.ShowCostCenter = true;

                    var wAskAccountingInfo = new AskAccountingWindow(windowViewModel);
                    wAskAccountingInfo.Owner = Window.GetWindow(this);
                    if (wAskAccountingInfo.ShowDialog() == true)
                        LoadData();
                }
            }
        }
        #endregion

        #region Grid validation
        private void GridView_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            e.Cancel = (GridView.SelectedItem as ACC_EINVOICE_HEADS)!.accounted.HasValue;
        }

        private void GridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType == Telerik.Windows.Controls.GridView.GridViewEditOperationType.Edit)
            {
                var data = e.Row.DataContext as ACC_EINVOICE_HEADS;

                if (data != null)
                {
                    var isValid = data.Supplier != null;

                    if (isValid)
                    {
                        if (ConfirmHandler.Confirm($"Assegnare la fattura n.{data.fattnum} al fornitore [{data.Supplier?.FullDescriptionSearchable}] ?"))
                        {
                            data.fattfor = data.Supplier?.abecod;
                            _dataContext.Update(data);
                        }
                    }
                    e.IsValid = isValid;
                    LoadData();
                }
            }
        }
        #endregion

        #region Grid functions
        private void rgInvoiceAttachmentsIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var row = (sender as RadGlyph)?.DataContext as ACC_EINVOICE_HEADS;

            if (row != null && row.fattalle.HasValue && row.fattalle.Value > 0)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AttachmentsWindowViewModel>();
                windowViewModel.Head = row;

                var wAttachments = new AttachmentsWindow(windowViewModel);
                wAttachments.Owner = Window.GetWindow(this);
                wAttachments.ShowDialog();
                LoadData();
            }
        }

        private void rgPrint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as RadGlyph)?.DataContext as ACC_EINVOICE_HEADS;

            if (item != null && !string.IsNullOrWhiteSpace(item.fattnomefileric))
            {
                string? APIKey = _dataContext.GetApiKey();

                byte[]? doc = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyGuidID}/{StorageHelper.INVOICE_RECEIVED_FOLDER}{item.fattnomefileric?.Trim()}");

                if (doc == null && !string.IsNullOrWhiteSpace(APIKey))
                {
                    // search on CERBERO for previous invoices
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                    var client = new CerberoRetrieveAPI.RetrieveInvoicesClient();
                    if (client.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        try
                        {
                            var request = new CerberoRetrieveAPI.ReceivedFileRetrieveRequest(APIKey, item.fattnomefileric?.Trim(), item.fattidsdi?.Trim());
                            var result = client.ReceivedFileRetrieveAsync(request).Result;
                            if (string.IsNullOrWhiteSpace(result.ReceivedFileRetrieveResult.ErrorCode))
                            {
                                doc = result.ReceivedFileRetrieveResult.Data;
                            }
                            else
                            {
                                Mouse.OverrideCursor = null;
                                ErrorHandler.Validation($"{result.ReceivedFileRetrieveResult.ErrorCode}-{result.ReceivedFileRetrieveResult.ErrorMessage}");
                            }
                        }
                        catch (Exception exc)
                        {
                            Exception? exi = exc;
                            StringBuilder sb = new StringBuilder();
                            do
                            {
                                sb.Append(exi.Message).Append("\n\n");
                                exi = exi.InnerException;
                            } while (exi != null);
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Validation(sb.ToString());
                            client.Close();
                        }
                        client.Close();
                    }
                }
                if (doc != null)
                {
                    var xmlD = XMLHelper.GetDocumentFromBytes(doc);
                    if (xmlD != null)
                    {
                        var xmlDecoded = XMLHelper.GetDocumentFullInfo(xmlD);

                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SelectPrintTypeWindowViewModel>();
                        windowViewModel.Filename = item.fattnomefileric?.Trim() ?? string.Empty;
                        windowViewModel.SendFormat = xmlDecoded.FormatoTrasmissione ?? string.Empty;
                        windowViewModel.Data = doc;

                        var nwSelectPrintType = new SelectPrintTypeWindow(windowViewModel);
                        nwSelectPrintType.Owner = Window.GetWindow(this);
                        Mouse.OverrideCursor = null;

                        if (nwSelectPrintType.ShowDialog() == true)
                        {
                            item.fattstampa = "S";
                            _dataContext.Update(item);
                            LoadData();
                        }
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("Impossibile recuperare il file XML ricevuto, file non presente e non recuperabile online per mancanza di abilitazione al Sistema Di Interscambio (SDI) dell'Agenzia delle Entrate");
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("Impossibile recuperare il file XML ricevuto, nome file o identificativo SDI non trovati");
            }
        }

        private void rgAbe_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var invoice = (sender as RadGlyph)?.DataContext as ACC_EINVOICE_HEADS;

            if (invoice != null)
            {
                var item = invoice.Supplier;

                if (item != null)
                {
                    item = _dataContext.GetData(item.abecod);

                    if (item != null)
                    {
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
                        if (item.abecfe == "C" || item.abecfe == "E" || item.abecfe == "P")
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

                        var abeWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ABEWindowViewModel>();
                        var data = _dataContext.GetData(item.abecod);

                        if (data != null)
                        {
                            abeWindowViewModel.Data = data;
                            abeWindowViewModel.CustomerNotes = _dataContext.GetNOTECLI(item.abecod);
                            abeWindowViewModel.SupplierNotes = _dataContext.GetNOTEFOR(item.abecod);
                            abeWindowViewModel.SupplierData = supplierData;
                            abeWindowViewModel.CustomerData = customerData;
                            abeWindowViewModel.CustomerCommercialData = customerCommercialData;
                            abeWindowViewModel.SupplierCommercialData = supplierCommercialData;
                            abeWindowViewModel.SupplierReferences = _dataContext.GetRFFTB00F(item.abecod);
                            abeWindowViewModel.CustomerReferences = _dataContext.GetANDEFRES(item.abecod);
                            abeWindowViewModel.CounterpartsRows = _dataContext.GetSUPPLIER_GROUPS(item.abecod);
                            abeWindowViewModel.IsInsert = false;

                            var wABE = VulpesServiceProvider.Provider.GetRequiredService<IABEWindowFactory>().Create(abeWindowViewModel);
                            Mouse.OverrideCursor = null;
                            wABE.Owner = Window.GetWindow(this);
                            if (wABE.ShowDialog() == true)
                                LoadData();
                        }

                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ABEWindowViewModel>();
                    }
                }
            }
        }

        private void rgCheckInvoice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var invoice = (sender as RadGlyph)?.DataContext as ACC_EINVOICE_HEADS;

            if (invoice != null)
            {
                if (invoice.Supplier != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CheckInvoiceEntranceWindowViewModel>();
                    windowViewModel.InvoiceID = invoice.fattnum;
                    windowViewModel.InvoiceDate = invoice.fattdata;
                    windowViewModel.VATNumber = invoice.fattpiva;
                    windowViewModel.SupplierID = invoice.Supplier.abecod;

                    var window = new CheckInvoiceEntranceWindow(windowViewModel);
                    window.ShowDialog();
                }
                else
                {
                    ErrorHandler.Validation($"Selezionare prima un fornitore in anagrafica per la fattura - {invoice.fattnum}");
                }
            }
        }

        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private void rdtPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        #endregion

        private void cmdViewAccounting_Click(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button)!.DataContext as ACC_EINVOICE_HEADS;

            if (selected != null && selected.fattannoreg.HasValue && selected.fattnumreg.HasValue)
            {
                var pnHead = _dataContext.GetPNTESTATA(selected.fattannoreg!.Value, selected.fattnumreg!.Value);

                if (pnHead != null)
                {
                    var causals = _dataContext.GetCAUCONT("*");
                    var codes = !string.IsNullOrWhiteSpace(pnHead.N1FLCF) ? _dataContext.GetABE(pnHead.N1FLCF) : null;

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNRIGHEWindowViewModel>();
                    windowViewModel.Head = pnHead;
                    windowViewModel.HeadSelectedCausal = causals?.Where(w => w.caucod == pnHead.pncaus).FirstOrDefault();
                    windowViewModel.IsInsert = false;

                    var wPNRIGHE = new PNRIGHEWindow(windowViewModel);
                    wPNRIGHE.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wPNRIGHE.ShowDialog() == true)
                        LoadData();
                }
            }
        }
    }
}
