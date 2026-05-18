using Microsoft.Extensions.DependencyInjection;
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
using System.Windows.Threading;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for ORDITAL00FWindow.xaml
    /// </summary>
    public partial class ORDITAL00FWindow : FluentDefaultWindow
    {
        private ORDITAL00FWindowViewModel _dataContext;
        public ORDITAL00FWindow(ORDITAL00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.HasMergedSigns = (UserContext.Instance!.ACCESS!.Roles?.canOrdersSignCommercial ?? false) && (UserContext.Instance!.ACCESS!.Roles?.canOrdersSignTech ?? false);

            Loaded += (sender, e) =>
            {
                this.Title = $"Allegati all'ordine cliente {_dataContext.Head.PrintFullID}";
                if (_dataContext.IsReadonly)
                {
                    this.Title += " - [sola lettura]";
                    GridView.CanUserInsertRows = false;
                }
            };
        }

        #region Buttons
        private void cmdCancel_Click(object? sender, RoutedEventArgs? e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            // reset attachments
            _dataContext.Head.Attachments = _dataContext.GetList();

            Mouse.OverrideCursor = null;

            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            // check if already signed
            bool proceed = true;
            if ((_dataContext.Head.OTFICO.HasValue || _dataContext.Head.OTFITE.HasValue) && !_dataContext.HasMergedSigns)
            {
                if (ConfirmHandler.Confirm("Questo ordine ha già delle firme apposte, proseguendo verranno annullate e bisognerà richiederle nuovamente, procedere ?"))
                {
                    // clear signs
                    _dataContext.Head.OTFITE = null;
                    _dataContext.Head.OTFITEUSR = null;
                    _dataContext.Head.OTFICO = null;
                    _dataContext.Head.OTFICOUSR = null;
                    _dataContext.Head.OTINFI = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                    _dataContext.Head.OTINFIUSR = _dataContext.UserID;
                }
                else
                {
                    proceed = false;
                }
            }
            if (proceed)
            {
                if (_dataContext.UpdateAll())
                {
                    Mouse.OverrideCursor = null;
                    this.DialogResult = true;
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("Errore imprevisto durante il salvataggio dei dati");
                }
            }
            else
            {
                cmdCancel_Click(null, null);
            }
        }

        #endregion

        #region Grid events
        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.Head.flgchi != "C" && _dataContext.Head.flgchi != "P" && _dataContext.Head.canceled == null)
            {
                var item = (sender as RadButton)?.DataContext as ORDITAL00F;

                if (item != null)
                {
                    var fileDialog = new OpenFileDialog() { Multiselect = false };
                    if (fileDialog.ShowDialog() ?? false)
                    {
                        item.OTANAME = Path.GetFileName(fileDialog.FileName);
                        item.OTASIZE = new FileInfo(fileDialog.FileName).Length;
                        item.FullPath = fileDialog.FileName;
                    }
                }
            }
            e.Handled = true;
        }

        private void GridView_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var items = GridView.Items.Cast<ORDITAL00F>();

            var data = new ORDITAL00F
            {
                otasoci = _dataContext.Head.otsoci,
                OTAANNO = _dataContext.Head.OTANNO,
                OTANUOR = _dataContext.Head.OTNUOR,
                OTAUID = Guid.NewGuid(),
                add_user = _dataContext.UserID,
                added = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(),
                IsNotModified = false,
                OTANAME = string.Empty,
            };

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void GridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as ORDITAL00F;

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

        private void GridView_Deleting(object sender, GridViewDeletingEventArgs e)
        {
            e.Cancel = !ConfirmHandler.Confirm($"Vuoi eliminare gli allegati selezionati ?");
        }
        #endregion

        #region Grid functions
        private void boDownload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = (sender as Border)?.DataContext as ORDITAL00F;

                if (item != null)
                {
                    var folderDialog = new OpenFolderDialog();
                    if (folderDialog.ShowDialog() ?? false)
                    {
                        if (!string.IsNullOrWhiteSpace(folderDialog.FolderName))
                        {
                            string path = $@"{folderDialog.FolderName}\{item.OTANAME}";

                            var data = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.ORDERS_ATTACHMENTS_FOLDER}{item.OTAUID}");

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
                var item = (sender as Border)?.DataContext as ORDITAL00F;
                if (item != null)
                {
                    string path = $"{Path.GetTempPath()}{item.OTANAME}";

                    var data = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.ORDERS_ATTACHMENTS_FOLDER}{item.OTAUID}");

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
        #endregion

     
    }
}
