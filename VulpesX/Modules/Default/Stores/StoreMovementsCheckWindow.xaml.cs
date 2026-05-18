using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VulpesX.Models.Models;
using VulpesX.Modules.Default.General;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.General;
using VulpesX.ViewModels.Modules.Default.Stores;

namespace VulpesX.Modules.Default.Stores
{
    /// <summary>
    /// Interaction logic for StoreMovementsCheckWindow.xaml
    /// </summary>
    public partial class StoreMovementsCheckWindow : FluentDefaultWindow
    {
        private StoreMovementsCheckWindowViewModel _dataContext;
        private int _selectedLotPage = 0;
        private int _selectedNoLotPage = 0;

        public StoreMovementsCheckWindow(StoreMovementsCheckWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();
            this.Width = System.Windows.SystemParameters.WorkArea.Width - 100;
            this.Height = System.Windows.SystemParameters.WorkArea.Height - 100;

            this.DataContext = _dataContext;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                    LoadData();
            };

            rgvExistanceLots.DataLoaded += (s, e) =>
            {
                rdpExistanceLots.MoveToPage(_selectedLotPage);

                txtSearchExistance_TextChanged(txtSearchExistance, null);

            };
            rdpExistanceLots.PageIndexChanged += (s, e) =>
            {
                _selectedLotPage = e.NewPageIndex;
            };
            rgvExistanceNoLots.DataLoaded += (s, e) =>
            {
                rdpExistanceNoLots.MoveToPage(_selectedNoLotPage);

                txtSearchExistance_TextChanged(txtSearchExistance, null);

            };
            rdpExistanceNoLots.PageIndexChanged += (s, e) =>
            {
                _selectedNoLotPage = e.NewPageIndex;
            };

            LoadData();
        }

        private async void LoadData()
        {
            await _dataContext.LoadDataLots();
            await _dataContext.LoadDataNoLots();
        }


        #region Full text searches
        private void txtSearchExistance_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearchExistance.Text, rgvExistanceLots);
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearchExistance.Text, rgvExistanceNoLots);
        }
        #endregion

        #region Buttons
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnArticleSelect_Click(object sender, RoutedEventArgs e)
        {
            string? productID = string.Empty;

            if ((sender as Button)?.DataContext is StockCheckExistance.Lots)
            {
                productID = ((sender as Button)?.DataContext as StockCheckExistance.Lots)?.ProductID;
            }
            else
            {
                productID = ((sender as Button)?.DataContext as StockCheckExistance.NoLots)?.ProductID;
            }

            if (!string.IsNullOrEmpty(productID))
            {
                var article = _dataContext.GetArticolo(productID);

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

        private void rgAlign_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as StockCheckExistance.NoLots;
            if (item != null && !string.IsNullOrEmpty(item.StoreID) && !string.IsNullOrEmpty(item.ProductID))
            {
                if (ConfirmHandler.Confirm($"Confermi l'aggiornamento della giacenza con il valore dei movimenti?"))
                {
                    var result = _dataContext.Align(item.StoreID, item.ProductID, string.Empty);
                    if (!result)
                        ErrorHandler.Validation($"Impossibile aggiornare la giacenza");

                    LoadData();
                }
            }
        }

        private void rgAlignLots_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as StockCheckExistance.Lots;

            if (item != null && !string.IsNullOrEmpty(item.StoreID) && !string.IsNullOrEmpty(item.ProductID) && !string.IsNullOrEmpty(item.Lot))
            {
                if (ConfirmHandler.Confirm($"Confermi l'aggiornamento della giacenza con il valore dei movimenti?"))
                {
                    var result = _dataContext.Align(item.StoreID, item.ProductID, item.Lot);
                    if (!result)
                        ErrorHandler.Validation($"Impossibile aggiornare la giacenza");
                    LoadData();
                }
            }
        }
        #endregion

        #region Grid events
        private async void rgvExistanceNoLots_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (e.AddedItems.Any())
            {
                var item = e.AddedItems[0] as StockCheckExistance.NoLots;

                if (item != null && !string.IsNullOrEmpty(item.StoreID) && !string.IsNullOrEmpty(item.ProductID))
                {
                    await _dataContext.LoadMovementsNoLots(item.ProductID, item.StoreID);
                }
            }
        }

        private async void rgvExistanceLots_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (e.AddedItems.Any())
            {
                var item = e.AddedItems[0] as StockCheckExistance.Lots;
                if (item != null && !string.IsNullOrEmpty(item.StoreID) && !string.IsNullOrEmpty(item.ProductID) && !string.IsNullOrEmpty(item.Lot))
                {
                    await _dataContext.LoadMovementsLots(item.ProductID, item.StoreID, item.Lot);
                }
            }
        }
        #endregion
    }
}
