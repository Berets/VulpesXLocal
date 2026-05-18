using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;
using VulpesX.DAL;
using VulpesX.Models.Models;
using VulpesX.Models.Models.Accounting.eInvoice;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;

namespace VulpesX.Modules.Default.Accounting.Invoicing
{
    /// <summary>
    /// Interaction logic for WaitImportWindow.xaml
    /// </summary>
    public partial class WaitImportWindow : FluentDefaultWindow
    {
        private WaitImportWindowViewModel _dataContext;

        private int goodFiles = 0;
        private int invoicesCount = 0;

        public WaitImportWindow(WaitImportWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // select files
            var fileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Fattura XML (*.xml)|*.xml|Fattura XML firmata (*.p7m)|*.p7m"
            };

            if (fileDialog.ShowDialog() == true)
            {
                using (BackgroundWorker bgwCheck = new BackgroundWorker())
                {
                    bgwCheck.DoWork += delegate (object? s, DoWorkEventArgs args)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            Mouse.OverrideCursor = Cursors.Wait;
                            pbCounter.Maximum = fileDialog.FileNames.Count();
                            pbCounter.Value = 1;
                        }));
                        foreach (var item in fileDialog.FileNames)
                        {
                            Dispatcher.Invoke(new Action(() => { tbCurrent.Text = item; }));
                            // save to cloud
                            var fs = new FileStream(item, FileMode.Open, FileAccess.Read);
                            byte[] data = new byte[fs.Length];
                            _ = fs.Read(data, 0, data.Length);
                            StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyGuid}/{(_dataContext.Direction == "S" ? StorageHelper.INVOICE_EXTERNAL_SENT_FOLDER : StorageHelper.INVOICE_RECEIVED_FOLDER)}{new FileInfo(item).Name}", data);
                            // add to table
                            var xml = XMLHelper.GetDocumentFromBytes(data);

                            if (xml != null)
                            {
                                var doc = XMLHelper.GetDocumentFullInfo(xml);
                                var att = XMLHelper.GetAttachments(xml);
                                var gxItem = new SdIItemGX()
                                {
                                    FileName = new FileInfo(item).Name,
                                    DataRicezione = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime().ToString("dd/MM/yyyy HH:mm:ss"),
                                    FullDocumentDecoded = doc,
                                    SdIID = null
                                };
                                var added = _dataContext.InsertLocal(gxItem, (short)att.Count);
                                invoicesCount += added;
                                if (added > 0)
                                    goodFiles++;
                            }

                            Dispatcher.Invoke(new Action(() => { pbCounter.Value += 1; }));
                        }
                        Dispatcher.Invoke(() =>
                        {
                            Mouse.OverrideCursor = null;
                            InfoHandler.Show($"Importati correttamente {goodFiles} files su {fileDialog.FileNames.Count()}, per un totale di {invoicesCount} fatture.");
                        });
                    };
                    bgwCheck.RunWorkerAsync();
                    bgwCheck.RunWorkerCompleted += (s, e) =>
                    {
                        this.DialogResult = true;
                    };
                }
            }
            else
            {
                this.DialogResult = false;
            }
        }

    }
}
