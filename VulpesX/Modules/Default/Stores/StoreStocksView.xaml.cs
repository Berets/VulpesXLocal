using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Modules.Default.General;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.General;
using VulpesX.ViewModels.Modules.Default.Stores;

namespace VulpesX.Modules.Default.Stores
{
    /// <summary>
    /// Interaction logic for StoreStocksView.xaml
    /// </summary>
    public partial class StoreStocksView : UserControl
    {
        private StoreStocksViewModel _dataContext;
        private int _selectedPage = 0;
        private int _selectedStockPage;

        private store_stocks? _selectedStock;
        private store_stocks_engage? _selectedEngage;

        public StoreStocksView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<StoreStocksViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.Stores = _dataContext.GetStore_Stores();
            _dataContext.SelectedStore = _dataContext.Stores?.Where(w => w.id == string.Empty).FirstOrDefault();

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                    LoadData();
            };

            rgvStocks.DataLoaded += (s, e) =>
            {
                rdpStocks.MoveToPage(_selectedPage);

                txtSearchStocks_TextChanged(txtSearchStocks, null);

                if (_selectedStock != null)
                {
                    rgvStocks.SelectedItem = rgvStocks.Items.Cast<store_stocks>().Where(w => w.store_id == _selectedStock.store_id && w.product_id == _selectedStock.product_id).FirstOrDefault();
                }
                else
                {
                    _dataContext.Movements = null;
                    _dataContext.Engages = null;
                }
                RefreshUI();
            };
            rdpStocks.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            LoadData();
            txtSearchStocks.Focus();
        }

        private async void LoadData()
        {
            if (_dataContext.SelectedStore != null)
                await _dataContext.Load();
        }

        private async void LoadMovements()
        {
            if (_selectedStock != null)
            {
                await _dataContext.LoadMovements(_selectedStock.product_id);
            }
            else
            {
                _dataContext.Movements = null;
            }
        }

        private async void LoadEngagements()
        {
            if (_selectedStock != null)
            {
                await _dataContext.LoadEngagements(_selectedStock.product_id);
            }
            else
            {
                _dataContext.Engages = null;
            }
        }

        private void RefreshUI()
        {
            rmiInsertMovement.IsEnabled = _selectedStock != null;
            rmiInsertEngage.IsEnabled = _selectedStock != null;
            rmiUpdateEngage.IsEnabled = _selectedEngage != null && !_selectedEngage.date_unloaded.HasValue && !_selectedEngage.canceled.HasValue;
            rmiDeleteEngage.IsEnabled = _selectedEngage != null && !_selectedEngage.date_unloaded.HasValue && !_selectedEngage.canceled.HasValue;
            rmiUnloadEngage.IsEnabled = _selectedEngage != null && !_selectedEngage.date_unloaded.HasValue && !_selectedEngage.canceled.HasValue;
        }

        #region User control events
        private void txtSearchEngage_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearchEngage.Text, rgvEngages);
        }

        private void txtSearchStocks_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearchStocks.Text, rgvStocks);
        }

        private void txtSearchMovements_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearchMovements.Text, rgvMovements);
        }
        #endregion

        #region Engage buttons
        private void rmiInsertEngage_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStock != null)
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<StoreStocksEngageWindowViewModel>();
                windowViewModel.Data = new store_stocks_engage()
                {
                    company_id = _selectedStock.company_id,
                    id = _dataContext.GetEngageNumerator(),
                    add_user = _dataContext.UserID,
                    date_engaged = now,
                    product_id = _selectedStock.product_id,
                    ProductFullDescription = $"{_selectedStock.product_id} {_selectedStock.ProductDescription}",
                    UM = _selectedStock.UM,
                    store_id = _selectedStock.store_id,
                    StoreFullDescription = $"{_selectedStock.store_id} {_selectedStock.StoreDescription}"
                };
                windowViewModel.IsInsert = true;

                var wEngage = new StoreStocksEngageWindow(windowViewModel);
                if (wEngage.ShowDialog() == true)
                    LoadData();
            }
        }

        private void rmiUpdateEngage_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEngage != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<StoreStocksEngageWindowViewModel>();
                windowViewModel.Data = _selectedEngage;
                windowViewModel.IsInsert = false;

                var _wEngage = new StoreStocksEngageWindow(windowViewModel);
                _wEngage.ShowDialog();
                LoadData();
            }
        }

        private void rmiDeleteEngage_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEngage != null)
            {
                if (ConfirmHandler.Confirm($"Confermate di voler annullare l'impegno di {(_selectedEngage.quantity ?? 0).ToString("N6")} {_selectedEngage.UM} con ID {_selectedEngage.id} ?"))
                {
                    _selectedEngage.cancel_user = _dataContext.UserID;
                    _selectedEngage.canceled = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                    if (_dataContext.UpdateEngage(_selectedEngage))
                        LoadData();
                }
            }
        }

        private void rmiUnloadEngage_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEngage != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<StoreStocksEngageWindowViewModel>();
                windowViewModel.Data = _selectedEngage;
                windowViewModel.IsInsert = false;
                windowViewModel.IsUnloading = true;


                var _wEngage = new StoreStocksEngageWindow(windowViewModel);
                _wEngage.ShowDialog();
                LoadData();
            }
        }
        #endregion

        private void btnArticleSelect_Click(object sender, RoutedEventArgs e)
        {
            var stock = (sender as Button)?.DataContext as store_stocks;

            if (stock != null && stock.product_id != null)
            {
                var article = _dataContext.GetArticolo(stock.product_id);

                if (article != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTWindowViewModel>();
                    windowViewModel.Data = article;
                    windowViewModel.IsInsert = false;

                    var wArticolo = new ARTWindow(windowViewModel);
                    wArticolo.ShowDialog();
                    LoadData();
                }
            }
        }

        #region Movements buttons
        private void rmiInsertMovement_Click(object sender, RoutedEventArgs e)
        {
            if (rgvStocks.SelectedItem != null)
            {
                var stock = rgvStocks.SelectedItem as store_stocks;

                if (stock != null)
                {
                    var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<StoreMovementsWindowViewModel>();
                    windowViewModel.Data = new store_movements()
                    {
                        id = _dataContext.GetMovementNumerator(),
                        company_id = _dataContext.CompanyID,
                        product_id = stock.product_id,
                        store_id = stock.store_id,
                        date = now
                    };
                    windowViewModel.IsInsert = true;
                    windowViewModel.IsFixedProduct = true;

                    var _wMovement = new StoreMovementsWindow(windowViewModel);
                    _wMovement.ShowDialog();
                    LoadData();
                }
            }
        }
        #endregion

        #region Selections changes
        private void rgvStocks_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            _selectedStock = rgvStocks.SelectedItem as store_stocks;
            _selectedStockPage = rdpStocks.PageIndex;

            LoadMovements();
            LoadEngagements();
            RefreshUI();
        }

        private void rgvEngages_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            _selectedEngage = rgvEngages.SelectedItem as store_stocks_engage;
            RefreshUI();
        }
        #endregion

        #region Autocompletes
        private void acStores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedStore != null)
            {
                LoadData();
            }
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

        private void rgvStocks_RowIsExpandedChanged(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            var row = (e.Row as GridViewRow);
            var item = e.Row.DataContext as store_stocks;

            if (row != null && item != null)
            {
                if (row.IsExpanded)
                {
                    item.Lots = _dataContext.GetStore_Stocks_Lots(item.store_id, item.product_id);
                }
                else
                {
                    item.Lots = null;
                }
            }
        }

        private void rgPrintLotLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as RadGlyph)?.DataContext as store_stocks_lots;

            if (item != null)
            {
                var reportData = _dataContext.GetPrintLotLabel(item.store_id, item.product_id, item.lot);
                if (reportData != null && reportData.Lotto != null)
                    ReportingHandler.PrintPDF(UserContext.Instance.Domain!, Constants.MODULE_SRM, Constants.REPORT_TYPE_LOT_LABEL, _dataContext.CompanyID, reportData, $"Etichetta lotto {reportData.Lotto.lot}", $"EtichettaLotto {reportData.Lotto.lot}.pdf", true);
                else
                    ErrorHandler.Validation($"Impossibile recuperare il lotto");
            }
        }

        private void togShowInfinite_Changed(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
