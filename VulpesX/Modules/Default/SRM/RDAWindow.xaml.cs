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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for RDAWindow.xaml
    /// </summary>
    public partial class RDAWindow : FluentDefaultWindow
    {
        private RDAWindowViewModel _dataContext;
        public RDAWindow(RDAWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli RDA {(_dataContext.IsInsert ? "nuova" : _dataContext.Data.id)}";
                if (_dataContext.IsReadonly)
                    Title += " - [sola lettura]";
            };

            _dataContext.LoadDetails();

            if (!_dataContext.IsInsert)
            {
                _dataContext.Data.Product = _dataContext.Products?.Where(w => w.ID == _dataContext.Data.product_id).FirstOrDefault();
                _dataContext.Data.Supplier = _dataContext.Data.Suppliers?.Where(w => w.abecod == _dataContext.Data.supplier_id).FirstOrDefault();
            }
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                {
                    Mouse.OverrideCursor = null;
                    this.DialogResult = true;
                }
                Mouse.OverrideCursor = null;
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Autocompletes
        private void acProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_dataContext.Data.Product != null && !string.IsNullOrWhiteSpace(_dataContext.Data.Product.ID))
            {
                _dataContext.Data.product_id = _dataContext.Data.Product.ID;
                if (!_dataContext.Data.supplier_id.HasValue)
                {
                    // retrieve default supplier
                    var defaultSupplier = _dataContext.GetDefaultSupplier(_dataContext.Data.Product.ID);
                    if (defaultSupplier.HasValue)
                        _dataContext.Data.Supplier = _dataContext.Data.Suppliers?.Where(w => w.abecod == defaultSupplier.Value).FirstOrDefault();
                }
            }
            else
            {
                _dataContext.Data.product_id = string.Empty;
            }
        }

        private void acSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.Data.Supplier != null && _dataContext.Data.Supplier.abecod > 0)
            {
                _dataContext.Data.supplier_id = _dataContext.Data.Supplier.abecod;
            }
            else
            {
                _dataContext.Data.supplier_id = null;
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
    }
}
