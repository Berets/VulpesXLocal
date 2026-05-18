using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Controls.Helpers;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Assets;

namespace VulpesX.Modules.Default.Assets
{
    /// <summary>
    /// Interaction logic for ASSETAL00FWindow.xaml
    /// </summary>
    public partial class ASSETAL00FWindow : FluentDefaultWindow
    {
        private ASSETAL00FWindowViewModel _dataContext;
        public ASSETAL00FWindow(ASSETAL00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            Loaded += (sender, e) =>
            {
                this.Title = $"Allegati dell'asset {_dataContext.Head.FullDescriptionSearchable}";
                if (!_dataContext.IsReadonly)
                {
                    this.Title += " - [sola lettura]";
                    GridView.CanUserInsertRows = false;
                }
            };
        }

        #region Buttons
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
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("Errore imprevisto durante il salvataggio dei dati");
            }
        }

        #endregion

        #region Grid events
        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.Head.canceled == null)
            {
                var item = (sender as RadButton)?.DataContext as ASSETAL00F;

                if (item != null)
                {
                    var fileDialog = new OpenFileDialog() { Multiselect = false };
                    if (fileDialog.ShowDialog() ?? false)
                    {
                        item.document_name = Path.GetFileName(fileDialog.FileName);
                        item.document_size = new FileInfo(fileDialog.FileName).Length;
                        item.FullPath = fileDialog.FileName;
                    }
                }
            }
            e.Handled = true;
        }

        private void GridView_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var items = GridView.Items.Cast<ASSETAL00F>();

            var data = new ASSETAL00F
            {
                company_id = _dataContext.CompanyID,
                id = _dataContext.Head.id,
                add_user = _dataContext.UserID,
                document_name = string.Empty,
                added = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(),
                IsNotModified = false,
                document_id = Guid.NewGuid(),
            };

            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void GridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as ASSETAL00F;
                if (item != null)
                {
                    var validated = _dataContext.Validate(item);
                    if (!string.IsNullOrEmpty(validated))
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                        GridView.CancelEdit();
                    }
                    item.IsNotModified = false;
                    e.IsValid = string.IsNullOrWhiteSpace(validated);
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

        #region Grid icons
        private void boDownload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = (sender as Border)?.DataContext as ASSETAL00F;

                if (item != null)
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
                            string path = $@"{folderDialog.FolderName}\{item.document_name}";

                            var data = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.ASSETS_ATTACHMENTS_FOLDER}{item.document_id}");

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
                ErrorHandler.Show(ex.Message.ToString());
            }
            e.Handled = true;
        }

        private void boOpen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = (sender as Border)?.DataContext as ASSETAL00F;

                if (item != null)
                {
                    string path = $"{Path.GetTempPath()}{item.document_name}";

                    var data = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.ASSETS_ATTACHMENTS_FOLDER}{item.document_id}");

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
                ErrorHandler.Show(ex.Message.ToString());
            }
            e.Handled = true;
        }
        #endregion
    }
}
