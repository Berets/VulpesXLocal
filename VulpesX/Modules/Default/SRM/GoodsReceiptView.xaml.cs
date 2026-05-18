using Itenso.TimePeriod;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.Cmp;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.DAL;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for GoodsReceiptView.xaml
    /// </summary>
    public partial class GoodsReceiptView : UserControl
    {
        private GoodsReceiptViewModel _dataContext;

        private int _selectedPage;
        private int _selectedPageHistory;

        public GoodsReceiptView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<GoodsReceiptViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            _dataContext.FromDate = now.AddDays(-7);
            _dataContext.ToDate = now;

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadGoods();
                    LoadHistory();
                }
            };

            GridView.DataLoaded += (s, e) =>
            {
                rdpGrid.MoveToPage(_selectedPage);
                txtSearch_TextChanged(txtSearch, null);
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            GridViewHistory.DataLoaded += (s, e) =>
            {
                rdpGridHistory.MoveToPage(_selectedPageHistory);
                txtSearchHistory_TextChanged(txtSearchHistory, null);
            };
            rdpGridHistory.PageIndexChanged += (s, e) =>
            {
                _selectedPageHistory = e.NewPageIndex;
            };
        }

        private async void LoadGoods()
        {
            if (_dataContext.SelectedSupplier != null)
            {
                await _dataContext.LoadGoods();
            }
        }

        private async void LoadHistory()
        {
            if (_dataContext.FromDate.HasValue && _dataContext.ToDate.HasValue)
            {
                await _dataContext.LoadHistory();
            }
        }

        #region Buttons
        private void cmdReceipt_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as Button)?.DataContext as acq_orders_rows;

            if (item != null)
            {
                item = _dataContext.GetFull(item.id, item.sequence);

                if (item != null && _dataContext.SelectedSupplier != null)
                {
                    var head = _dataContext.Get_Acq_Orders_Head(item.id);

                    if (head != null)
                    {
                        var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<GoodsReceiptReceiveWindowViewModel>();
                        windowViewModel.Data = new acq_goods_receipts()
                        {
                            company_id = item.company_id,
                            id = _dataContext.GetNumerator(),
                            supplier_id = _dataContext.SelectedSupplier.abecod,
                            product_id = item.product_id,
                            document_date = now.Date,
                            order_id = item.id,
                            order_row_id = item.sequence,
                            addedUserID = _dataContext.UserID,
                            unit_id = item.unit_id,
                            document_number = string.Empty,
                        };
                        windowViewModel.OrderRowData = item;
                        windowViewModel.OrderHeadData = head;

                        var wGR = new GoodsReceiptReceiveWindow(windowViewModel);
                        wGR.Owner = Window.GetWindow(this);
                        Mouse.OverrideCursor = null;
                        if (wGR.ShowDialog() == true)
                        {
                            LoadGoods();
                            LoadHistory();

                        }
                    }
                }
            }
        }

        private void cmdClose_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as acq_orders_rows;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate la chiusura (saldo) della riga selezionata ?\n" +
                    $"Ordine n.{item.id} riga {item.sequence} articolo {item.Product?.FullDescriptionSearchable}\nNon sarà più possibile ricevere ulteriore materiale da questa riga d'ordine!"))
                {
                    item.is_closed = true;
                    _dataContext.Update(item);

                    // check if need to close head
                    if ((_dataContext.GetAcq_Orders_Rows(item.id) ?? new()).All(all => all.is_closed))
                    {
                        var head = _dataContext.Get_Acq_Orders_Head(item.id);

                        if (head != null)
                        {
                            head.closed = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                            _dataContext.Update(head);
                        }
                    }
                    LoadGoods();
                    LoadHistory();
                }
            }
        }

        private void cmdEditHistory_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as acq_goods_receipts;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<GoodsReceiptWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wGR = new GoodsReceiptWindow(windowViewModel);
                wGR.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wGR.ShowDialog() == true)
                {
                    LoadGoods();
                    LoadHistory();
                }
            }
        }

        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private void txtSearchHistory_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearchHistory.Text, GridView);
        }
        #endregion

        #region Autocompletes
        private void acSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadGoods();
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

        private void rdtFromDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadHistory();
        }

        private void rdtToDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadHistory();
        }
    }
}
