using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VulpesX.Models.Models;
using VulpesX.Models.Models.Accounting.eInvoice;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;

namespace VulpesX.Modules.Default.Accounting.Invoicing
{
    /// <summary>
    /// Interaction logic for AttachmentsWindow.xaml
    /// </summary>
    public partial class AttachmentsWindow : FluentDefaultWindow
    {
        private AttachmentsWindowViewModel _dataContext;
        public AttachmentsWindow(AttachmentsWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
            this.Title = $"Allegati alla fattura {_dataContext.Head.fattnum} di {_dataContext.Head.fattabers1}";

            _dataContext.Attachments = LoadAttachments(_dataContext.Head);
        }

        private ObservableCollection<DocumentAttachment>? LoadAttachments(ACC_EINVOICE_HEADS Item)
        {
            string? APIKey = _dataContext.GetApiKey();

            byte[]? doc = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyGuidID}/{StorageHelper.INVOICE_RECEIVED_FOLDER}{Item.fattnomefileric?.Trim()}");

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
                        var request = new CerberoRetrieveAPI.ReceivedFileRetrieveRequest(APIKey, Item.fattnomefileric?.Trim(), Item.fattidsdi?.Trim());

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
                        ErrorHandler.Show(sb.ToString());
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
                    return new ObservableCollection<DocumentAttachment>(XMLHelper.GetAttachments(xmlD));
                }
                return null;
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("Impossibile recuperare il file XML ricevuto, file non presente e non recuperabile online per mancanza di abilitazione al Sistema Di Interscambio (SDI) dell'Agenzia delle Entrate");
                return null;
            }
        }

        private void boDownload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = (sender as Border)?.DataContext as DocumentAttachment;

                if (item != null && item.Data != null)
                {
                    var folderDialog = new OpenFolderDialog
                    {
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        Title = "Seleziona la cartella di destinazione"
                    };

                    if (folderDialog.ShowDialog() == true)
                    {
                        if (!string.IsNullOrWhiteSpace(folderDialog.FolderName))
                        {
                            string path = $@"{folderDialog.FolderName}\{item.Name}";

                            File.WriteAllBytes(path, item.Data);

                            if (ConfirmHandler.Confirm($"Allegato salvato correttamente nel seguente percorso:\n\n{path}\n\nVolete aprire la cartella dove e' avvenuto il salvataggio ?"))
                            {
                                var proc = new ProcessStartInfo(path);
                                proc.UseShellExecute = true;
                                Process.Start(proc);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
            }
            e.Handled = true;
        }

        private void boOpen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = (sender as Border)?.DataContext as DocumentAttachment;

                if (item != null && item.Data != null)
                {
                    string path = $"{Path.GetTempPath()}{item.Name}";

                    File.WriteAllBytes(path, item.Data);

                    var proc = new ProcessStartInfo(path);
                    proc.UseShellExecute = true;

                    Process.Start(proc);
                }
            }
            catch (Exception ex)
            {
               ErrorHandler.Show(ex.Message);
            }
            e.Handled = true;
        }
    }
}
