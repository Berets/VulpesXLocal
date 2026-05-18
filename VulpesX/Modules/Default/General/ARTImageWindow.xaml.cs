using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for ARTImageWindow.xaml
    /// </summary>
    public partial class ARTImageWindow : FluentDefaultWindow
    {
        private ARTImageWindowViewModel _dataContext;
        public ARTImageWindow(ARTImageWindowViewModel dataContext)
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
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
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

                        var attach = new tab_articolo_immagine
                        {
                            SocietaID = _dataContext.CompanyID,
                            ArticoloID = _dataContext.Data.ID,
                            Url = fn,
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
            var item = (sender as Button)?.DataContext as tab_articolo_immagine;

            if (item != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.Multiselect = false;
                dialog.ValidateNames = true;
                dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
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

                        item.Url = selectedFile;
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
            var item = (sender as Button)?.DataContext as tab_articolo_immagine;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminiazione dell'immagine\n{item.File} ?"))
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
            var item = (sender as Button)?.DataContext as tab_articolo_immagine;

            if (item != null)
            {
                try
                {
                    if (File.Exists(item.Url))
                    {
                        var process = new Process();
                        process.StartInfo = new ProcessStartInfo(item.Url)
                        {
                            UseShellExecute = true
                        };
                        process.Start();
                    }
                    else
                    {
                        ErrorHandler.Validation(string.Format("File non trovato - {0}", item.Url));
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
