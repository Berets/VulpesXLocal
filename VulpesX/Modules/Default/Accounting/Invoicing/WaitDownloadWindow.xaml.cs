using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;

namespace VulpesX.Modules.Default.Accounting.Invoicing
{
    /// <summary>
    /// Interaction logic for WaitDownloadWindow.xaml
    /// </summary>
    public partial class WaitDownloadWindow : FluentDefaultWindow
    {
        private WaitDownloadWindowViewModel _dataContext;

        private int goodFiles = 0;
        private int invoicesCount = 0;

        public WaitDownloadWindow(WaitDownloadWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();
            
            this.DataContext = _dataContext;

            using (BackgroundWorker bgwCheck = new BackgroundWorker())
            {
                bgwCheck.DoWork += delegate (object? s, DoWorkEventArgs args)
                {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                    var client = new CerberoRetrieveAPI.RetrieveInvoicesClient();
                    if (client.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        try
                        {
                            var request = new CerberoRetrieveAPI.RetrieveVulpesXRequest(_dataContext.ApiKey);
                            var result = client.RetrieveVulpesXAsync(request).Result;
                            if (!string.IsNullOrWhiteSpace(result.RetrieveVulpesXResult.ErrorCode))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    ErrorHandler.Validation($"{result.RetrieveVulpesXResult.ErrorCode} - {result.RetrieveVulpesXResult.ErrorMessage}");
                                });
                            }
                            else
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    pbCounter.Maximum = result.RetrieveVulpesXResult.FileList.Count();
                                    pbCounter.Value = 1;
                                }));
                                foreach (var item in result.RetrieveVulpesXResult.FileList)
                                {
                                    Dispatcher.Invoke(new Action(() => { tbCurrent.Text = item.FileName; }));
                                    var added = _dataContext.Insert(item);
                                    invoicesCount += added;
                                    if (added > 0)
                                        goodFiles++;
                                    Dispatcher.Invoke(new Action(() => { pbCounter.Value += 1; }));
                                }
                                Dispatcher.Invoke(() =>
                                {
                                    Mouse.OverrideCursor = null;
                                    if (result.RetrieveVulpesXResult.FileList.Count() > 0)
                                        InfoHandler.Show($"Scaricati correttamente {goodFiles} files su {result.RetrieveVulpesXResult.FileList.Count()}, per un totale di {invoicesCount} fatture.");
                                    else
                                        InfoHandler.Show($"Nessun file da scaricare");
                                });
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
                            ErrorHandler.Show(sb.ToString());
                            client.Close();
                        }
                        client.Close();
                    }
                };
                bgwCheck.RunWorkerAsync();
                bgwCheck.RunWorkerCompleted += (s, e) =>
                {
                    this.DialogResult = true;
                };
            }
        }
    }
}
