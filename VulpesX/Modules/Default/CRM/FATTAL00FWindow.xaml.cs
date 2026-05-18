using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for FATTAL00FWindow.xaml
    /// </summary>
    public partial class FATTAL00FWindow : FluentDefaultWindow
    {
        private FATTAL00FWindowViewModel _dataContext;
        public FATTAL00FWindow(FATTAL00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            if (!string.IsNullOrWhiteSpace(_dataContext.Head.FTNUMFEL))
            {
                GridView.IsReadOnly = true;
                GridView.CanUserInsertRows = false;
                Title = $"Allegati alla fattura {_dataContext.Head.FTANNO}/{_dataContext.Head.FTNUOR} [sola lettura]";
            }
            else
            {
                Title = $"Allegati alla fattura {_dataContext.Head.FTANNO}/{_dataContext.Head.FTNUOR}";
            }
        }

        #region Buttons
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            // reset attachments
            _dataContext.Head.Attachments = _dataContext.GetFATTAL00Fs();

            Mouse.OverrideCursor = null;
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            if (_dataContext.UpdateAll())
            {
                Mouse.OverrideCursor = null;
                this.DialogResult = true;
            }
            else
            {
                Mouse.OverrideCursor = null; ErrorHandler.Validation("Errore imprevisto durante il salvataggio dei dati");
            }
        }

        #endregion

        #region Grid events
        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_dataContext.Head.FTNUMFEL))
            {
                var item = (sender as RadButton)?.DataContext as FATTAL00F;
                if (item != null)
                {
                    var fileDialog = new OpenFileDialog() { Multiselect = false };
                    if (fileDialog.ShowDialog() ?? false)
                    {
                        item.FTANAME = Path.GetFileName(fileDialog.FileName);
                        item.FTASIZE = new FileInfo(fileDialog.FileName).Length;
                        item.FullPath = fileDialog.FileName;
                    }
                }
            }
            e.Handled = true;
        }

        private void GridView_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var items = GridView.Items.Cast<FATTAL00F>();

            var data = new FATTAL00F
            {
                FTASOCI = _dataContext.Head.ftsoci,
                FTAANNO = _dataContext.Head.FTANNO,
                FTANUOR = _dataContext.Head.FTNUOR,
                FTAUID = Guid.NewGuid(),
                add_user = _dataContext.UserID,
                IsNotModified = false,
                FTANAME = string.Empty,
            };

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void GridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as FATTAL00F;
                if (item != null)
                {
                    var validated = _dataContext.Validate(item);

                    if (validated != null)
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                        GridView.CancelEdit();
                    }
                    item.IsNotModified = false;
                    e.IsValid = string.IsNullOrWhiteSpace(validated);
                }
                else
                {
                    e.IsValid = false;
                }
            }
            else
            {
                e.IsValid = true;
            }
            e.Handled = true;
        }

        private void GridView_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            GridView.ScrollIntoView(e.Row.Item, GridView.Columns[1]);
        }
        #endregion

        private void boDownload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = (sender as Border)?.DataContext as FATTAL00F;

                if (item != null)
                {
                    var folderDialog = new OpenFolderDialog();
                    if (folderDialog.ShowDialog() ?? false)
                    {
                        if (!string.IsNullOrWhiteSpace(folderDialog.FolderName))
                        {
                            string path = $@"{folderDialog.FolderName}\{item.FTANAME}";

                            var data = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.INVOICE_ATTACHMENTS_FOLDER}{item.FTAUID}");

                            if (data != null)
                            {
                                File.WriteAllBytes(path, data);
                                if (ConfirmHandler.Confirm($"Allegato scaricato correttamente nel seguente percorso:\n\n{path}\n\nVolete aprire la cartella dove e' avvenuto il salvataggio ?"))
                                {
                                    var proc = new ProcessStartInfo(path);
                                    proc.UseShellExecute = true;
                                    Process.Start(proc);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Validation(ex.Message);
            }
            e.Handled = true;
        }

        private void boOpen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = (sender as Border)?.DataContext as FATTAL00F;

                if (item != null)
                {
                    string path = $"{Path.GetTempPath()}{item.FTANAME}";

                    var data = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.INVOICE_ATTACHMENTS_FOLDER}{item.FTAUID}");

                    if (data != null)
                    {
                        File.WriteAllBytes(path, data);

                        var proc = new ProcessStartInfo(path);
                        proc.UseShellExecute = true;
                        Process.Start(proc);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Validation(ex.Message);
            }
            e.Handled = true;
        }
    }
}
