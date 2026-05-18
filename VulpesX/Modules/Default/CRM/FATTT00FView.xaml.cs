using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using Telerik.Windows.Controls.ChartView;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Models.Models.CRM;
using VulpesX.Modules.Default.Accounting.Invoicing;
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.CRM;
using VulpesX.ViewModels.Modules.Default.General;
using VulpesX.WindowsFactory.Default.General;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for FATTT00FView.xaml
    /// </summary>
    public partial class FATTT00FView : UserControl
    {
        private FATTT00FViewModel _dataContext;
        private int _selectedPage;
        private bool _isFirstLoad = true;

        public FATTT00FView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<FATTT00FViewModel>();

            InitializeComponent();

            var today = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            rdpFromDate.SelectedValue = new DateTime(today.Year, today.Month, 1);
            rdpToDate.SelectedValue = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

            this.DataContext = _dataContext;

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                }
            };
            GridView.DataLoaded += (s, e) =>
            {
                rdpGrid.MoveToPage(_selectedPage);
                txtSearch_TextChanged(txtSearch, null);
            };
            GridView.SelectionChanged += (s, e) =>
            {
                if (GridView.SelectedItems != null)
                {
                    cmdPrintFinal.IsEnabled = !GridView.SelectedItems.Cast<FATTT00F>().Any(w => w.FTNUFD != 0) &&
                                                GridView.SelectedItems.Cast<FATTT00F>().GroupBy(g => g.FTCAUS).Count() == 1;
                }
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            LoadData();

            var companySettings = _dataContext.GetAZIENDA();

            cmdGenerateFile.Visibility = (companySettings?.azfatfile ?? false) ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void LoadData()
        {
            var fromDate = rdpFromDate.SelectedValue;
            var toDate = rdpToDate.SelectedValue;
            if (fromDate.HasValue && toDate.HasValue)
            {
                if (fromDate.Value <= toDate.Value)
                {
                    await _dataContext.Load(fromDate.Value, toDate.Value);

                    bsyGridTrend.IsBusy = true;

                    var invoicingSerie = new SplineSeries() { DisplayName = "Fatturato", LegendSettings = new SeriesLegendSettings() { Title = "Fatturato" } };
                    var creditSerie = new SplineSeries() { DisplayName = "Note credito", LegendSettings = new SeriesLegendSettings() { Title = "Note credito" } };
                    var balanceSerie = new SplineSeries() { DisplayName = "Saldo", LegendSettings = new SeriesLegendSettings() { Title = "Saldo" } };

                    using (BackgroundWorker bgwTrend = new BackgroundWorker())
                    {
                        bgwTrend.DoWork += delegate (object? s, DoWorkEventArgs args)
                        {
                            foreach (var item in (_dataContext.Trend?.Months ?? new()).OrderBy(o => o.Year).ThenBy(o => o.Month))
                            {
                                invoicingSerie.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint() { Category = item.CategoryLabel, Value = (double)item.Amount });
                                creditSerie.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint() { Category = item.CategoryLabel, Value = (double)item.CreditAmount });
                                balanceSerie.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint() { Category = item.CategoryLabel, Value = (double)item.Balance });
                            }
                        };
                        bgwTrend.RunWorkerAsync();
                        bgwTrend.RunWorkerCompleted += (s, e) =>
                        {
                            rccTrend.Series.Clear();
                            rccTrend.Series.Add(invoicingSerie);
                            rccTrend.Series.Add(creditSerie);
                            rccTrend.Series.Add(balanceSerie);
                            bsyGridTrend.IsBusy = false;
                        };
                    }
                }
                else
                {
                    ErrorHandler.Validation("La data fino alla quale mostrare le fatture deve essere maggiore o uguale alla datta dalla quale partire");
                }
            }
            else
            {
                ErrorHandler.Validation("Inserire entrambe le date");
            }

            _isFirstLoad = false;
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as FATTT00F;
            if (item != null)
            {
                item = _dataContext.Get(item.FTANNO, item.FTNUOR);

                if (item != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTT00FWindowViewModel>();
                    windowViewModel.Data = item;
                    windowViewModel.IsInsert = false;

                    var wFATTT00F = new FATTT00FWindow(windowViewModel);
                    Mouse.OverrideCursor = null;
                    wFATTT00F.Owner = Window.GetWindow(this);
                    if (wFATTT00F.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdEditRows_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as FATTT00F;

            if (item != null)
            {
                item = _dataContext.GetHead(item.FTANNO, item.FTNUOR);

                if (item != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTD00FWindowViewModel>();
                    windowViewModel.Head = item;

                    var wFATTD00F = new FATTD00FWindow(windowViewModel);
                    wFATTD00F.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wFATTD00F.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as FATTT00F;

            if (item != null)
            {
                item = _dataContext.Get(item.FTANNO, item.FTNUOR);

                if (item != null)
                {
                    if (item.FTNUFD <= 0)
                    {
                        if (ConfirmHandler.Confirm($"Confermate di voler eliminare la fattura provvisoria n. {item.FTANNO}/{item.FTNUOR} ?\nVerranno riaperti eventuali ordini o DDT collegati (ed eventuali registrazioni in caso di autofattura), ma non sara' possibile recuperare le informazioni eliminate, procedere ?"))
                        {
                            if (_dataContext.Delete(item))
                            {
                                LoadData();
                            }
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation("Impossibile eliminare una fattura stampata in definitivo");
                    }
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var yearWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<YearWindowViewModel>();

            var wAsk = new YearWindow(yearWindowViewModel);
            wAsk.Owner = Window.GetWindow(this);
            if (wAsk.ShowDialog() == true)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTT00FWindowViewModel>();
                windowViewModel.Data = new FATTT00F()
                {
                    ftsoci = _dataContext.CompanyID,
                    FTANNO = yearWindowViewModel.SelectedYear,
                    FTTIPO = "F",
                    FTDAOR = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(),
                    ftciva = "EUR",
                    FTCIDI = "UIC",
                    ftling = "IT"
                };
                windowViewModel.IsInsert = true;

                var wFATTT00F = new FATTT00FWindow(windowViewModel);
                Mouse.OverrideCursor = null;
                wFATTT00F.Owner = Window.GetWindow(this);
                if (wFATTT00F.ShowDialog() == true)
                {
                    var windowDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTD00FWindowViewModel>();
                    windowDetailViewModel.Head = windowViewModel.Data;

                    var wFATTD00F = new FATTD00FWindow(windowDetailViewModel);
                    wFATTD00F.Owner = Window.GetWindow(this);
                    wFATTD00F.ShowDialog();
                    LoadData();
                }
            }
        }

        private void cmdGenerateFile_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTPERSTXTWindowViewModel>();
            var wFATTPERSTXT = new FATTPERSTXTWindow(windowViewModel);
            wFATTPERSTXT.ShowDialog();
        }

        private void cmdEditCustomer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var selected = (sender as Button)?.DataContext as FATTT00F;

            if (selected != null && selected.FTCODC.HasValue && selected.FTCODC.Value > 0)
            {
                var item = _dataContext.GetABE(selected.FTCODC.Value);

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
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private void rdpFromDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isFirstLoad)
                LoadData();
        }

        private void rdpToDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isFirstLoad)
                LoadData();
        }
        #endregion

        #region Icons events
        private void rgSelfInvoiceIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var row = (sender as RadGlyph)?.DataContext as FATTT00F;

            if (row != null)
            {
                row = _dataContext.Get(row.FTANNO, row.FTNUOR);

                if (row != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTAUTWindowViewModel>();
                    windowViewModel.Data = row.SelfInvoice ?? new FATTAUT()
                    {
                        FTAUSC = row.ftsoci,
                        FTAUAN = row.FTANNO,
                        FTAUNUM = row.FTNUOR
                    };
                    windowViewModel.IsInsert = row.SelfInvoice == null;

                    var wFATTAUT = new FATTAUTWindow(windowViewModel);
                    wFATTAUT.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wFATTAUT.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void rgInvoiceAttachmentsIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var row = (sender as RadGlyph)?.DataContext as FATTT00F;

            if (row != null)
            {
                row = _dataContext.Get(row.FTANNO, row.FTNUOR);

                if (row != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTAL00FWindowViewModel>();
                    windowViewModel.Head = row;
                    var wFATTAL00F = new FATTAL00FWindow(windowViewModel);
                    wFATTAL00F.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wFATTAL00F.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void rgFE_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var row = (sender as RadGlyph)?.DataContext as FATTT00F;
            if (row != null)
            {
                if (row.FTNUFD > 0)
                {
                    if (string.IsNullOrWhiteSpace(row.SDISentStatus) || row.SDISentStatus == "R")
                    {
                        var settings = _dataContext.GetAZIENDA();

                        if (settings != null)
                        {
                            if (settings.azuseei)
                            {
                                // send with CERBERO
                                if (ConfirmHandler.Confirm($"Confermate la generazione e l'invio in formato elettronico della fattura {row.FTANNO}/{row.FTNUOR} ?"))
                                {
                                    Mouse.OverrideCursor = Cursors.Wait;
                                    // generate XML
                                    var fullPath = _dataContext.GenerateInvoiceXML(row.FTANNO, row.FTNUOR, false);

                                    if (!string.IsNullOrWhiteSpace(fullPath))
                                    {
                                        // ask if wanna sign before send
                                        bool signError = false;
                                        if (ConfirmHandler.Confirm("Volete firmare il file XML prima di inviarlo ?"))
                                        {
                                            var signResult = SignHelper.SignXMLCAdESBES(fullPath, fullPath + ".p7m");
                                            if (signResult.Item1)
                                                fullPath = fullPath + ".p7m";
                                            else
                                            {
                                                signError = true;
                                                ErrorHandler.Show(signResult.Item2 ?? "Errore generico nel firmare");
                                            }
                                        }
                                        if (!signError)
                                        {
                                            #region Send to SDI
                                            bool sendSuccess = false;
                                            string? sdiID = null;
                                            string? APIKey = settings.azapikey?.Trim();

                                            if (!string.IsNullOrEmpty(APIKey))
                                            {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                                                var client = new CerberoSendAPI.SendInvoicesClient();
                                                if (client.State != System.ServiceModel.CommunicationState.Faulted)
                                                {
                                                    try
                                                    {
                                                        var request = new CerberoSendAPI.SendRequest(APIKey, Path.GetFileName(fullPath), File.ReadAllBytes(fullPath), null, "X");
                                                        var result = client.SendAsync(request).Result;
                                                        if (!string.IsNullOrWhiteSpace(result.SendResult.ErrorCode))
                                                        {
                                                            ErrorHandler.Validation($"{result.SendResult.ErrorCode} - {result.SendResult.ErrorMessage}");
                                                        }
                                                        else
                                                        {
                                                            sendSuccess = true;
                                                            sdiID = result.SendResult.SdIID;
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
                                                        }
                                                        while (exi != null);
                                                        ErrorHandler.Validation(sb.ToString());
                                                        client.Close();
                                                    }
                                                    client.Close();
                                                }
                                                #endregion
                                                // write logs
                                                if (sendSuccess)
                                                {
                                                    row = _dataContext.GetSingle(row.FTANNO, row.FTNUOR);

                                                    if (row != null)
                                                    {
                                                        row.FTNUMFEL = Path.GetFileName(fullPath);
                                                        row.FTDATFEL = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                                                        if (_dataContext.Update(row))
                                                        {
                                                            _dataContext.InsertTFATTT00FLEVEL1(new TFATTT00FLEVEL1()
                                                            {
                                                                ftsoci = row.ftsoci,
                                                                FTANNO = row.FTANNO,
                                                                FTNUOR = row.FTNUOR,
                                                                ftfilename = Path.GetFileName(fullPath),
                                                                ftsdid = sdiID,
                                                                addedUserID = _dataContext.UserID
                                                            });
                                                            Mouse.OverrideCursor = null;
                                                            ErrorHandler.Validation($"Inviato con successo il file {Path.GetFileName(fullPath)} , assegnato ID da SDI: {sdiID}");
                                                        }
                                                        else
                                                        {
                                                            ErrorHandler.Validation($"Errore durante la registrazione dell'invio, il file e' stato spedito ma occorre contattare il supporto fornendo questi dati:\nFattura: {row.FTANNO}/{row.FTNUOR} Def. {row.FTNUFD}\nFile XML: {Path.GetFileName(fullPath)}\n SDI ID: {sdiID}\n Data ed ora: {VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime()}");
                                                        }
                                                        LoadData();
                                                    }
                                                }
                                                else
                                                {
                                                    ErrorHandler.Validation("Errore durante l'invio a SDI della fattura elettronica, invio non effettuato");
                                                }
                                            }
                                            else
                                            {
                                                ErrorHandler.Validation("API Key non presente");
                                            }
                                        }
                                        else
                                        {
                                            ErrorHandler.Validation("Errore durante la firma, verificare lettore e smartcard e riprovare");
                                        }
                                    }
                                    else
                                    {
                                        Mouse.OverrideCursor = null;
                                        ErrorHandler.Validation("Errore durante la creazione del file XML, invio non effettuato");
                                    }
                                }
                            }
                            else
                            {
                                // no CERBERO integration
                                if (ConfirmHandler.Confirm($"Confermate la generazione in locale della fattura {row.FTANNO}/{row.FTNUOR} ?"))
                                {
                                    Mouse.OverrideCursor = Cursors.Wait;
                                    // generate XML
                                    var fullPath = _dataContext.GenerateInvoiceXML(row.FTANNO, row.FTNUOR, false);
                                    if (!string.IsNullOrWhiteSpace(fullPath))
                                    {
                                        // ask if wanna sign
                                        bool signError = false;
                                        if (ConfirmHandler.Confirm("Volete firmare il file XML ?"))
                                        {
                                            var signResult = SignHelper.SignXMLCAdESBES(fullPath, fullPath + ".p7m");
                                            if (signResult.Item1)
                                                fullPath = fullPath + ".p7m";
                                            else
                                            {
                                                signError = true;
                                                ErrorHandler.Show(signResult.Item2 ?? "Errore generico nel firmare");
                                            }
                                        }
                                        if (!signError)
                                        {
                                            // write logs
                                            row = _dataContext.GetSingle(row.FTANNO, row.FTNUOR);

                                            if (row != null)
                                            {
                                                row.FTNUMFEL = Path.GetFileName(fullPath);
                                                row.FTDATFEL = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                                                if (_dataContext.Update(row))
                                                {
                                                    _dataContext.InsertTFATTT00FLEVEL1(new TFATTT00FLEVEL1()
                                                    {
                                                        ftsoci = row.ftsoci,
                                                        FTANNO = row.FTANNO,
                                                        FTNUOR = row.FTNUOR,
                                                        ftfilename = Path.GetFileName(fullPath),
                                                        ftsdid = "MANUALE",
                                                        addedUserID = _dataContext.UserID
                                                    });

                                                    Mouse.OverrideCursor = null;
                                                    InfoHandler.Show($"Creato con successo il file {fullPath}");

                                                    // open folder
                                                    var folder = Path.GetDirectoryName(fullPath);

                                                    if (!string.IsNullOrEmpty(folder))
                                                    {
                                                        var proc = new ProcessStartInfo(folder);
                                                        proc.UseShellExecute = true;
                                                        Process.Start(proc);
                                                    }
                                                }
                                                else
                                                {
                                                    ErrorHandler.Validation($"Errore durante la registrazione dell'invio, il file e' stato generato ma occorre contattare il supporto fornendo questi dati:\nFattura: {row.FTANNO}/{row.FTNUOR} Def. {row.FTNUFD}\nFile XML: {Path.GetFileName(fullPath)}\n Data ed ora: {VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime()}");
                                                }
                                                LoadData();
                                            }
                                        }
                                        else
                                        {
                                            ErrorHandler.Validation("Errore durante la firma, verificare lettore e smartcard e riprovare");
                                        }
                                    }
                                    else
                                    {
                                        Mouse.OverrideCursor = null;
                                        ErrorHandler.Validation("Errore durante la creazione del file XML, invio non effettuato");
                                    }
                                }
                            }
                        }
                        else
                        {
                            ErrorHandler.Validation("Impossibile trovare personalizzazione societaria");
                        }
                    }
                    else
                    {
                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTT00FSentInfoWindowViewModel>();
                        windowViewModel.InvoiceHead = row;

                        var wSentFileInfo = new FATTT00FSentInfoWindow(windowViewModel);
                        wSentFileInfo.Owner = Window.GetWindow(this);
                        wSentFileInfo.ShowDialog();
                    }
                }
                else
                {
                    ErrorHandler.Validation("Impossibile inviare una fattura non ancora stampata in definitivo");
                }
            }
        }

        private void rgPrint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as RadGlyph)?.DataContext as FATTT00F;

            if (item != null)
            {
                item = _dataContext.GetPrintFull(item.FTANNO, item.FTNUOR);

                if (item != null)
                {
                    // check bank is present
                    if (item.FTABIB.HasValue && item.FTCABB.HasValue && item.FTABIB.Value > 0 && item.FTCABB.Value > 0)
                    {
                        // plafond check only if not printed final and not accounted
                        if (string.IsNullOrWhiteSpace(item.FTFLA1) && string.IsNullOrWhiteSpace(item.FTFLA2))
                        {
                            var invoicePlafondTotal = item.Rows?.Where(w => w.HasPlafond).Sum(sum => sum.Amount);
                            if (invoicePlafondTotal > 0)
                            {
                                var plafond = _dataContext.GetLast(item.FTCODC!.Value, item.FTANNO, item.FTDAOR!.Value);

                                if (plafond?.AmountAvailable < invoicePlafondTotal)
                                {
                                    InfoHandler.Show($"ATTENZIONE!\nL'importo disponibile residuo per la dichiarazione d'intento e' insufficiente per la fattura selezionata.\n\nImporto totale plafond: {plafond.cliimpesefino}\nImporto plafond utilizzato: {plafond.cliimpfattprog}\nImporto plafond impegnato: {plafond.cliimpfattprovv}\nTOTALE PLAFOND DISPONIBILE: {plafond.AmountAvailable}");
                                }
                            }
                        }

                        
                        var reportData = _dataContext.PrintInvoice(item);
                        if (reportData != null)
                            ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_CRM, Constants.REPORT_TYPE_INVOICE, _dataContext.CompanyID, reportData, $"Fattura n.{item.PrintFullID}", item.PrintFilename, true);
                        else
                            ErrorHandler.Validation($"Impossibile trovare la fattura {item.FTANNO}/{item.FTNUOR}");
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("Impossibile stampare in assenza della banca di riferimento");
                    }
                }
            }
        }
        #endregion

        #region Functions
        private void cmdPrintFinal_Click(object sender, RoutedEventArgs e)
        {
            if (GridView.SelectedItems != null && GridView.SelectedItems.Count > 0)
            {
                if (GridView.SelectedItems.Cast<FATTT00F>().Select(s => s.FTANNO).Distinct().Count() == 1)
                {
                    // check numerators existence
                    bool allOK = true;
                    var causalCheck = _dataContext.GetCAUFAT00F(GridView.SelectedItems.Cast<FATTT00F>().First().FTCAUS ?? string.Empty);

                    if (causalCheck == null || string.IsNullOrWhiteSpace(causalCheck.fatnmr))
                    {
                        ErrorHandler.Validation($"La causale\n [{causalCheck?.FullDescriptionSearchable}]\n non ha impostato un numeratore per la stampa definitiva");
                        allOK = false;
                    }
                    else
                    {
                        foreach (var item in GridView.SelectedItems.Cast<FATTT00F>())
                        {
                            // check prices
                            if (!_dataContext.CheckPrices(item.FTANNO, item.FTNUOR))
                            {
                                ErrorHandler.Validation($"Ci sono delle righe della fattura {item.PrintFullID} con importo a 0");
                                allOK = false;
                                break;
                            }
                        }
                    }

                    if (allOK && ConfirmHandler.Confirm($"Confermate la stampa definitiva delle {GridView.SelectedItems.Count} fatture selezionate ?\nQuesta operazione non e' annullabile!"))
                    {
                        var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                        DateTime newDate = now;

                        var windowAskDateViewModel = VulpesServiceProvider.Provider.GetRequiredService<AskInvoiceFinalDateWindowViewModel>();
                        windowAskDateViewModel.Year = GridView.SelectedItems.Cast<FATTT00F>().First().FTANNO;
                        windowAskDateViewModel.NumeratorID = causalCheck!.fatnmr ?? string.Empty;
                        windowAskDateViewModel.SelectedDate = now;

                        var askWindow = new AskInvoiceFinalDateWindow(windowAskDateViewModel);
                        askWindow.Owner = Window.GetWindow(this);
                        if (askWindow.ShowDialog() == true && windowAskDateViewModel.SelectedDate.HasValue)
                        {
                            Mouse.OverrideCursor = Cursors.Wait;
                            int failed = 0;

                            foreach (var inv in GridView.SelectedItems.Cast<FATTT00F>())
                            {
                                var invoice = _dataContext.GetPrintFull(inv.FTANNO, inv.FTNUOR);

                                if (invoice != null)
                                {
                                    var validatedMessage = _dataContext.Validate(invoice);
                                    if (string.IsNullOrWhiteSpace(validatedMessage))
                                    {
                                        #region check for plafond
                                        bool canGo = false;
                                        var plafondTotal = invoice.Rows?.Where(w => w.HasPlafond).Sum(sum => sum.NetPrice) ?? 0;
                                        ACC_PLAFOND? plafond = null;
                                        decimal neededAmount = 0;
                                        if (plafondTotal > 0)
                                        {
                                            var plafondSettings = _dataContext.GetACC_PLAFOND_PARMS();
                                            if (plafondTotal > plafondSettings?.limit_amount)
                                            {
                                                plafond = _dataContext.GetLast(invoice.FTCODC!.Value, invoice.FTDAOR!.Value.Year, invoice.FTDAOR.Value.Date);

                                                neededAmount = plafondTotal + (plafond?.cliimpfattprovv ?? 0) + (plafond?.cliimpfattprog ?? 0);

                                                if (neededAmount <= plafond?.cliimpesefino)
                                                {
                                                    canGo = true;
                                                }
                                                else
                                                {
                                                    Mouse.OverrideCursor = null;
                                                    failed++;
                                                    ErrorHandler.Validation($"Impossibile stampare la fattura {invoice.PrintFullID}, non c'è sufficiente disponibilità sul plafond per completare l'operazione.\n\nTotale plafond: {plafond?.cliimpesefino?.ToString("N2")}\nImporto plafond fattura corrente: {plafondTotal.ToString("N2")}\nTotale necessario: {neededAmount.ToString("N2")}\nImporto plafond mancante: {(neededAmount - plafond?.cliimpesefino)?.ToString("N2")}");
                                                    canGo = false;
                                                    Mouse.OverrideCursor = Cursors.Wait;
                                                }
                                            }
                                        }
                                        #endregion
                                        if ((plafondTotal > 0 && canGo) || plafondTotal == 0)
                                        {
                                            // get definitive id from CAUFAT00F causal 
                                            var numerator = _dataContext.GetNumerator(invoice.FTANNO, new GenericIDDescription() { ID = invoice.Causal!.fatnmr, Description = invoice.Causal.fatdes });
                                            if (numerator > 0)
                                            {
                                                // update head
                                                invoice.FTNUFD = numerator;
                                                invoice.FTDAOR = windowAskDateViewModel.SelectedDate.Value;
                                                invoice.FTFLA1 = "1";
                                                if (_dataContext.Update(invoice))
                                                {
                                                    // update details
                                                    foreach (var row in _dataContext.GetList(invoice.FTANNO, invoice.FTNUOR) ?? new())
                                                    {
                                                        row.FDNUFD = numerator;
                                                        _dataContext.Update(row);
                                                    }
                                                    #region increment temp plafond
                                                    if (canGo)
                                                    {
                                                        if (plafond != null)
                                                        {
                                                            if (plafond.cliimpfattprovv == null)
                                                                plafond.cliimpfattprovv = 0;

                                                            plafond.cliimpfattprovv += plafondTotal;
                                                            _dataContext.Update(plafond);
                                                        }
                                                    }
                                                    #endregion
                                                    #region print
                                                    var reportData = _dataContext.PrintInvoice(invoice);
                                                    if (reportData != null)
                                                    {
                                                        var companySettings = _dataContext.GetAZIENDA();
                                                        var fullPath = ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_CRM, Constants.REPORT_TYPE_INVOICE, _dataContext.CompanyID, reportData, $"Fattura n.{invoice.PrintFullID}", invoice.PrintFilename, true)?.FullPath;
                                                        if (!string.IsNullOrWhiteSpace(fullPath) && companySettings != null && companySettings.AZATTINV)
                                                        {
                                                            // save PDF into invoice attachments
                                                            var attachment = new FATTAL00F()
                                                            {
                                                                FTASOCI = invoice.ftsoci,
                                                                FTAANNO = invoice.FTANNO,
                                                                FTANUOR = invoice.FTNUOR,
                                                                FTANAME = invoice.PrintFilename,
                                                                FTASIZE = new FileInfo(fullPath!).Length,
                                                                add_user = _dataContext.UserID,
                                                                FTAUID = Guid.NewGuid()
                                                            };
                                                            // upload
                                                            var uploadResult = StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.INVOICE_ATTACHMENTS_FOLDER}{attachment.FTAUID}", File.ReadAllBytes(fullPath));
                                                            // save info
                                                            if (string.IsNullOrWhiteSpace(uploadResult))
                                                            {
                                                                _dataContext.InsertFATTAL00F(attachment);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Mouse.OverrideCursor = null;
                                                        failed++;
                                                        ErrorHandler.Validation($"Impossibile trovare la fattura {invoice.PrintFullID}");
                                                        Mouse.OverrideCursor = Cursors.Wait;
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    Mouse.OverrideCursor = null;
                                                    failed++;
                                                    ErrorHandler.Validation("Errore durante l'aggiornamento della testata");
                                                    Mouse.OverrideCursor = Cursors.Wait;
                                                }
                                            }
                                            else
                                            {
                                                Mouse.OverrideCursor = null;
                                                ErrorHandler.Validation("Errore durante il recupero del numero definitivo");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        failed++;
                                        ErrorHandler.Validation(validatedMessage);
                                    }
                                }
                            }
                            Mouse.OverrideCursor = null;
                            if (failed == 0)
                                InfoHandler.Show($"Tutte le {GridView.SelectedItems.Count} fatture selezionate sono state stampate definitivamente");
                            else
                                ErrorHandler.Validation($"{failed} fatture NON sono state stampate in definitivo\n{(GridView.SelectedItems.Count - failed)} fatture sono state stampate correttamente");
                            LoadData();
                        }
                    }
                }
                else
                {
                    ErrorHandler.Validation("Impossibile procedere selezionando fatture di anni differenti");
                }
            }
        }
        #endregion

        #region Context menu
        private void cmGrid_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            #region Accounting
            if (GridView.SelectedItems.Count == 1)
            {
                var item = GridView.SelectedItem as FATTT00F;
                if (item != null)
                {
                    rmiAccounting.IsEnabled = item.FTFLA1 == "1" && item.FTFLA2 != "1";
                    if (rmiAccounting.IsEnabled)
                    {
                        rmiAccounting.Header = $"Contabilizza la fattura n. {item.FTANNO}/{item.FTNUOR} def. {item.FTNUFD}";
                    }
                    else
                    {
                        if (item.FTFLA1 != "1")
                            rmiAccounting.Header = $"Fattura n. {item.FTANNO}/{item.FTNUOR} def. {item.FTNUFD} non contabilizzabile, manca stampa definitiva";
                        else
                            rmiAccounting.Header = $"Fattura n. {item.FTANNO}/{item.FTNUOR} def. {item.FTNUFD} gia' contabilizzata";
                    }
                }
            }
            else if (GridView.SelectedItems.Count > 1)
            {
                if (GridView.SelectedItems.Count == GridView.SelectedItems.Cast<FATTT00F>().Where(w => w.FTFLA1 == "1" && w.FTFLA2 != "1").Count())
                {
                    rmiAccounting.IsEnabled = true;
                    rmiAccounting.Header = $"Contabilizza le {GridView.SelectedItems.Count} fatture selezionate";
                }
                else
                {
                    rmiAccounting.IsEnabled = false;
                    rmiAccounting.Header = $"Impossibile contabilizzare le {GridView.SelectedItems.Count} fatture selezionate, perchè alcune risultano già contabilizzate o non ancora stampate in definitivo";
                }
            }
            else
            {
                rmiAccounting.IsEnabled = false;
            }
            #endregion

            #region Not to be sent
            rmiSendable.Visibility = Visibility.Collapsed;
            if (GridView.SelectedItems.Count == 1)
            {
                var item = GridView.SelectedItem as FATTT00F;

                if (item != null)
                {
                    if (string.IsNullOrWhiteSpace(item.ftsdiid) && string.IsNullOrWhiteSpace(item.FTNUMFEL))
                    {
                        rmiSendable.Visibility = Visibility.Visible;
                        rmiSendable.Header = $"Imposta come 'da non inviare' la fattura n.{item.DefinitiveID}";
                        rmiSendable.Tag = "N";
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(item.ftsdiid) && string.IsNullOrWhiteSpace(item.FTNUMFEL) && item.ftsdiid == "#")
                        {
                            rmiSendable.Visibility = Visibility.Visible;
                            rmiSendable.Header = $"Imposta come 'da inviare' la fattura n.{item.DefinitiveID}";
                            rmiSendable.Tag = "I";
                        }
                    }
                }
            }
            #endregion
        }

        private void rmiAccounting_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItems != null && GridView.SelectedItems.Count > 0)
            {
                List<FATTT00F> selected = new List<FATTT00F>();
                Parallel.ForEach(GridView.SelectedItems.Cast<FATTT00F>(), item =>
                {
                    var data = _dataContext.GetFull(item.FTANNO, item.FTNUOR);

                    if (data != null)
                        selected.Add(data);
                });
                if (selected.All(all => all.FTFLA1 == "1" && all.FTFLA2 != "1"))
                {
                    var windowYearViewModel = VulpesServiceProvider.Provider.GetRequiredService<AccountingYearWindowViewModel>();

                    var windowYear = new AccountingYearWindow(windowYearViewModel);
                    if (windowYear.ShowDialog() ?? false)
                    {
                        int eseini = windowYearViewModel.AccountingYear!.eseini!.Value;
                        int failed = 0;
                        foreach (var item in selected)
                        {
                            if (item.FTDAOR!.Value >= new DateTime(windowYearViewModel.AccountingYear.eseann, eseini, 1))
                            {
                                // come on
                                if (!_dataContext.Accounting(item, windowYearViewModel.AccountingYear))
                                {
                                    failed++;
                                    ErrorHandler.Validation($"Fattura {item.PrintFullID} NON contabilizzata per un errore imprevisto");
                                }
                            }
                            else
                            {
                                failed++;
                                ErrorHandler.Validation($"Fattura {item.PrintFullID} NON contabilizzata perchè in caso di sovrapposizione di esercizio l'anno IVA deve essere precedente all'anno contabile se la data fattura e' successiva o uguale al 1 Gennaio {windowYearViewModel.AccountingYear.eseann}");
                                Mouse.OverrideCursor = Cursors.Wait;
                            }
                        }

                        if (failed == 0)
                            InfoHandler.Show($"Tutte le {selected.Count} fatture selezionate sono state contabilizzate correttamente");
                        else
                            ErrorHandler.Validation($"{failed} fatture NON sono state contabilizzate\n{(selected.Count - failed)} fatture sono state contabilizzate correttamente");
                    }

                    LoadData();
                }
                else
                {
                    ErrorHandler.Validation("Impossibile contabilizzare una o più fatture poiché risultano già contabilizzate (probabilmente da un altro utente).\nRicaricare i dati e riprovare");
                }
            }
        }
        #endregion

        private void rmiSendable_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItems != null && GridView.SelectedItems.Count == 1)
            {
                var item = GridView.SelectedItem as FATTT00F;
                if (item != null)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    item = _dataContext.GetHead(item.FTANNO, item.FTNUOR);

                    if (item != null)
                    {
                        if ((e.OriginalSource as RadMenuItem)?.Tag.ToString() == "N")
                        {
                            // flag as not sendable
                            item.ftsdiid = "#";
                            _dataContext.Update(item);
                        }
                        else
                        {
                            // reset to sendable
                            item.ftsdiid = null;
                            _dataContext.Update(item);
                        }
                    }
                    Mouse.OverrideCursor = null;
                    LoadData();
                }
            }
        }
    }
}
