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
using System.Windows.Shapes;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.General;

namespace VulpesX.Modules.Default.General
{
    /// <summary>
    /// Interaction logic for ARTAttachWindow.xaml
    /// </summary>
    public partial class ARTAttachWindow : FluentDefaultWindow
    {
        private ARTAttachWindowViewModel _dataContext;
        public ARTAttachWindow(ARTAttachWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
            LoadData();
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }


        #region Buttons
        private void btnOpen_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = rgvList.SelectedItem as tab_articolo_immagine;
            if (selected != null)
            {
                try
                {
                    if (File.Exists(selected.Url))
                    {
                        var process = new Process();
                        process.StartInfo = new ProcessStartInfo(selected.Url)
                        {
                            UseShellExecute = true
                        };
                        process.Start();
                    }
                    else
                    {
                        ErrorHandler.Validation(string.Format("File non trovato - {0}", selected.Url));
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Validation(ex.ToString());
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Multiselect = true;
            dialog.ValidateNames = true;
            dialog.FileOk += (ss, ee) =>
            {
                if (ss != null && ss is OpenFileDialog)
                {
                    foreach (var fn in (ss as OpenFileDialog)!.FileNames)
                    {
                        var info = new FileInfo(fn);

                        if (string.IsNullOrEmpty(fn) || fn.Contains(".lnk"))
                        {
                            MessageBox.Show("Please select a valid File");
                            ee.Cancel = true;
                        }

                        var id = (rgvList.Items.Count > 0) ? rgvList.Items.Cast<tab_articolo_allegato>().Max(o => o.ID) : 0;

                        var attach = new tab_articolo_allegato
                        {
                            SocietaID = _dataContext.CompanyID,
                            ArticoloID = _dataContext.Data.ID,
                            ID = ++id,
                            Uri = fn,
                        };

                        _dataContext.Insert(attach);
                    }
                }

                LoadData();
            };

            dialog.ShowDialog();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as tab_articolo_allegato;

            if (item != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.Multiselect = false;
                dialog.ValidateNames = true;
                dialog.FileOk += (ss, ee) =>
                {
                    if (ss != null && ss is OpenFileDialog)
                    {
                        string selectedFile = (ss as OpenFileDialog)!.FileName;
                        var info = new FileInfo(selectedFile);

                        if (string.IsNullOrEmpty(selectedFile) || selectedFile.Contains(".lnk"))
                        {
                            MessageBox.Show("Please select a valid File");
                            ee.Cancel = true;
                        }

                        item.Uri = selectedFile;
                        _dataContext.Update(item);

                        LoadData();
                    }
                };

                dialog.ShowDialog();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as tab_articolo_allegato;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminiazione dell'allegato\n{item.File} ?"))
                {
                    _dataContext.Delete(item);
                    LoadData();
                }
                Mouse.OverrideCursor = null;
            }
        }

        private void cmdOpen_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as tab_articolo_allegato;

            if (item != null)
            {
                try
                {
                    if (File.Exists(item.Uri))
                    {
                        var process = new Process();
                        process.StartInfo = new ProcessStartInfo(item.Uri)
                        {
                            UseShellExecute = true
                        };
                        process.Start();
                    }
                    else
                    {
                        ErrorHandler.Validation(string.Format("File non trovato - {0}", item.Uri));
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Validation(ex.ToString());
                }
                Mouse.OverrideCursor = null;
            }
        }
        #endregion
    }
}
