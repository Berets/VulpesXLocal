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
using VulpesX.DAL;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for ORDITALEWindow.xaml
    /// </summary>
    public partial class ORDITALEWindow : FluentDefaultWindow
    {
        private ORDITALEWindowViewModel _dataContext;
        public ORDITALEWindow(ORDITALEWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.Alerts = _dataContext.GetList();

            Loaded += (sender, e) =>
            {
                this.Title = $"Promemoria sull'ordine cliente {_dataContext.Head.PrintFullID}";
            };
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            foreach (var item in _dataContext.Alerts ?? new())
            {
                item.updatedUserID = _dataContext.UserID;
                item.updated = now;
            }
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
        #endregion

        #region Grid events
        private void GridView_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var items = GridView.Items.Cast<ORDITALE>();

            var data = new ORDITALE
            {
                otsoci = _dataContext.Head.otsoci,
                OTANNO = _dataContext.Head.OTANNO,
                OTNUOR = _dataContext.Head.OTNUOR,
                OTAID = items.Any() ? items.Max(max => max.OTAID) + 1 : 1,
                OTAATTI = true,
                addedUserID = _dataContext.UserID,
                added = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(),
                OTADESC = string.Empty,
            };

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void GridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as ORDITALE;
                if (item != null)
                {
                    var validated = _dataContext.Validate(item);
                    if (validated != null)
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                        GridView.CancelEdit();
                    }
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
