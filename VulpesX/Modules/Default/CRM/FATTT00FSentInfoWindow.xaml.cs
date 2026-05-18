using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Xml;
using VulpesX.Models.Models;
using VulpesX.Modules.Default.Accounting.Invoicing;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for FATTT00FSentInfoWindow.xaml
    /// </summary>
    public partial class FATTT00FSentInfoWindow : FluentDefaultWindow
    {
        private FATTT00FSentInfoWindowViewModel _dataContext;
        public FATTT00FSentInfoWindow(FATTT00FSentInfoWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            Title = $"Dettagli sull'invio del file XML riguardante la fattura {_dataContext.InvoiceHead.FTANNO}/{_dataContext.InvoiceHead.FTNUOR}";
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            bsyWait.IsBusy = true;

            string? APIKey = _dataContext.GetAZIENDA()?.azapikey?.Trim();

            if (!string.IsNullOrEmpty(APIKey))
            {
                string? fileName = _dataContext.InvoiceHead.FTNUMFEL?.Trim();
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                var client = new CerberoSendAPI.SendInvoicesClient();
                if (client.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    try
                    {
                        var request = new CerberoSendAPI.SentFileRetrieveRequest(APIKey, fileName);
                        var result = await client.SentFileRetrieveAsync(request);
                        if (!string.IsNullOrWhiteSpace(result.SentFileRetrieveResult.ErrorCode))
                        {
                            ErrorHandler.Validation($"{result.SentFileRetrieveResult.ErrorCode} - {result.SentFileRetrieveResult.ErrorMessage}");
                            Dispatcher.Invoke(() => { this.Close(); });
                        }

                        _dataContext.Result = result.SentFileRetrieveResult;
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
                        Dispatcher.Invoke(() => { this.Close(); });
                    }
                    client.Close();
                }
            }
            else
            {
                ErrorHandler.Validation("API Key non trovata");
            }
            bsyWait.IsBusy = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var folderDialog = new OpenFolderDialog();
                if (folderDialog.ShowDialog() ?? false)
                {
                    if (!string.IsNullOrWhiteSpace(folderDialog.FolderName))
                    {
                        string path = $@"{folderDialog.FolderName}\{_dataContext.InvoiceHead.FTNUMFEL}";

                        var data = _dataContext.Result?.Data;
                        if (data != null)
                        {
                            File.WriteAllBytes(path, data);
                            if (ConfirmHandler.Confirm($"Il file XML e' stato scaricato correttamente nel seguente percorso:\n\n{path}\n\nVolete aprire la cartella dove e' avvenuto il salvataggio ?"))
                            {
                                var proc = new ProcessStartInfo(folderDialog.FolderName);
                                proc.UseShellExecute = true;
                                Process.Start(proc);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Validation(ex.Message);
            }
        }

        private void cmdOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = $"{Path.GetTempPath()}{_dataContext.InvoiceHead.FTNUMFEL}";

                var data = _dataContext.Result?.Data;
                if (data != null)
                {
                    File.WriteAllBytes(path, data);
                    var proc = new ProcessStartInfo(path);
                    proc.UseShellExecute = true;
                    Process.Start(proc);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Validation(ex.Message);
            }
        }

        private void cmdRecreate_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm($"Confermate la rigenerazione del file XML per la fattura {_dataContext.InvoiceHead.FTANNO}/{_dataContext.InvoiceHead.FTNUOR} ?\n\nQuesto file non sara' comunque inviabile all'Agenzia delle Entrate, e' utilizzabile a soli fini di verifica."))
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                var fullPath = _dataContext.GenerateInvoiceXML(_dataContext.InvoiceHead.FTANNO, _dataContext.InvoiceHead.FTNUOR, true);
                Mouse.OverrideCursor = null;
                if (!string.IsNullOrWhiteSpace(fullPath))
                {
                    InfoHandler.Show($"File {fullPath} creato correttamente.\n\nCliccando OK verra' aperto in automatico");
                    var proc = new ProcessStartInfo(fullPath);
                    proc.UseShellExecute = true;
                    Process.Start(proc);
                }
                else
                {
                    ErrorHandler.Validation("Errore durante la creazione del file XML");
                }
            }
        }

        private void cmdPrint_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            if (_dataContext.Result?.Data != null)
            {
                var xmlD = XMLHelper.GetDocumentFromBytes(_dataContext.Result.Data);

                if (xmlD != null)
                {
                    XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlD.NameTable);
                    nsManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
                    nsManager.AddNamespace("p", "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2");
                    nsManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    var sendFormat = xmlD.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/FormatoTrasmissione", nsManager)?.InnerText;

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SelectPrintTypeWindowViewModel>();
                    windowViewModel.Filename = _dataContext.InvoiceHead.FTNUMFEL!.Trim();
                    windowViewModel.SendFormat = sendFormat!;
                    windowViewModel.Data = _dataContext.Result!.Data;

                    var nwSelectPrintType = new SelectPrintTypeWindow(windowViewModel);
                    nwSelectPrintType.Owner = Window.GetWindow(this);
                    var mousePosition = Mouse.GetPosition(System.Windows.Application.Current.MainWindow);
                    nwSelectPrintType.Left = mousePosition.X;
                    nwSelectPrintType.Top = mousePosition.Y;
                    Mouse.OverrideCursor = null;
                    nwSelectPrintType.ShowDialog();
                }
            }
            else
            { ErrorHandler.Validation("Impossibile recuperare il file XML inviato"); }
        }
    }
}
