using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
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
using VulpesX.Models.Models.Accounting;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Ufp.Accounting.Invoicing;

namespace VulpesX.Modules.Ufp.Accounting.Invoicing
{
    /// <summary>
    /// Interaction logic for CheckInvoiceEntranceWindow.xaml
    /// </summary>
    public partial class CheckInvoiceEntranceWindow : FluentDefaultWindow
    {
        private CheckInvoiceEntranceWindowViewModel _dataContext;
        public CheckInvoiceEntranceWindow(CheckInvoiceEntranceWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = _dataContext;

            rgvRows.DataLoaded += (s, e) =>
            {
                txtSearch_TextChanged(txtSearch, null);
            };
            rgvEntrances.DataLoaded += (s, e) =>
            {
                txtSearchEntrance_TextChanged(txtSearchEntrance, null);
            };

            this.PreviewKeyDown += async (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    await LoadData();
                }
            };
            this.Loaded += async (s, e) =>
            {
                await LoadData();
            };
        }

        private async Task LoadData()
        {
            await _dataContext.Load();
        }

        private async void btnSearchEntrata_Click(object sender, RoutedEventArgs e)
        {
            var ddt = txtDDT.Text.TrimEnd();

            if (!string.IsNullOrEmpty(ddt))
            {
                await _dataContext.LoadEntrance(ddt);
            }
            else
            {
                ErrorHandler.Validation("Inserire un numero di DDT");
            }
        }

        private async void rgvRows_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (e.AddedItems.Any())
            {
                txtDDT.Text = string.Empty;

                var row = e.AddedItems[0] as CheckInvoiceEntranceModel.DettaglioModel;

                if (row != null && !string.IsNullOrEmpty(row.DDTID))
                {
                    txtDDT.Text = row.DDTID;

                    await _dataContext.LoadEntrance(row.DDTID);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, rgvRows);
        }

        private void txtSearchEntrance_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, rgvEntrances);
        }

        private async void rgLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as CheckInvoiceEntranceModel.EntrataModel;
            var invoice = rgvRows.SelectedItem as CheckInvoiceEntranceModel.DettaglioModel;

            if(item != null)
            {
                if (invoice != null) {

                    var result = await _dataContext.UpdateEntrance(invoice, item);

                    if (result)
                    {
                        invoice.Entrata = item;

                        if (_dataContext.EntranceLinked == null)
                            _dataContext.EntranceLinked = new List<CheckInvoiceEntranceModel.EntrataModel>();
                        
                        _dataContext.EntranceLinked.Add(item);

                        await _dataContext.LoadEntrance(txtDDT.Text);
                        rgvRows.Items.Refresh();
                    }
                }
                else
                {
                    ErrorHandler.Validation("Selezionare una riga di fattura");
                }
            }
        }
    }
}
