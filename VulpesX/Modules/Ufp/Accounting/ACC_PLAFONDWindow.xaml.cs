using System;
using System.Collections.Generic;
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
using VulpesX.Models;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Ufp.Accounting
{
    /// <summary>
    /// Interaction logic for ACC_PLAFONDWindow.xaml
    /// </summary>
    public partial class ACC_PLAFONDWindow : FluentDefaultWindow, IWindowFactory
    {
        private ACC_PLAFONDWindowViewModel _dataContext;
        public ACC_PLAFONDWindow(ACC_PLAFONDWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            if (!_dataContext.IsInsert)
            {
                _dataContext.Data.Customer = _dataContext.Customers?.Where(w => w.abecod == _dataContext.Data.Cliacod).FirstOrDefault();
            }
            LoadDetailsData();

            this.Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli {(_dataContext.IsInsert ? "nuova dichiarazione d'intento" : "dichiarazione d'intento n." + _dataContext.Data.FullID)}";

                if (!_dataContext.IsReadonly)
                    this.Title += " - [sola lettura]";

                MoveFocus(new TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
            };
        }

        private async void LoadDetailsData()
        {
            await _dataContext.Load();
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();
            if (string.IsNullOrWhiteSpace(validated))
            {
                var result = _dataContext.Save();

                if (string.IsNullOrEmpty(result))
                {
                    this.DialogResult = true;
                }
                else
                {
                    ErrorHandler.Validation(result);
                }
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }

        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.Wait;
            //var item = (sender as Button).DataContext as ACC_PLAFOND_ROWS;
            //var invoice = new FATTT00FService().Get(item.Cliasoc, item.cliannosol, int.Parse(item.clinumfat));
            //if (item != null)
            //{
            //    var wFATTT00F = new wFATTT00F(ctx, new FATTT00FViewModel() { Data = invoice, IsInsert = false });
            //    Mouse.OverrideCursor = null;
            //    wFATTT00F.Owner = Window.GetWindow(this);
            //    if (wFATTT00F.ShowDialog() == true)
            //        LoadDetailsData();
            //}
        }

        private void cmdEditRows_Click(object sender, RoutedEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.Wait;
            //var item = (sender as Button).DataContext as ACC_PLAFOND_ROWS;
            //var invoice = new FATTT00FService().Get(item.Cliasoc, item.cliannosol, int.Parse(item.clinumfat));
            //var wFATTD00F = new wFATTD00F(ctx, new FATTT00FViewModel()
            //{
            //    Data = invoice,
            //    IsInsert = false
            //});
            //wFATTD00F.Owner = Window.GetWindow(this);
            //Mouse.OverrideCursor = null;
            //if (wFATTD00F.ShowDialog() == true)
            //    LoadDetailsData();
        }
        #endregion

        #region Autocompletes
        private void acCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.Data.Customer != null && _dataContext.Data.Customer.abecod > 0)
            {
                _dataContext.Data.Cliacod = _dataContext.Data.Customer.abecod;
            }
        }

        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }

        private void ac_LostFocus(object sender, RoutedEventArgs e)
        {
            var ac = sender as RadAutoCompleteBox;
            if (ac != null)
            {
                if (ac.SelectedItem == null)
                {
                    ac.SearchText = null;
                }
            }
        }

        #endregion

        private async void rgPrint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)!.DataContext as ACC_PLAFOND_ROWS;

            if (item != null)
            {
                var fileName = _dataContext.GetInvoiceArchived((short)item.cliannosol, item.InvoiceDefinitiveNumber);

                if (!string.IsNullOrEmpty(fileName))
                {
                    var fileData = await StorageHelperCustom.Ufp.DownloadAsync(StorageHelperCustom.Ufp.CLIENTI_FOLDER, fileName);

                    if (fileData != null)
                    {
                        string fullPath = Path.Combine(Path.GetTempPath(), fileName);

                        File.WriteAllBytes(fullPath, fileData);

                        if(File.Exists(fullPath))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = fullPath,
                                UseShellExecute = true
                            });
                        }
                        else
                        {
                            ErrorHandler.Validation($"Non trovato file - {fullPath}");
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation($"Non trovati dati fattura archiviata - {fileName}");
                    }
                }
                else
                {
                    ErrorHandler.Validation("Non trovata fattura archiviata");
                }
            }
        }
    }
}
