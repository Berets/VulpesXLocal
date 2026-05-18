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
using System.Windows.Threading;
using VulpesX.DAL;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Stores;

namespace VulpesX.Modules.Default.Stores
{
    /// <summary>
    /// Interaction logic for StoreMovementsWindow.xaml
    /// </summary>
    public partial class StoreMovementsWindow : FluentDefaultWindow
    {
        private StoreMovementsWindowViewModel _dataContext;
        public StoreMovementsWindow(StoreMovementsWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            dtpMoved.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpMoved.Culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            dtpMoved.Culture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            if (!_dataContext.IsInsert || (_dataContext.IsInsert && _dataContext.IsFixedProduct))
            {
                _dataContext.SelectedStore = _dataContext.Stores?.Where(w => w.id == _dataContext.Data.store_id).FirstOrDefault();
                _dataContext.Data.Causal = _dataContext.Causals?.Where(w => w.id == _dataContext.Data.causal_id).FirstOrDefault();
                _dataContext.Data.Product = _dataContext.Products?.Where(w => w.ID == _dataContext.Data.product_id).FirstOrDefault();
                _dataContext.Data.Supplier = _dataContext.Suppliers?.Where(w => w.abecod == _dataContext.Data.supplier_id).FirstOrDefault();
            }

            // save old value
            _dataContext.Data.OldVersion = new store_movements_history()
            {
                company_id = _dataContext.Data.company_id,
                id = _dataContext.Data.id,
                date = _dataContext.Data.date,
                store_id = _dataContext.Data.store_id,
                product_id = _dataContext.Data.product_id,
                causal_id = _dataContext.Data.causal_id,
                quantity = _dataContext.Data.quantity,
                document_id = _dataContext.Data.document_id,
                document_date = _dataContext.Data.document_date,
                document_row = _dataContext.Data.document_row,
                note = _dataContext.Data.note,
                add_user = _dataContext.UserID,
                order_id = _dataContext.Data.order_id,
                engage_id = _dataContext.Data.engage_id,
                lot = _dataContext.Data.lot,
                expire = _dataContext.Data.expire,
                goods_receipt_id = _dataContext.Data.goods_receipt_id,
                goods_location = _dataContext.Data.goods_location,
                supplier_id = _dataContext.Data.supplier_id,
                document_year = _dataContext.Data.document_year,
                supplier_lot = _dataContext.Data.supplier_lot,
                price = _dataContext.Data.price
            };

            // lock the window
            if (_dataContext.Data.engage_id.HasValue || _dataContext.Data.goods_receipt_id.HasValue || !string.IsNullOrWhiteSpace(_dataContext.Data.order_id))
            {
                gMain.IsEnabled = false;
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            CreateLot();

            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                    this.DialogResult = true;
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }

        #region Autocompletes
        private void acStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedStore != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedStore.id))
            {
                _dataContext.Data.store_id = _dataContext.SelectedStore.id;
            }
        }
        private void acCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.Data.Causal != null && !string.IsNullOrWhiteSpace(_dataContext.Data.Causal.id))
            {
                _dataContext.Data.causal_id = _dataContext.Data.Causal.id;
            }
        }
        private void acProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.Data.Product != null && !string.IsNullOrWhiteSpace(_dataContext.Data.Product.ID))
            {
                _dataContext.Data.product_id = _dataContext.Data.Product.ID;

                // get default store if present
                var defaultStore = _dataContext.GetDefaultStoreID();

                if (!string.IsNullOrWhiteSpace(defaultStore))
                    _dataContext.SelectedStore = _dataContext.Stores?.Where(w => w.id == defaultStore).FirstOrDefault();
                if (_dataContext.Data.Product.HasLots)
                {
                    grdLot.Visibility = Visibility.Visible;
                }
                else
                {
                    _dataContext.Data.lot = null;
                    _dataContext.Data.expire = null;
                    grdLot.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void acSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.Data.Supplier != null && _dataContext.Data.Supplier.abecod > 0)
            {
                _dataContext.Data.supplier_id = _dataContext.Data.Supplier.abecod;
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

        private void txtLot_GotFocus(object sender, RoutedEventArgs e)
        {
            CreateLot();
        }
        private void CreateLot()
        {
            if (_dataContext.Data.Product != null)
            {
                if (_dataContext.Data.Product.HasLots && string.IsNullOrWhiteSpace(_dataContext.Data.lot))
                {
                    _dataContext.Data.lot = _dataContext.GenerateLotID();
                }
            }
        }
    }
}
